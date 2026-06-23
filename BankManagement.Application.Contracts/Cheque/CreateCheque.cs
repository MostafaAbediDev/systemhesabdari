namespace BankManagement.Application.Contracts.Cheque
{
    public class CreateCheque
    {
        public ChequeTypeDTO ChequeType { get; set; }
        public string ChequeNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public ChequeReferenceTypeDTO ReferenceType { get; set; }
        public long ReferenceId { get; set; }
        public long ChequeBookId { get; set; }
        public long BranchId { get; set; }
    }
}
