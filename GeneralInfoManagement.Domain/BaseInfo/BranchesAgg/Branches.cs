using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace GeneralInfoManagement.Domain.BaseInfo.BranchesAgg
{
    public class Branches : EntityBase
    {
        public string Title { get; private set; }
        public string NationalId { get; private set; }
        public string EconomicCode { get; private set; }
        public string RegisterNumber { get; private set; }
        public string Email { get; private set; }
        public string MobilePhone { get; private set; }
        public string TelePhone { get; private set; }
        public string Address { get; private set; }
        public string PostCode { get; private set; }
        public bool IsMain { get; private set; }
        public long CompanyId { get; private set; }
        public long ProvinceId { get; private set; }
        public long CityId { get; private set; }
        public Location Location { get; private set; }
        public Companies Company { get; private set; }
        public Provinces Provinces { get; private set; }
        public Cities Cities { get; private set; }
        public List<BranchArchive> BranchArchive { get; private set; }
        public List<FinancialPeriods> FinancialPeriod { get; private set; }

        protected Branches()
        {
            BranchArchive = new List<BranchArchive>();
            FinancialPeriod = new List<FinancialPeriods>();
        }

        public Branches(string title, string nationalId, string economicCode,
            string registerNumber, string email, string mobilePhone,
            string address, string postCode, Location location, long companyId, string telePhone, long provinceId, long cityId)
        {
            Title = title;
            NationalId = nationalId;
            EconomicCode = economicCode;
            RegisterNumber = registerNumber;
            Email = email;
            MobilePhone = mobilePhone;
            Address = address;
            PostCode = postCode;
            Location = location;
            CompanyId = companyId;
            TelePhone = telePhone;
            ProvinceId = provinceId;
            CityId = cityId;
        }

        public void Edit(string title, string nationalId, string economicCode,
            string registerNumber, string email, string mobilePhone,
            string address, string postCode, Location location, long companyId, string telePhone, long provinceId, long cityId)
        {
            Title = title;
            NationalId = nationalId;
            EconomicCode = economicCode;
            RegisterNumber = registerNumber;
            Email = email;
            MobilePhone = mobilePhone;
            Address = address;
            PostCode = postCode;
            Location = location;
            CompanyId = companyId;
            TelePhone = telePhone;
            ProvinceId = provinceId;
            CityId = cityId;
        }

        public void SetAsMain()
        {
            IsMain = true;
        }

        public void UnsetMain()
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

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
