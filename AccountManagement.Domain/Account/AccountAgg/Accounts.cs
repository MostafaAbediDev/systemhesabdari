using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingEntrieAgg;
using AccountManagement.Domain.Account.AccountLinkAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;


namespace AccountManagement.Domain.Account.AccountAgg
{
    public class Accounts : EntityBase
    {
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public AccountLevel Level { get; private set; }
        public AccountType AccountType { get; private set; }
        public AccountNature Nature { get; private set; }
        public bool IsPersonRelated { get; private set; }
        public bool IsProductRelated { get; private set; }
        public bool IsBankRelated { get; private set; }
        public bool IsFundRelated { get; private set; }
        public bool IsEmployeeRelated { get; private set; }
        public bool AllowManualEntry { get; private set; }
        public bool IsControlAccount { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public long? ParentId { get; private set; }
        public long CompanyId { get; private set; }
        public Accounts? Parent { get; private set; }
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

        public Accounts(string title, string? description, AccountLevel level, AccountType accountType,
            AccountNature nature, long companyId)
        {
            Title = title;
            Description = description;
            Level = level;
            AccountType = accountType;
            Nature = nature;
            CompanyId = companyId;
            IsPersonRelated = false;
            IsProductRelated = false;
            IsBankRelated = false;
            IsFundRelated = false;
            IsEmployeeRelated = false;
            AllowManualEntry = false;
            IsControlAccount = false;
            IsActive = true;
        }

        public void Edit(string title, string? description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException(
                    "Account title is required.");

            Title = title;
            Description = description;
        }

        public void ChangeAccountClassification(AccountLevel level, AccountType accountType, AccountNature nature)
        {
            if (AccountingEntrie.Any())
                throw new InvalidOperationException(
                    "Account classification cannot be changed because account has transactions.");

            Level = level;
            AccountType = accountType;
            Nature = nature;

            UpdatedAt = DateTime.Now;
        }

        public void ChangeParent(long? parentId)
        {
            if (AccountingEntrie.Any())
                throw new InvalidOperationException(
                    "Account parent cannot be changed because account has transactions.");

            ParentId = parentId;

            UpdatedAt = DateTime.Now;
        }

        public void EnablePersonRelation()
        {
            IsPersonRelated = true;
        }

        public void DisablePersonRelation()
        {
            IsPersonRelated = false;
        }

        public void EnableProductRelation()
        {
            IsProductRelated = true;
        }

        public void DisableProductRelation()
        {
            IsProductRelated = false;
        }

        public void EnableBankRelation()
        {
            IsBankRelated = true;
        }

        public void DisableBankRelation()
        {
            IsBankRelated = false;
        }

        public void EnableFundRelation()
        {
            IsFundRelated = true;
        }

        public void DisableFundRelation()
        {
            IsFundRelated = false;
        }

        public void EnableEmployeeRelation()
        {
            IsEmployeeRelated = true;
        }

        public void DisableEmployeeRelation()
        {
            IsEmployeeRelated = false;
        }

        public void AllowManualEntries()
        {
            AllowManualEntry = true;
        }

        public void DisableManualEntries()
        {
            AllowManualEntry = false;
        }

        public void SetAsControlAccount()
        {
            IsControlAccount = true;
        }

        public void RemoveControlAccount()
        {
            IsControlAccount = false;
        }
        public void Remove()
        {
            if (AccountingEntrie.Any())
                throw new InvalidOperationException(
                    "Account with transactions cannot be deleted.");

            if (Children.Any())
                throw new InvalidOperationException(
                    "Account with child accounts cannot be deleted.");

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
