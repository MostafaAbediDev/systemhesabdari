using _0_Framework.Application;
using FinancialManagement.Application.Contracts.PettyCash;
using FinancialManagement.Domain.PettyCashAgg;

namespace FinancialManagement.Application
{
    public class PettyCasheApplication : IPettyCashApplication
    {

        private readonly IPettyCashRepository _pettyCashRepository;

        public PettyCasheApplication(IPettyCashRepository pettyCashRepository)
        {
            _pettyCashRepository = pettyCashRepository;
        }

        public OperationResult Create(CreatePettyCash command)
        {
            var operation = new OperationResult();


            if (string.IsNullOrWhiteSpace(command.Title))
                return operation.Failed("عنوان تنخواه الزامی است");


            if (command.InitialAmount < 0)
                return operation.Failed("مبلغ اولیه نمی‌تواند منفی باشد");


            if (command.MaxLimit < command.InitialAmount)
                return operation.Failed("سقف تنخواه نمی‌تواند کمتر از مبلغ اولیه باشد");


            var exists = _pettyCashRepository.Search(new PettyCashSearchModel
            {
                Title = command.Title
            })
            .Any(x => x.Title == command.Title);


            if (exists)
                return operation.Failed("تنخواهی با این عنوان وجود دارد");



            var pettyCash = new PettyCashes(
                command.Title,
                command.Description,
                command.InitialAmount,
                command.MaxLimit,
                command.LastSettlementDate,
                command.BranchId,
                command.ResponsiblePersonId,
                command.HolderPersonId,
                command.AccountId,
                command.SettlementAccountId
            );


            _pettyCashRepository.Create(pettyCash);
            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }



        public OperationResult Edit(EditPettyCash command)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(command.Id);


            if (pettyCash == null)
                return operation.Failed("تنخواه مورد نظر یافت نشد");


            if (string.IsNullOrWhiteSpace(command.Title))
                return operation.Failed("عنوان تنخواه الزامی است");


            if (command.MaxLimit < 0)
                return operation.Failed("سقف تنخواه نامعتبر است");



            pettyCash.Edit(
                command.Title,
                command.Description,
                command.MaxLimit,
                command.LastSettlementDate
            );


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }



        public OperationResult ChangeMaxLimit(ChangeMaxLimitDTO command)
        {
            var operation = new OperationResult();


            if (command.MaxLimit <= 0)
                return operation.Failed("سقف تنخواه باید بیشتر از صفر باشد");


            var pettyCash = _pettyCashRepository.Get(command.Id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.ChangeMaxLimit(command.MaxLimit);


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }




        public OperationResult ChangeResponsiblePerson(ChangeResponsiblePersonDTO command)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(command.Id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.ChangeResponsiblePerson(
                command.ResponsiblePersonId
            );


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }




        public OperationResult ChangeSettlementAccount(ChangeSettlementAccountDTO command)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(command.Id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.ChangeSettlementAccount(
                command.SettlementAccountId
            );


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public OperationResult IncreaseBalance(IncreaseBalanceDTO command)
        {
            var operation = new OperationResult();


            if (command.Amount <= 0)
                return operation.Failed("مبلغ افزایش باید بیشتر از صفر باشد");


            var pettyCash = _pettyCashRepository.Get(command.Id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");



            pettyCash.IncreaseBalance(command.Amount);


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public OperationResult DecreaseBalance(DecreaseBalanceDTO command)
        {
            var operation = new OperationResult();


            if (command.Amount <= 0)
                return operation.Failed("مبلغ کاهش باید بیشتر از صفر باشد");


            var pettyCash = _pettyCashRepository.Get(command.Id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");



            if (pettyCash.CurrentBalance < command.Amount)
                return operation.Failed("موجودی تنخواه کافی نیست");



            pettyCash.DecreaseBalance(command.Amount);


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.Remove();


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.Restore();


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.Active();


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();


            var pettyCash = _pettyCashRepository.Get(id);


            if (pettyCash == null)
                return operation.Failed("تنخواه یافت نشد");


            pettyCash.NotActive();


            _pettyCashRepository.SaveChanges();


            return operation.Succedded();
        }





        public EditPettyCash GetDetails(long id)
        {
            return _pettyCashRepository.GetDetails(id);
        }




        public List<PettyCashViewModel> GetPettyCashes()
        {
            return _pettyCashRepository.GetPettyCashes();
        }




        public List<PettyCashViewModel> Search(PettyCashSearchModel searchModel)
        {
            return _pettyCashRepository.Search(searchModel);
        }
    }
}
