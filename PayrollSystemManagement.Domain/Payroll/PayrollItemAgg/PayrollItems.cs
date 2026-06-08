using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace PayrollSystemManagement.Domain.Payroll.PayrollItemAgg
{
    public class PayrollItems : EntityBase
    {
        public string Title { get; private set; }
        public bool IsFixed { get; private set; }
        public bool Taxable { get; private set; }
        public bool Insuranceable { get; private set; }
        public PayrollItemType ItemType { get; private set; }
        public PayrollRuleType RuleType { get; private set; }
        public long BranchId { get; private set; }
        public Branches Branches { get; private set; }

        protected PayrollItems()
        {
        }

        public PayrollItems(string title, PayrollItemType itemType, PayrollRuleType ruleType, long branchId, bool taxable, bool insuranceable)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            Title = title;
            ItemType = itemType;
            RuleType = ruleType;
            BranchId = branchId;
            Taxable = taxable;
            Insuranceable = insuranceable;

            IsFixed = false;
        }

        public void Edit(string title, PayrollItemType itemType, PayrollRuleType ruleType, bool taxable, bool insuranceable)
        {
            if (IsFixed)
                throw new InvalidOperationException("Fixed items cannot be edited");

            Title = title;
            ItemType = itemType;
            RuleType = ruleType;
            Taxable = taxable;
            Insuranceable = insuranceable;
        }

        public void SetFixed()
        {
            IsFixed = true;
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
