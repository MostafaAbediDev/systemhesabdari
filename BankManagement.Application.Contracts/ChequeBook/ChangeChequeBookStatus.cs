namespace BankManagement.Application.Contracts.ChequeBook
{
    public class ChangeChequeBookStatus
    {
        public long Id { get; set; }
        public ChequeBookStatusDTO Status { get; set; }
    }
}