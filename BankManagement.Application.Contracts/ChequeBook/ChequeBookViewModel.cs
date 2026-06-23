namespace BankManagement.Application.Contracts.ChequeBook
{
    public class ChequeBookViewModel
    {
        public long Id { get; set; }
        public int ChequeCount { get; set; }
        public string FirstChequeCode { get; set; }
        public string LastChequeCode { get; set; }
        public string SerialNumber { get; set; }
        public string ReceiveDate { get; set; }
        public string Status { get; set; }
        public string CompanyBankAccountTitle { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
