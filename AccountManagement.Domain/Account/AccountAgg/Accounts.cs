using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingEntrieAgg;
using AccountManagement.Domain.Account.AccountLinkAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;


namespace AccountManagement.Domain.Account.AccountAgg
{
    public class Accounts : EntityBase
    {
        public string Code { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Level { get; private set; }
        public int AccountType { get; private set; }
        public int Nature { get; private set; }
        public bool IsPersonRelated { get; private set; }
        public bool IsProductRelated { get; private set; }
        public bool IsBankRelated { get; private set; }
        public bool IsFundRelated { get; private set; }
        public bool IsEmployeeRelated { get; private set; }
        public bool AllowManualEntry { get; private set; }
        public bool IsControlAccount { get; private set; }
        public decimal OpeningBalance { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public long? ParentId { get; private set; }
        public long CompanyId { get; private set; }
        public Accounts Parent { get; private set; }
        public Companies Company { get; private set; }
        public List<Accounts> Children { get; private set; }
        public List<AccountLinks> AccountLink { get; private set; }
        public List<AccountingEntries> AccountingEntrie { get; private set; }


        protected Accounts()
        {
            Children = new List<Accounts>();
            AccountLink = new List<AccountLinks>();
            AccountingEntrie = new List<AccountingEntries>();
        }

        public Accounts(string code, string title, string description, int level, int accountType, int nature, decimal openingBalance, DateTime updatedAt)
        {
            Code = code;
            Title = title;
            Description = description;
            Level = level;
            AccountType = accountType;
            Nature = nature;
            OpeningBalance = openingBalance;
            IsPersonRelated = false;
            IsProductRelated = false;
            IsBankRelated = false;
            IsFundRelated = false;
            IsEmployeeRelated = false;
            AllowManualEntry = false;
            IsControlAccount = false;
            UpdatedAt = updatedAt;
        }

        public void Edit(string code, string title, string description, int level, int accountType, int nature, decimal openingBalance, DateTime updatedAt)
        {
            Code = code;
            Title = title;
            Description = description;
            Level = level;
            AccountType = accountType;
            Nature = nature;
            OpeningBalance = openingBalance;
            IsPersonRelated = false;
            IsProductRelated = false;
            IsBankRelated = false;
            IsFundRelated = false;
            IsEmployeeRelated = false;
            AllowManualEntry = false;
            IsControlAccount = false;
            UpdatedAt = updatedAt;
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
