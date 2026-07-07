namespace FinancialManagement.Application.Contracts.Fund
{
    public class CreateFunds
    {
        public string Title { get; set; }
        public long BranchId { get; set; }
        public long AccountId { get; set; }
    }
}
