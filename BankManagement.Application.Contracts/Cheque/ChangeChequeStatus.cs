namespace BankManagement.Application.Contracts.Cheque
{
    public class ChangeChequeStatus
    {
        public long Id { get; set; }
        public ChequeStatusDTO Status { get; set; }
    }
}
