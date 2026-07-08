using _0_Framework.Application;
using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.CompanyBankAccount;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using CodeManagement.Application.Contracts.Code;

namespace BankManagement.Application
{
    public class CompanyBankAccountApplication : ICompanyBankAccountApplication
    {
        private readonly ICompanyBankAccountRepository _companyBankAccountRepository;
        private readonly ICodeApplication _codeApplication;

        public CompanyBankAccountApplication(ICompanyBankAccountRepository companyBankAccountRepository, ICodeApplication codeApplication)
        {
            _companyBankAccountRepository = companyBankAccountRepository;
            _codeApplication = codeApplication;
        }

        public OperationResult Create(CreateCompanyBankAccount command)
        {
            var operation = new OperationResult();

            if (_companyBankAccountRepository.Exists(x => x.AccountNumber == command.AccountNumber))
                return operation.Failed("شماره حساب تکراری است.");

            if (_companyBankAccountRepository.Exists(x => x.Shaba == command.Shaba))
                return operation.Failed("شماره شبا تکراری است.");

            var companyBankAccount = new CompanyBankAccounts(
                command.AccountTitle,
                command.AccountNumber,
                command.CardNumber,
                command.Shaba,
                command.BranchId,
                command.BankId
            );

            _companyBankAccountRepository.Create(companyBankAccount);
            _companyBankAccountRepository.SaveChanges();

            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = companyBankAccount.Id,
                OwnerType = CodeOwnerTypeDTO.CompanyBankAccount,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return operation.Failed(codeResult.Message);

            return operation.Succedded();
        }

        public OperationResult Edit(EditCompanyBankAccount command)
        {
            var operation = new OperationResult();

            var companyBankAccount = _companyBankAccountRepository.Get(command.Id);

            if (companyBankAccount == null)
                return operation.Failed("حساب بانکی یافت نشد.");

            companyBankAccount.Edit(
                command.AccountTitle,
                command.AccountNumber,
                command.CardNumber,
                command.Shaba
            );

            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = companyBankAccount.Id,
                OwnerType = CodeOwnerTypeDTO.CompanyBankAccount,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return operation.Failed(codeResult.Message);

            _companyBankAccountRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<CompanyBankAccountViewModel> GetCompanyBankAccounts()
        {
            var companyBankAccount = _companyBankAccountRepository.GetCompanyBankAccounts();

            if (companyBankAccount == null || companyBankAccount.Count == 0) return companyBankAccount;

            var ids = companyBankAccount.Select(x => x.Id).ToList();
            var codes = _codeApplication.GetListByOwners(ids, CodeOwnerTypeDTO.CompanyBankAccount);

            var dict = codes
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var p in companyBankAccount)
                p.Code = dict.TryGetValue(p.Id, out var v) ? v : null;

            return companyBankAccount;
        }

        public EditCompanyBankAccount GetDetails(long id)
        {
            var details =  _companyBankAccountRepository.GetDetails(id);

            if (details == null) return null;

            var code = _codeApplication.GetByOwner(id, CodeOwnerTypeDTO.CompanyBankAccount);
            details.CurrentCode = code?.Value;
            details.ManualCode = code?.Value;

            return details;
        }

       public List<CompanyBankAccountViewModel> Search(CompanyBankAccountSearchModel searchModel)
        {
            var companyBankAccount = _companyBankAccountRepository.Search(searchModel);

            if (companyBankAccount == null || companyBankAccount.Count == 0) return companyBankAccount;

            var ids = companyBankAccount.Select(x => x.Id).ToList();
            var codes = _codeApplication.GetListByOwners(ids, CodeOwnerTypeDTO.CompanyBankAccount);

            var dict = codes
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var p in companyBankAccount)
                p.Code = dict.TryGetValue(p.Id, out var v) ? v : null;

            return companyBankAccount;
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var person = _companyBankAccountRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Remove();
            _companyBankAccountRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var person = _companyBankAccountRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Restore();
            _companyBankAccountRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var person = _companyBankAccountRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Active();
            _companyBankAccountRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var person = _companyBankAccountRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.NotActive();
            _companyBankAccountRepository.SaveChanges();
            return operation.Succedded();
        }
    }
}
