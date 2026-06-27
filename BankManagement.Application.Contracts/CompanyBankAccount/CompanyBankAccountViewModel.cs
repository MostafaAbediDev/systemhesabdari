namespace BankManagement.Application.Contracts.CompanyBankAccount
{
    public class CompanyBankAccountViewModel
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string Shaba { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string CreationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
