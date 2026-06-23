namespace BankManagement.Application.Contracts.Bank
{
    public class CreateBank
    {
        public string Title { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string? Logo { get; set; }
        public long BankTypeId { get; set; }
    }
}
