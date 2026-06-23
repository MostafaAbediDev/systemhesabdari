namespace BankManagement.Application.Contracts.Bank
{
    public class BankViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string? Logo { get; set; }
        public string BankType { get; set; }
        public string CreationDate { get; set; }
    }
}
