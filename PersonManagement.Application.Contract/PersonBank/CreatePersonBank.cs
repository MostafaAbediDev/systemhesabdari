namespace PersonManagement.Application.Contract.PersonBank
{
    public class CreatePersonBank
    {
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string Shaba { get; set; }
        public long PersonId { get; set; }
        public long BankBranchId { get; set; }
        public bool IsDefault { get; set; }
    }
}
