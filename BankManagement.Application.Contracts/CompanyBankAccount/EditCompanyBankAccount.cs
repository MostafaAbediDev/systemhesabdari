namespace BankManagement.Application.Contracts.CompanyBankAccount
{
    public class EditCompanyBankAccount : CreateCompanyBankAccount
    {
        public long Id { get; set; }
        public string? CurrentCode { get; set; }

    }
}
