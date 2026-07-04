using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.PayrollItem;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;

namespace PayrollSystemManagement.Application
{
    public class PayrollItemApplication : IPayrollItemApplication
    {
        private readonly IPayrollItemRepository _payrollItemRepository;

        public PayrollItemApplication(IPayrollItemRepository payrollItemRepository)
        {
            _payrollItemRepository = payrollItemRepository;
        }

        public OperationResult Create(CreatePayrollItem command)
        {
            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.Title))
                return result.Failed("عنوان آیتم الزامی است");

            if (command.BranchId <= 0)
                return result.Failed("شعبه معتبر نیست");

            var payrollItem = new PayrollItems(
                command.Title,
                (PayrollItemType)command.ItemType,
                (PayrollRuleType)command.RuleType,
                command.BranchId,
                command.Taxable,
                command.Insuranceable
            );

            _payrollItemRepository.Create(payrollItem);
            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Edit(EditPayrollItem command)
        {
            var result = new OperationResult();

            if (command.Id <= 0)
                return result.Failed("شناسه نامعتبر است");

            if (string.IsNullOrWhiteSpace(command.Title))
                return result.Failed("عنوان آیتم الزامی است");

            var item = _payrollItemRepository.Get(command.Id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.Edit(
                command.Title,
                (PayrollItemType)command.ItemType,
                (PayrollRuleType)command.RuleType,
                command.Taxable,
                command.Insuranceable
            );

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var item = _payrollItemRepository.Get(id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.Remove();

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var item = _payrollItemRepository.Get(id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.Restore();

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }


        public OperationResult Activate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var item = _payrollItemRepository.Get(id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.Active();

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var item = _payrollItemRepository.Get(id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.NotActive();

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult SetFixed(long id)
        {
            var result = new OperationResult();

            var item = _payrollItemRepository.Get(id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.SetFixed();

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult UnSetFixed(long id)
        {
            var result = new OperationResult();

            var item = _payrollItemRepository.Get(id);
            if (item == null)
                return result.Failed("آیتم یافت نشد");

            item.UnSetFixed();

            _payrollItemRepository.SaveChanges();

            return result.Succedded();
        }


        public EditPayrollItem GetDetails(long id)
        {
            return _payrollItemRepository.GetDetails(id);
        }

        public List<PayrollItemViewModel> GetPayrollDetails()
        {
            return _payrollItemRepository.GetPayrollDetails();
        }

        public List<PayrollItemViewModel> Search(PayrollItemSearchModel searchModel)
        {
            return _payrollItemRepository.Search(searchModel);
        }

    }
}
