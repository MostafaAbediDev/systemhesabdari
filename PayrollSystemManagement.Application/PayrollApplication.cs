using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.Payroll;
using PayrollSystemManagement.Application.Contracts.Payroll.PayrollSystemManagement.Application.Contracts.Payroll;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;

namespace PayrollSystemManagement.Application
{
    public class PayrollApplication : IPayrollApplication
    {
        private readonly IPayrollRepository _payrollRepository;

        public PayrollApplication(IPayrollRepository payrollRepository)
        {
            _payrollRepository = payrollRepository;
        }

        public OperationResult Create(CreatePayroll command)
        {
            var result = new OperationResult();

            if (command.EmployeeId <= 0)
                return result.Failed("کارمند نامعتبر است");

            if (command.BranchId <= 0)
                return result.Failed("شعبه نامعتبر است");

            if (command.FinancialPeriodId <= 0)
                return result.Failed("دوره مالی نامعتبر است");

            var payroll = new Payrolls(
                command.EmployeeId,
                command.BranchId,
                command.FinancialPeriodId
            );

            _payrollRepository.Create(payroll);
            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Edit(EditPayroll command)
        {
            var result = new OperationResult();

            if (command.Id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var payroll = _payrollRepository.Get(command.Id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.Edit(
                command.EmployeeId,
                command.BranchId,
                command.FinancialPeriodId
            );

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }
        public OperationResult Approve(long id)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.GetWithDetails(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            if (payroll.IsDeleted)
                return result.Failed("پای‌رول حذف شده است");

            payroll.RecalculateTotals(payroll.PayrollDetails);
            payroll.Approve();

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Pay(long id, long accountingDocumentId)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.Get(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.Pay(accountingDocumentId);

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Cancel(long id)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.Get(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.Cancel();

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.Get(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.Remove();

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.Get(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.Restore();

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.Get(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.Active();

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var result = new OperationResult();

            var payroll = _payrollRepository.Get(id);
            if (payroll == null)
                return result.Failed("پای‌رول یافت نشد");

            payroll.NotActive();

            _payrollRepository.SaveChanges();

            return result.Succedded();
        }

        public List<PayrollViewModel> GetAllPayrolls()
        {
            return _payrollRepository.GetAllPayrolls();
        }

        public EditPayroll GetDetails(long id)
        {
            return _payrollRepository.GetDetails(id);
        }

        public List<PayrollViewModel> Search(PayrollSearchModel searchModel)
        {
            return _payrollRepository.Search(searchModel);
        }
    }
}
