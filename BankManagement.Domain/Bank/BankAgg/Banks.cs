using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.BankTypeAgg;
using BankManagement.Domain.Bank.ChequeBookAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using GeneralInfoManagement.Domain.BaseInfo.PictureAgg;

namespace BankManagement.Domain.Bank.BankAgg
{
    public class Banks : EntityBase
    {
        public string Title { get; private set; }
        public long BankTypeId { get; private set; }
        public string Country { get; private set; }
        public string Description { get; private set; }
        public long PictureId { get; private set; }
        public BankTypes BankTypes { get; private set; }
        public Pictures Pictures { get; private set; }
        public List<CompanyBankAccounts> CompanyBankAccounts { get; private set; }
        public List<ChequeBooks> ChequeBooks { get; private set; }

        protected Banks()
        {
            CompanyBankAccounts = new List<CompanyBankAccounts>();
            ChequeBooks = new List<ChequeBooks>();
        }
        public Banks(string title, long bankTypeId, string country, string description)
        {
            Title = title;
            BankTypeId = bankTypeId;
            Country = country;
            Description = description;
        }

        public void Edit(string title, long bankTypeId, string country, string description)
        {
            Title = title;
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
