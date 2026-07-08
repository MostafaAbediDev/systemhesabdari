using _0_Framework.Application;
using FinancialManagement.Application.Contracts.Fund;
using FinancialManagement.Domain.FundAgg;

namespace FinancialManagement.Application
{
    public class FundApplication : IFundApplication
    {
        private readonly IFundRepository _fundRepository;

        public FundApplication(IFundRepository fundRepository)
        {
            _fundRepository = fundRepository;
        }

        public OperationResult Create(CreateFunds command)
        {
            var operation = new OperationResult();


            if (string.IsNullOrWhiteSpace(command.Title))
                return operation.Failed("عنوان صندوق را وارد کنید");


            var duplicate = _fundRepository.Search(new FundSearchModel
            {
                Title = command.Title
            })
            .Any(x => x.Title == command.Title);


            if (duplicate)
                return operation.Failed("صندوقی با این عنوان قبلاً ثبت شده است");


            var fund = new Funds(
                command.Title,
                command.BranchId,
                command.AccountId
            );


            _fundRepository.Create(fund);
            _fundRepository.SaveChanges();


            return operation.Succedded();
        }



        public OperationResult Edit(EditFund command)
        {
            var operation = new OperationResult();


            if (string.IsNullOrWhiteSpace(command.Title))
                return operation.Failed("عنوان صندوق را وارد کنید");


            var fund = _fundRepository.Get(command.Id);


            if (fund == null)
                return operation.Failed("صندوق مورد نظر یافت نشد");


            var duplicate = _fundRepository.Search(new FundSearchModel
            {
                Title = command.Title
            })
            .Any(x => x.Title == command.Title && x.Id != command.Id);


            if (duplicate)
                return operation.Failed("صندوقی با این عنوان وجود دارد");


            fund.Edit(
                command.Title,
                command.BranchId,
                command.AccountId
            );


            _fundRepository.SaveChanges();


            return operation.Succedded();
        }



        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();


            var fund = _fundRepository.Get(id);


            if (fund == null)
                return operation.Failed("صندوق مورد نظر یافت نشد");


            fund.Remove();


            _fundRepository.SaveChanges();


            return operation.Succedded();
        }



        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();


            var fund = _fundRepository.Get(id);


            if (fund == null)
                return operation.Failed("صندوق مورد نظر یافت نشد");


            fund.Restore();


            _fundRepository.SaveChanges();


            return operation.Succedded();
        }



        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();


            var fund = _fundRepository.Get(id);


            if (fund == null)
                return operation.Failed("صندوق مورد نظر یافت نشد");


            fund.Active();

            _fundRepository.SaveChanges();

            return operation.Succedded();
        }



        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var fund = _fundRepository.Get(id);

            if (fund == null)
                return operation.Failed("صندوق مورد نظر یافت نشد");

            fund.NotActive();

            _fundRepository.SaveChanges();
            return operation.Succedded();
        }


        public EditFund GetDetails(long id)
        {
            return _fundRepository.GetDetails(id);  
        }

        public List<FundViewModel> GetFunds()
        {
            return _fundRepository.GetFunds();
        }

        public List<FundViewModel> Search(FundSearchModel searchModel)
        {
            return _fundRepository.Search(searchModel);
        }
    }
}
