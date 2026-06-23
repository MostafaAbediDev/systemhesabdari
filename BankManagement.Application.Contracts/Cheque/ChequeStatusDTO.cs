namespace BankManagement.Application.Contracts.Cheque
{
    public enum ChequeStatusDTO
    {
        New = 1,
        Issued = 2,
        Received = 3,
        Passed = 4,
        Bounced = 5,
        Cancelled = 6
    }
}