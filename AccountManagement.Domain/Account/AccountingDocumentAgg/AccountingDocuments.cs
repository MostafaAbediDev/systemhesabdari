using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingEntrieAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;

namespace AccountManagement.Domain.Account.AccountingDocumentAgg
{
    public class AccountingDocuments : EntityBase
    {
        public int DocumentNumber { get; private set; }
        public DateTime DocumentDate { get; private set; }
        public int DocumentType { get; private set; }
        public int Status { get; private set; }
        public string Description { get; private set; }
        public int ReferenceType { get; private set; }
        public int? ReferenceId { get; private set; }
        public int? ApprovedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public long CreatedBy { get; private set; }
        public long BranchId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public Branches Branch { get; private set; }
        public FinancialPeriods FinancialPeriod { get; private set; }
        //public Persons Person { get; private set; }
        public List<AccountingEntries> AccountingEntrie { get; private set; }

        protected AccountingDocuments()
        {
            AccountingEntrie = new List<AccountingEntries>();
        }

        public AccountingDocuments(int documentNumber, DateTime documentDate, int documentType, int status, 
            string description, int referenceType, int? referenceId, DateTime? approvedAt)
        {
            DocumentNumber = documentNumber;
            DocumentDate = documentDate;
            DocumentType = documentType;
            Status = status;
            Description = description;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            ApprovedAt = approvedAt;
        }

        public void Edit(int documentNumber, DateTime documentDate, int documentType, int status,
            string description, int referenceType, int? referenceId, DateTime? approvedAt)
        {
            DocumentNumber = documentNumber;
            DocumentDate = documentDate;
            DocumentType = documentType;
            Status = status;
            Description = description;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            ApprovedAt = approvedAt;
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
