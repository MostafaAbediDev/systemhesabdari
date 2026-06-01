using _0_FrameWork.Domain;
<<<<<<< HEAD
using BankManagement.Domain.Bank.BankBrancheAgg;
using BankManagement.Domain.Bank.BankTypeAgg;
=======
<<<<<<< HEAD
=======
using BankManagement.Domain.Bank.BankTypeAgg;
>>>>>>> master
>>>>>>> front
using BankManagement.Domain.Bank.ChequeBookAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;

namespace BankManagement.Domain.Bank.BankAgg
{
    public class Banks : EntityBase
    {
        public string Title { get; private set; }
<<<<<<< HEAD
        public long BankTypeId { get; private set; }
        public string Country { get; private set; }
        public string Description { get; private set; }
        public string Logo { get; private set; }
        public BankTypes BankTypes { get; private set; }
=======
<<<<<<< HEAD
        public int BankType { get; private set; }
=======
>>>>>>> master
        public long BankTypeId { get; private set; }
        public string Country { get; private set; }
        public string Description { get; private set; }
        public long PictureId { get; private set; }
<<<<<<< HEAD
=======
        public BankTypes BankTypes { get; private set; }
>>>>>>> master
        public Pictures Pictures { get; private set; }
>>>>>>> front
        public List<CompanyBankAccounts> CompanyBankAccounts { get; private set; }
        public List<ChequeBooks> ChequeBooks { get; private set; }
        public List<BankBranches> BankBranches { get; private set; }

        protected Banks()
        {
            CompanyBankAccounts = new List<CompanyBankAccounts>();
            ChequeBooks = new List<ChequeBooks>();
        }
<<<<<<< HEAD
        public Banks(string title, long bankTypeId, string country, string description)
        {
            Title = title;
=======
<<<<<<< HEAD
        public Banks(string title, int bankType, long bankTypeId, string country, string description)
        {
            Title = title;
            BankType = bankType;
=======
        public Banks(string title, long bankTypeId, string country, string description)
        {
            Title = title;
>>>>>>> master
>>>>>>> front
            BankTypeId = bankTypeId;
            Country = country;
            Description = description;
        }

<<<<<<< HEAD
        public void Edit(string title, long bankTypeId, string country, string description)
        {
            Title = title;
=======
<<<<<<< HEAD
        public void Edit(string title, int bankType, long bankTypeId, string country, string description)
        {
            Title = title;
            BankType = bankType;
=======
        public void Edit(string title, long bankTypeId, string country, string description)
        {
            Title = title;
>>>>>>> master
>>>>>>> front
            BankTypeId = bankTypeId;
            Country = country;
            Description = description;
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
