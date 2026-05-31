namespace PersonManagement.Application.Contract.PersonBank
{
    public class PersonBankSearchModel
    {
        public long? PersonId { get; set; }
        public long? BankBranchId { get; set; }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string Shaba { get; set; }
    }
}
