namespace BankManagement.Application.Contracts.Cheque
{
    public class ChequeSearchModel
    {
        public string ChequeNumber { get; set; }
        public ChequeTypeDTO? ChequeType { get; set; }
        public ChequeStatusDTO? Status { get; set; }
        public long? ChequeBookId { get; set; }
        public long? BranchId { get; set; }
    }
}
