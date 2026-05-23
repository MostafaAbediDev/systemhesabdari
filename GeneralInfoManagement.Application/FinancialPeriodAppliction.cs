using _0_Framework.Application;
using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;

namespace GeneralInfoManagement.Application
{
    public class FinancialPeriodAppliction : IFinancialPeriodApplication
    {

        private readonly IFinancialPeriodRepository _financialPeriodRepository;

        public FinancialPeriodAppliction(IFinancialPeriodRepository financialPeriodRepository)
        {
            _financialPeriodRepository = financialPeriodRepository;
        }

        public OperationResult Create(CreateFinancialPeriod command)
        {
            var operation = new OperationResult();

            if (_financialPeriodRepository.Exists(x => x.Title == command.Title))
                return operation.Failed("نام دوره مالی تکراری است.");

            if (command.StartDate >= command.EndDate)
                return operation.Failed("تاریخ پایان باید بعد از تاریخ شروع باشد.");

            var period = new FinancialPeriods(command.Title, command.StartDate, command.EndDate, command.BranchId);

            _financialPeriodRepository.Create(period);
            _financialPeriodRepository.SaveChanges();
            return operation.Succedded();
        }


        public OperationResult Edit(EditFinancialPeriod command)
        {
            var operation = new OperationResult();
            var period = _financialPeriodRepository.Get(command.Id);

            if (period == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            if (_financialPeriodRepository.Exists(x => x.Title == command.Title && x.Id != command.Id))
                return operation.Failed("نام دوره مالی تکراری است.");

            if (command.StartDate >= command.EndDate)
                return operation.Failed("تاریخ پایان باید بعد از تاریخ شروع باشد.");

            period.Edit(command.Title, command.StartDate, command.EndDate, command.BranchId);

            _financialPeriodRepository.SaveChanges();
            return operation.Succedded();
        }

        public EditFinancialPeriod GetDetails(long id)
        {
            return _financialPeriodRepository.GetDetails(id);
        }

        public List<FinancialPeriodViewModel> GetFinancialPeriods()
        {
            return _financialPeriodRepository.GetFinancialPeriods();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();
            var period = _financialPeriodRepository.Get(id);
            if (period == null) return operation.Failed(ApplicationMessages.RecordNotFound);

            period.Remove();
            _financialPeriodRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();
            var period = _financialPeriodRepository.Get(id);
            if (period == null) return operation.Failed(ApplicationMessages.RecordNotFound);

            period.Restore();
            _financialPeriodRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var period = _financialPeriodRepository.Get(id);
            if (period == null) return operation.Failed(ApplicationMessages.RecordNotFound);

            period.Active();
            _financialPeriodRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var period = _financialPeriodRepository.Get(id);
            if (period == null) return operation.Failed(ApplicationMessages.RecordNotFound);

            period.NotActive();
            _financialPeriodRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<FinancialPeriodViewModel> Search(FinancialPeriodSearchModel searchModel)
        {
            return _financialPeriodRepository.Search(searchModel);   
        }
    }
}
