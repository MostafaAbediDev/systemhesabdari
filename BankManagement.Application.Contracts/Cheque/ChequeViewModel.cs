namespace BankManagement.Application.Contracts.Cheque
{
    public class ChequeViewModel
    {
        public long Id { get; set; }
        public string ChequeNumber { get; set; }
        public string ChequeType { get; set; }
        public decimal Amount { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string ReferenceType { get; set; }
        public string ChequeBookSerial { get; set; }
        public string BranchName { get; set; }
        public string CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
