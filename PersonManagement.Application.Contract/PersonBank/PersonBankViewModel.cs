namespace PersonManagement.Application.Contract.PersonBank
{
    public class PersonBankViewModel
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string PersonFullName { get; set; }
        public long BankBranchId { get; set; }
        public string BankBranchName { get; set; }
        public string BankName { get; set; }  
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string Shaba { get; set; }
        public bool IsDefault { get; set; }
    }
}
