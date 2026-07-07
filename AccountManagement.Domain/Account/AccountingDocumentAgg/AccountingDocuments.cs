using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingEntrieAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace AccountManagement.Domain.Account.AccountingDocumentAgg
{
    public class AccountingDocuments : EntityBase
    {
        public long DocumentNumber { get; private set; }
        public DateOnly DocumentDate { get; private set; }
        public AccountingDocumentType DocumentType { get; private set; }
        public AccountingDocumentStatus Status { get; private set; }
        public string? Description { get; private set; }
        public AccountingReferenceType ReferenceType { get; private set; }
        public long? ReferenceId { get; private set; }
        public long? ApprovedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public long CreatedBy { get; private set; }
        public long BranchId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public Branches Branch { get; private set; }
        public FinancialPeriods FinancialPeriod { get; private set; }
        public Persons Creator { get; private set; }
        public Persons? Approver { get; private set; }
        public List<AccountingEntries> AccountingEntrie { get; private set; }

        protected AccountingDocuments()
        {
            AccountingEntrie = new List<AccountingEntries>();
        }

        public AccountingDocuments(
            long documentNumber,
            DateOnly documentDate,
            AccountingDocumentType documentType,
            AccountingReferenceType referenceType,
             long? referenceId,
            long branchId,
            long financialPeriodId,
            long createdBy,
            string? description)
        {
            DocumentNumber = documentNumber;
            DocumentDate = documentDate;
            DocumentType = documentType;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            BranchId = branchId;
            FinancialPeriodId = financialPeriodId;
            CreatedBy = createdBy;
            Description = description;
            Status = AccountingDocumentStatus.Draft;
        }


        public void Edit(DateOnly documentDate, string? description)
        {
            if (Status != AccountingDocumentStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft documents can be edited.");

            DocumentDate = documentDate;
            Description = description;
        }

        public void Approve(long userId)
        {
            if (Status != AccountingDocumentStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft documents can be approved.");

            Status = AccountingDocumentStatus.Approved;

            ApprovedBy = userId;
            ApprovedAt = DateTime.Now;
        }


        public void Reject(long userId)
        {
            if (Status != AccountingDocumentStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft documents can be rejected.");

            Status = AccountingDocumentStatus.Rejected;

            ApprovedBy = userId;
            ApprovedAt = DateTime.Now;
        }


        public void Cancel()
        {
            if (Status == AccountingDocumentStatus.Cancelled)
                return;

            Status = AccountingDocumentStatus.Cancelled;
        }


        public void Remove()
        {
            if (Status == AccountingDocumentStatus.Approved)
                throw new InvalidOperationException(
                    "Approved document cannot be deleted.");

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
