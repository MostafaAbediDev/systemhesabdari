using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.ChequeBookAgg;
using BankManagement.Domain.Bank.ReceiptsPaymentAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace BankManagement.Domain.Bank.ChequeAgg
{
    public class Cheques : EntityBase
    {
        public int ChequeType { get; private set; }
        public string ChequeNumber { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public int Status { get; private set; }
        public int ReferenceType { get; private set; }
        public long ReferenceId { get; private set; }
        public long ChequeBookId { get; private set; }
        public long BranchId { get; private set; }
        public ChequeBooks ChequeBooks { get; private set; }
        public Branches Branches { get; private set; }
        public List<ReceiptsPayments> ReceiptsPayments { get; private set; }

        protected Cheques()
        {
            ReceiptsPayments = new List<ReceiptsPayments>();
        }

        public Cheques(int chequeType, string chequeNumber, decimal amount, DateTime dueDate, int status, int referenceType, long referenceId)
        {
            ChequeType = chequeType;
            ChequeNumber = chequeNumber;
            Amount = amount;
            DueDate = dueDate;
            Status = status;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
        }

        public void Edit(int chequeType, string chequeNumber, decimal amount, DateTime dueDate, int status, int referenceType, long referenceId)
        {
            ChequeType = chequeType;
            ChequeNumber = chequeNumber;
            Amount = amount;
            DueDate = dueDate;
            Status = status;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
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
