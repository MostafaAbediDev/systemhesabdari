namespace GeneralInfoManagement.Application.Contract.Branches
{
    public class CreateBranches
    {
        public string Title { get; set; }
        public string NationalId { get; set; }
        public string EconomicCode { get; set; }
        public string RegisterNumber { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string TelePhone { get; set; }

        public string Code { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public long CompanyId { get; set; }
        public long CityId { get; set; }
        public long ProvinceId { get; set; }

        public bool IsCodeAutomatic { get; set; } = true;
        public bool IsMain { get; set; }
        public string? ManualCode { get; set; }

        // ⭐ این دوتا رو اضافه کن
        public bool IsMain { get; set; } = true;   // اصلی / فرعی
    }
}