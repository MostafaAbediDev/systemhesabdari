namespace PersonManagement.Application.Contract.PersonBank
{
    public class PersonBankViewModel
    {
        public long Id { get; set; }

        public string BankName { get; set; }

        public string AccountNumber { get; set; }

        public string CardNumber { get; set; }

        public string Shaba { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
