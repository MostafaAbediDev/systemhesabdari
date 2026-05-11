using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
<<<<<<< HEAD
=======
using PersonManagement.Domain.Person.PersonTypeAgg;
>>>>>>> master

namespace PersonManagement.Domain.Person.PersonAgg
{
    public class Persons : EntityBase
    {
        public string Code { get; private set; }
        public string FullName { get; private set; }
        public string NationalCode { get; private set; }
        public string EconomicCode { get; private set; }
        public string RegistrationNumber { get; private set; }
<<<<<<< HEAD
        public int PersonType { get; private set; }
        public bool IsLegal { get; private set; }
        public long BranchId { get; private set; }
        public Branches Branches { get; private set; }
=======
        public long PersonTypeId { get; private set; }
        public bool IsLegal { get; private set; }
        public long BranchId { get; private set; }
        public Branches Branches { get; private set; }
        public PersonType PersonType { get; private set; }
>>>>>>> master
        public List<PersonBanks> PersonBanks { get; private set; }
        public List<PersonAddresses> PersonAddresses { get; private set; }
        public List<PersonContacts> PersonContacts { get; private set; }

        protected Persons()
        {
            PersonBanks = new List<PersonBanks>();
            PersonAddresses = new List<PersonAddresses>();
            PersonContacts = new List<PersonContacts>();
        }

        public Persons(string code, string fullName, string nationalCode,
            string economicCode, string registrationNumber, int personType)
        {
            Code = code;
            FullName = fullName;
            NationalCode = nationalCode;
            EconomicCode = economicCode;
            RegistrationNumber = registrationNumber;
<<<<<<< HEAD
            PersonType = personType;
=======
>>>>>>> master
        }

        public void Edit(string code, string fullName, string nationalCode,
            string economicCode, string registrationNumber, int personType)
        {
            Code = code;
            FullName = fullName;
            NationalCode = nationalCode;
            EconomicCode = economicCode;
            RegistrationNumber = registrationNumber;
<<<<<<< HEAD
            PersonType = personType;
=======
>>>>>>> master
        }

        public void SetBranch(long branchId)
        {
            BranchId = branchId;
        }

        public void Legal()
        {
            IsLegal = true;
        }

        public void IlLegal()
        {
            IsLegal = false;
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
