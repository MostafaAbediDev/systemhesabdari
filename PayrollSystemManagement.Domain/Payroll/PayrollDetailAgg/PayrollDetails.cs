using _0_FrameWork.Domain;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;

namespace PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg
{
    public class PayrollDetails : EntityBase
    {
        public decimal Quantity { get; private set; }
        public decimal Rate { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public long PayrollId { get; private set; }
        public long PayrollItemId { get; private set; }
        public Payrolls Payrolls { get; private set; }
        public PayrollItems PayrollItems { get; private set; }

        protected PayrollDetails()
        {
        }

        public PayrollDetails(decimal quantity, decimal rate, string description, long payrollItemId)
        {
            if (payrollItemId <= 0)
                throw new ArgumentException(nameof(payrollItemId));

            if (quantity <= 0)
                throw new ArgumentException(nameof(quantity));

            if (rate < 0)
                throw new ArgumentException(nameof(rate));

            Quantity = quantity;
            Rate = rate;
            Description = description;
            PayrollItemId = payrollItemId;

            CalculateAmount();

        }

        public void Edit(decimal quantity, decimal rate, string description, long payrollItemId)
        {
            if (payrollItemId <= 0)
                throw new ArgumentException(nameof(payrollItemId));

            if (quantity <= 0)
                throw new ArgumentException(nameof(quantity));

            if (rate < 0)
                throw new ArgumentException(nameof(rate));

            Quantity = quantity;
            Rate = rate;
            Description = description;
            PayrollItemId = payrollItemId;

            CalculateAmount();

        }

        private void CalculateAmount()
        {
            Amount = Quantity * Rate;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
