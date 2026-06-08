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

        public PayrollDetails(decimal quantity, decimal rate, decimal amount, string description)
        {
            Quantity = quantity;
            Rate = rate;
            Amount = amount;
            Description = description;
        }

        public void Edit(decimal quantity, decimal rate, decimal amount, string description)
        {
            Quantity = quantity;
            Rate = rate;
            Amount = amount;
            Description = description;
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
