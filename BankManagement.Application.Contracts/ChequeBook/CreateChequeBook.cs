namespace BankManagement.Application.Contracts.ChequeBook
{
    public class CreateChequeBook
    {
        public int ChequeCount { get; set; }
        public string FirstChequeCode { get; set; }
        public string LastChequeCode { get; set; }
        public string SerialNumber { get; set; }
        public DateTime ReceiveDate { get; set; }
        public long CompanyBankAccountId { get; set; }
    }
}
