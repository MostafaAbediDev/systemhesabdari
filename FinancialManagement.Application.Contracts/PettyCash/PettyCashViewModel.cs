namespace FinancialManagement.Application.Contracts.PettyCash
{
    public class PettyCashViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal MaxLimit { get; set; }
        public DateTime LastSettlementDate { get; set; }
        public string BranchName { get; set; }
        public string ResponsiblePersonName { get; set; }
        public string HolderPersonName { get; set; }
        public string CreationDate { get; set; }
    }
}
