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
        public bool IsDeduction { get; private set; }
        public bool Taxable { get; private set; }
        public bool Insuranceable { get; private set; }
        public string Formula { get; private set; }
        public string Description { get; private set; }
        public long PayrollId { get; private set; }
        public long PayrollItemId { get; private set; }
        public Payrolls Payrolls { get; private set; }
        public PayrollItems PayrollItems { get; private set; }

        public PayrollDetails(decimal quantity, decimal rate, decimal amount, string formula, string description)
        {
            Quantity = quantity;
            Rate = rate;
            Amount = amount;
            Formula = formula;
            Description = description;
            IsDeduction = false;
            Taxable = false;
            Insuranceable = false;
        }

        public void Edit(decimal quantity, decimal rate, decimal amount, string formula, string description)
        {
            Quantity = quantity;
            Rate = rate;
            Amount = amount;
            Formula = formula;
            Description = description;
            IsDeduction = false;
            Taxable = false;
            Insuranceable = false;
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
