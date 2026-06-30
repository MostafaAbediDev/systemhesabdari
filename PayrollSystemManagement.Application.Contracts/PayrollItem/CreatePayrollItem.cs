namespace PayrollSystemManagement.Application.Contracts.PayrollItem
{
    public class CreatePayrollItem
    {
        public string Title { get; set; }
        public bool IsFixed { get; set; }
        public bool Taxable { get; set; }
        public bool Insuranceable { get; set; }
        public PayrollItemTypeDTO ItemType { get; set; }
        public PayrollRuleTypeDTO RuleType { get; set; }
        public long BranchId { get; set; }
    }
}
