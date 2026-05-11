using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;

namespace GeneralInfoManagement.Domain.BaseInfo.BranchesAgg
{
    public class Branches : EntityBase
    {
        public string Title { get; private set; }
        public int NationalId { get; private set; }
        public int EconomicCode { get; private set; }
        public int RegisterNumber { get; private set; }
        public string Code { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Lat_Log { get; private set; }
        public string Address { get; private set; }
        public string PostCode { get; private set; }
        public bool IsMain { get; private set; }
        public long CompanyId { get; private set; }
        public Companies Company { get; private set; }
        public List<BranchArchive> BranchArchive { get; private set; }
        public List<FinancialPeriods> FinancialPeriod { get; private set; }

        protected Branches()
        {
            BranchArchive = new List<BranchArchive>();
            FinancialPeriod = new List<FinancialPeriods>();
        }
        public Branches(string title, int nationalId, int economicCode,
            int registerNumber, string code, string email, string phone,
            string lat_Log, string address, string postCode)
        {
            Title = title;
            NationalId = nationalId;
            EconomicCode = economicCode;
            RegisterNumber = registerNumber;
            Code = code;
            Email = email;
            Phone = phone;
            Lat_Log = lat_Log;
            Address = address;
            PostCode = postCode;
        }

        public void Edit(string title, int nationalId, int economicCode,
            int registerNumber, string code, string email, string phone,
            string lat_Log, string address, string postCode)
        {
            Title = title;
            NationalId = nationalId;
            EconomicCode = economicCode;
            RegisterNumber = registerNumber;
            Code = code;
            Email = email;
            Phone = phone;
            Lat_Log = lat_Log;
            Address = address;
            PostCode = postCode;
        }

        public void Real()
        {
            IsMain = true;
        }

        public void NotReal()
        {
            IsMain = false;
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
