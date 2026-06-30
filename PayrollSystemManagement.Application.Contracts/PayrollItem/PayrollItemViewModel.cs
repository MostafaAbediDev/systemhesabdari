namespace PayrollSystemManagement.Application.Contracts.PayrollItem
{
    public class PayrollItemViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsFixed { get; set; }
        public bool Taxable { get; set; }
        public bool Insuranceable { get; set; }
        public string ItemType { get; set; }
        public string RuleType { get; set; }
        public string BranchId { get; set; }
        public string CreationDate { get; set; }
    }
}
