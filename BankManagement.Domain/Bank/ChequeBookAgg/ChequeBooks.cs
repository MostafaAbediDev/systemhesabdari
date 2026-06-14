using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.BankAgg;
using BankManagement.Domain.Bank.ChequeAgg;

namespace BankManagement.Domain.Bank.ChequeBookAgg
{
    public class ChequeBooks : EntityBase
    {
        public int ChequeCount { get; private set; }
        public string FirstChequeCode { get; private set; }
        public string LastChequeCode { get; private set; }
        public string OwnerName { get; private set; }
        public long BankId { get; private set; }
        public Banks Banks { get; private set; }
        public List<Cheques> Cheques { get; private set; }

        protected ChequeBooks()
        {
            Cheques = new List<Cheques>();
        }

        public ChequeBooks(int chequeCount, string firstChequeCode, string lastChequeCode, string ownerName)
        {
            ChequeCount = chequeCount;
            FirstChequeCode = firstChequeCode;
            LastChequeCode = lastChequeCode;
            OwnerName = ownerName;
        }

        public void Edit(int chequeCount, int firstChequeCode, int lastChequeCode, string ownerName)
        {
            ChequeCount = chequeCount;
            FirstChequeCode = firstChequeCode;
            LastChequeCode = lastChequeCode;
            OwnerName = ownerName;
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
