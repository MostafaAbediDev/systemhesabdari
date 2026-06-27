namespace BankManagement.Application.Contracts.CompanyBankAccount
{
    public class CreateCompanyBankAccount
    {
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string Shaba { get; set; }
        public long BranchId { get; set; }
        public long BankId { get; set; }
        public bool IsCodeAutomatic { get; set; } = true;
        public string? ManualCode { get; set; }
    }
}
