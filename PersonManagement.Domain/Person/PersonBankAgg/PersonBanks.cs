using _0_FrameWork.Domain;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Domain.Person.PersonBankAgg
{
    public class PersonBanks : EntityBase
    {
        public string BankName { get; private set; }
        public string AccountNumber { get; private set; }
        public string CardNumber { get; private set; }
        public string Shaba { get; private set; }
        public bool IsDefault { get; private set; }
        public long PersonId { get; private set; }
        public Persons Persons { get; private set; }

        public PersonBanks(string bankName, string accountNumber, string cardNumber, string shaba)
        {
            BankName = bankName;
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            IsDefault = false;
        }

        public void Edit(string bankName, string accountNumber, string cardNumber, string shaba)
        {
            BankName = bankName;
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            IsDefault = false;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
