using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
using PersonManagement.Domain.Person.PersonTypeAgg;

namespace PersonManagement.Domain.Person.PersonAgg
{
    public class Persons : EntityBase
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? NationalCode { get; private set; }
        public string? EconomicCode { get; private set; }
        public string? RegistrationNumber { get; private set; }
        public long PersonTypeId { get; private set; }
        public bool IsLegal { get; private set; }
        public decimal CreditLimit { get; private set; }
        public decimal AvailableCredit { get; private set; }
        public long BranchId { get; private set; }
        public Branches Branches { get; private set; }
        public PersonType PersonType { get; private set; }
        public List<PersonBanks> PersonBanks { get; private set; }
        public List<PersonAddresses> PersonAddresses { get; private set; }
        public List<PersonContacts> PersonContacts { get; private set; }

        protected Persons()
        {
            PersonBanks = new List<PersonBanks>();
            PersonAddresses = new List<PersonAddresses>();
            PersonContacts = new List<PersonContacts>();
        }

        public Persons(string firstName, string lastName , bool isLegal, string? nationalCode, string? economicCode,
            string? registrationNumber, long personTypeId, long branchId, decimal creditLimit)
        {
            FirstName = firstName;
            LastName = lastName;
            IsLegal = isLegal;

            if (isLegal)
            {
                EconomicCode = economicCode;
                RegistrationNumber = registrationNumber;
            }
            else
            {
                NationalCode = nationalCode;
            }

            PersonTypeId = personTypeId;
            BranchId = branchId;

            CreditLimit = creditLimit;
            AvailableCredit = creditLimit;
        }

        public void Edit(string firstName, string lastName, string? nationalCode,
            string? economicCode, string? registrationNumber, long personTypeId, long branchId, bool isLegal)
        {
            FirstName = firstName;
            LastName = lastName;

            PersonTypeId = personTypeId;

            SetBranch(branchId);

            IsLegal = isLegal;

            if (IsLegal)
            {
                EconomicCode = economicCode;
                RegistrationNumber = registrationNumber;

                NationalCode = null;
            }
            else
            {
                NationalCode = nationalCode;

                EconomicCode = null;
                RegistrationNumber = null;
            }
        }

        public void UpdateFinancialInfo(decimal creditLimit)
        {

            if (creditLimit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(creditLimit), "سقف اعتبار نمی‌تواند منفی باشد.");
            }

            CreditLimit = creditLimit;
        }

        public void SetBranch(long branchId)
        {
            BranchId = branchId;
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
