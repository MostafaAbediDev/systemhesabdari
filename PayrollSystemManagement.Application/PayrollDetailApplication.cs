using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.PayrollDetail;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;

namespace PayrollSystemManagement.Application
{
    public class PayrollDetailApplication : IPayrollDetailApplication
    {

        private readonly IPayrollDetailRepository _payrollDetailRepository;

        public PayrollDetailApplication(IPayrollDetailRepository payrollDetailRepository)
        {
            _payrollDetailRepository = payrollDetailRepository;
        }

        public OperationResult Create(CreatePayrollDetail command)
        {
            var result = new OperationResult();

            if (command.Quantity <= 0)
                return result.Failed("تعداد باید بزرگتر از صفر باشد");

            if (command.Rate < 0)
                return result.Failed("نرخ نمی‌تواند منفی باشد");

            if (command.PayrollItemId <= 0)
                return result.Failed("آیتم حقوقی معتبر نیست");

            var amount = command.Quantity * command.Rate;

            var detail = new PayrollDetails(
                command.Quantity,
                command.Rate,
                command.Description,
                command.PayrollItemId
            );

            _payrollDetailRepository.Create(detail);
            _payrollDetailRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Edit(EditPayrollDetails command)
        {
            var result = new OperationResult();

            if (command.Id <= 0)
                return result.Failed("شناسه نامعتبر است");

            if (command.Quantity <= 0)
                return result.Failed("تعداد باید بزرگتر از صفر باشد");

            if (command.Rate < 0)
                return result.Failed("نرخ نمی‌تواند منفی باشد");

            var detail = _payrollDetailRepository.Get(command.Id);
            if (detail == null)
                return result.Failed("آیتم حقوقی یافت نشد");

            var amount = command.Quantity * command.Rate;

            detail.Edit(
                command.Quantity,
                command.Rate,
                command.Description,
                command.PayrollItemId
            );

            _payrollDetailRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var detail = _payrollDetailRepository.Get(id);
            if (detail == null)
                return result.Failed("آیتم یافت نشد");

            detail.Remove();

            _payrollDetailRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var detail = _payrollDetailRepository.Get(id);
            if (detail == null)
                return result.Failed("آیتم یافت نشد");

            detail.Restore();

            _payrollDetailRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var result = new OperationResult();

            var detail = _payrollDetailRepository.Get(id);
            if (detail == null)
                return result.Failed("آیتم یافت نشد");

            detail.Active();

            _payrollDetailRepository.SaveChanges();

            return result.Succedded();
        }


        public OperationResult Deactivate(long id)
        {
            var result = new OperationResult();

            var detail = _payrollDetailRepository.Get(id);
            if (detail == null)
                return result.Failed("آیتم یافت نشد");

            detail.NotActive();

            _payrollDetailRepository.SaveChanges();

            return result.Succedded();
        }


        public EditPayrollDetails GetDetails(long id)
        {
            return _payrollDetailRepository.GetDetails(id);
        }

        public List<PayrollDetailViewModel> GetPayrollDetails()
        {
            return _payrollDetailRepository.GetPayrollDetails();
        }

        public List<PayrollDetailViewModel> Search(PayrollDetailsSearchModel searchModel)
        {
            return _payrollDetailRepository.Search(searchModel);
        }
    }
}
