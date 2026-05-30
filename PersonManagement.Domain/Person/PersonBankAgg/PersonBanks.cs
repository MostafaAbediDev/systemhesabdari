using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.BankBrancheAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Domain.Person.PersonBankAgg
{
    public class PersonBanks : EntityBase
    {
        public string AccountNumber { get; private set; }
        public string CardNumber { get; private set; }
        public string Shaba { get; private set; }
        public bool IsDefault { get; private set; }
        public long PersonId { get; private set; }
        public long BankBranchId { get; private set; }
        public Persons Persons { get; private set; }
        public BankBranches BankBranches { get; private set; }


        public PersonBanks(string accountNumber, string cardNumber, string shaba, long personId, long bankBranchId)
        {
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            PersonId = personId;
            BankBranchId = bankBranchId;
        }

        public void Edit(string accountNumber, string cardNumber, string shaba, long bankBranchId)
        {
            AccountNumber = accountNumber;
            CardNumber = cardNumber;
            Shaba = shaba;
            BankBranchId = bankBranchId;
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
        public void SetDefault()
        {
            IsDefault = true;
        }

        public void UnsetDefault()
        {
            IsDefault = false;
        }
    }
}
