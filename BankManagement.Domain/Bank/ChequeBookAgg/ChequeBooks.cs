using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.ChequeAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;

namespace BankManagement.Domain.Bank.ChequeBookAgg
{
    public class ChequeBooks : EntityBase
    {
        public int ChequeCount { get; private set; }
        public string FirstChequeCode { get; private set; }
        public string LastChequeCode { get; private set; }
        public ChequeBookStatus Status { get; private set; }
        public string SerialNumber { get; private set; }
        public DateTime ReceiveDate { get; private set; }
        public long CompanyBankAccountId { get; private set; }
        public CompanyBankAccounts CompanyBankAccount { get; private set; }
        public List<Cheques> Cheques { get; private set; }

        protected ChequeBooks()
        {
            Cheques = new List<Cheques>();
        }

        public ChequeBooks(int chequeCount, string firstChequeCode, string lastChequeCode, string serialNumber, DateTime receiveDate,
            long companyBankAccountId)
        {
            ChequeCount = chequeCount;
            FirstChequeCode = firstChequeCode;
            LastChequeCode = lastChequeCode;
            SerialNumber = serialNumber;
            ReceiveDate = receiveDate;
            CompanyBankAccountId = companyBankAccountId;

            Status = ChequeBookStatus.Active;

        }

        public void Edit(int chequeCount, string firstChequeCode, string lastChequeCode, string serialNumber, DateTime receiveDate,
            long companyBankAccountId)
        {
            ChequeCount = chequeCount;
            FirstChequeCode = firstChequeCode;
            LastChequeCode = lastChequeCode;
            SerialNumber = serialNumber;
            ReceiveDate = receiveDate;
            CompanyBankAccountId = companyBankAccountId;
        }

        public void ChangeStatus(ChequeBookStatus status)
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
