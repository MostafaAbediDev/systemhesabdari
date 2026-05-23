namespace GeneralInfoManagement.Application.Contract.FinancialPeriod
{
    public class FinancialPeriodViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BranchTitle { get; set; }
        public long BranchId { get; set; }
    }
}
