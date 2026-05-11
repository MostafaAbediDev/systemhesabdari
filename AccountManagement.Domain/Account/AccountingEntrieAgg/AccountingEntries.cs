using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace AccountManagement.Domain.Account.AccountingEntrieAgg
{
    public class AccountingEntries : EntityBase
    {
        public decimal Debit { get; private set; }
        public decimal Credit { get; private set; }
        public string Description { get; private set; }
        public int LineNumber { get; private set; }
        public long AccountingDocumentId { get; private set; }
        public long AccountId { get; private set; }
        public long? PersonId { get; private set; }
        public AccountingDocuments AccountingDocument { get; private set; }
        public Accounts Account { get; private set; }
        public Persons Person { get; private set; }

        public AccountingEntries(decimal debit, decimal credit, string description, int lineNumber)
        {
            Debit = debit;
            Credit = credit;
            Description = description;
            LineNumber = lineNumber;
        }

        public void Edit(decimal debit, decimal credit, string description, int lineNumber)
        {
            Debit = debit;
            Credit = credit;
            Description = description;
            LineNumber = lineNumber;
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
