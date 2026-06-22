using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.ChequeBookAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace BankManagement.Domain.Bank.ChequeAgg
{
    public class Cheques : EntityBase
    {
        public ChequeType ChequeType { get; private set; }
        public string ChequeNumber { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public ChequeStatus Status { get; private set; }
        public ChequeReferenceType ReferenceType { get; private set; }
        public long ReferenceId { get; private set; }
        public long ChequeBookId { get; private set; }
        public long BranchId { get; private set; }
        public ChequeBooks ChequeBooks { get; private set; }
        public Branches Branches { get; private set; }

        protected Cheques()
        {
        }

        public Cheques(ChequeType chequeType, string chequeNumber, decimal amount, DateTime dueDate, 
            ChequeReferenceType referenceType, long referenceId, long chequeBookId, long branchId)
        {
            ChequeType = chequeType;
            ChequeNumber = chequeNumber;
            Amount = amount;
            DueDate = dueDate;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            ChequeBookId = chequeBookId;
            BranchId = branchId;

            Status = ChequeStatus.New;
        }

        public void Edit(ChequeType chequeType, string chequeNumber, decimal amount, DateTime dueDate,
            ChequeReferenceType referenceType, long referenceId, long chequeBookId, long branchId)
        {
            ChequeType = chequeType;
            ChequeNumber = chequeNumber;
            Amount = amount;
            DueDate = dueDate;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            ChequeBookId = chequeBookId;
            BranchId = branchId;
        }

        public void ChangeStatus(ChequeStatus status)
        {
            Status = status;
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
