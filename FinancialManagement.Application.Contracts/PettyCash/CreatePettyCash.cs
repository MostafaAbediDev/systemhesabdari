namespace FinancialManagement.Application.Contracts.PettyCash
{
    public class CreatePettyCash
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal MaxLimit { get; set; }
        public DateTime LastSettlementDate { get; set; }
        public long BranchId { get; set; }
        public long ResponsiblePersonId { get; set; }
        public long HolderPersonId { get; set; }
        public long AccountId { get; set; }
        public long SettlementAccountId { get; set; }
    }
}
