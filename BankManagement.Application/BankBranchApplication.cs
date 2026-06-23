using _0_Framework.Application;
using BankManagement.Application.Contracts.BankBranch;
using BankManagement.Domain.Bank.BankBrancheAgg;

namespace BankManagement.Application
{
    public class BankBranchApplication : IBankBranchApplication
    {
        private readonly IBankBranchRepository _bankBranchRepository;

        public BankBranchApplication(IBankBranchRepository bankBranchRepository)
        {
            _bankBranchRepository = bankBranchRepository;
        }

        public OperationResult Create(CreateBankBranch command)
        {
            var operation = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.BranchName))
                return operation.Failed("نام شعبه الزامی است.");

            if (string.IsNullOrWhiteSpace(command.BranchCode))
                return operation.Failed("کد شعبه الزامی است.");

            if (command.BankId <= 0)
                return operation.Failed("بانک را انتخاب نمایید.");

            if (command.ProvinceId <= 0)
                return operation.Failed("استان را انتخاب نمایید.");

            if (command.CityId <= 0)
                return operation.Failed("شهر را انتخاب نمایید.");

            if (_bankBranchRepository.Exists(x => x.BranchCode == command.BranchCode))
                return operation.Failed("کد شعبه تکراری است.");

            var bankBranch = new BankBranches(
                command.BranchName.Trim(),
                command.BranchCode.Trim(),
                command.Address,
                command.Telephone,
                command.BankId,
                command.ProvinceId,
                command.CityId);

            _bankBranchRepository.Create(bankBranch);
            _bankBranchRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditBankBranch command)
        {
            var operation = new OperationResult();

            var bankBranch = _bankBranchRepository.Get(command.Id);

            if (bankBranch == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            if (string.IsNullOrWhiteSpace(command.BranchName))
                return operation.Failed("نام شعبه الزامی است.");

            if (string.IsNullOrWhiteSpace(command.BranchCode))
                return operation.Failed("کد شعبه الزامی است.");

            if (command.BankId <= 0)
                return operation.Failed("بانک را انتخاب نمایید.");

            if (command.ProvinceId <= 0)
                return operation.Failed("استان را انتخاب نمایید.");

            if (command.CityId <= 0)
                return operation.Failed("شهر را انتخاب نمایید.");

            if (_bankBranchRepository.Exists(x =>
                    x.BranchCode == command.BranchCode &&
                    x.Id != command.Id))
            {
                return operation.Failed("کد شعبه تکراری است.");
            }

            bankBranch.Edit(
                command.BranchName.Trim(),
                command.BranchCode.Trim(),
                command.Address,
                command.Telephone,
                command.BankId,
                command.ProvinceId,
                command.CityId);

            _bankBranchRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<BankBranchViewModel> GetBankBranches()
        {
            return _bankBranchRepository.GetBankBranches();
        }

        public EditBankBranch GetDetails(long id)
        {
            return _bankBranchRepository.GetDetails(id);
        }

        public List<BankBranchViewModel> Search(BankBranchSearchModel searchModel)
        {
            return _bankBranchRepository.Search(searchModel);
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var bankBranch = _bankBranchRepository.Get(id);

            if (bankBranch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            bankBranch.Remove();

            _bankBranchRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var bankBranch = _bankBranchRepository.Get(id);

            if (bankBranch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            bankBranch.Restore();

            _bankBranchRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();

            var bankBranch = _bankBranchRepository.Get(id);

            if (bankBranch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            bankBranch.Active();

            _bankBranchRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var bankBranch = _bankBranchRepository.Get(id);

            if (bankBranch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            bankBranch.NotActive();

            _bankBranchRepository.SaveChanges();

            return operation.Succedded();
        }
    }
}
