using _0_Framework.Application;
using BankManagement.Application.Contracts.Bank;
using BankManagement.Domain.Bank.BankAgg;

namespace BankManagement.Application
{
    public class BankApplication : IBankApplication
    {
        private readonly IBankRepository _bankRepository;

        public BankApplication(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public OperationResult Create(CreateBank command)
        {
            var operation = new OperationResult();

            if (_bankRepository.Exists(x => x.Title == command.Title))
                return operation.Failed("این بانک قبلاً ثبت شده است");

            var bank = new Banks(
                command.Title,
                command.BankTypeId,
                command.Country,
                command.Description,
                command.Logo
            );

            _bankRepository.Create(bank);
            _bankRepository.SaveChanges();

            return operation.Succedded("بانک با موفقیت ایجاد شد");
        }

        public OperationResult Edit(EditBank command)
        {
            var result = new OperationResult();

            var bank = _bankRepository.Get(command.Id);
            if (bank == null)
                return result.Failed("بانک پیدا نشد");

            if (_bankRepository.Exists(x => x.Title == command.Title && x.Id != command.Id))
                return result.Failed("این نام بانک تکراری است");

            bank.Edit(
                command.Title,
                command.BankTypeId,
                command.Country,
                command.Description,
                command.Logo
            );

            _bankRepository.SaveChanges();

            return result.Succedded("ویرایش با موفقیت انجام شد");
        }

        public List<BankViewModel> GetBanks()
        {
            return _bankRepository.GetBanks();
        }

        public EditBank GetDetails(long id)
        {
            return _bankRepository.GetDetails(id); 
        }

        public List<BankViewModel> Search(BankSearchModel searchModel)
        {
            return _bankRepository.Search(searchModel); 
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var bank = _bankRepository.Get(id);
            if (bank == null)
                return operation.Failed("بانک پیدا نشد");

            bank.Remove();
            _bankRepository.SaveChanges();

            return operation.Succedded("بانک حذف شد");
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var bank = _bankRepository.Get(id);
            if (bank == null)
                return operation.Failed("بانک پیدا نشد");

            bank.Restore();
            _bankRepository.SaveChanges();

            return operation.Succedded("بانک بازیابی شد");
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();

            var bank = _bankRepository.Get(id);
            if (bank == null)
                return operation.Failed("بانک پیدا نشد");

            bank.Active();
            _bankRepository.SaveChanges();

            return operation.Succedded("بانک فعال شد");
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var bank = _bankRepository.Get(id);
            if (bank == null)
                return operation.Failed("بانک پیدا نشد");

            bank.NotActive();
            _bankRepository.SaveChanges();

            return operation.Succedded("بانک غیرفعال شد");
        }
    }
}
