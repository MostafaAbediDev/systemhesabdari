namespace GeneralInfoManagement.Application.Contract.Branches
{
    public class BranchViewModel
    {
        public long Id { get; set; } 
        public string Title { get; set; }
        public string NationalId { get; set; }
        public string EconomicCode { get; set; }
        public string RegisterNumber { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; } 
        public string TelePhone { get; set; } 
        public string Address { get; set; } 
        public string PostCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long CompanyId { get; set; }
        public long CityId { get; set; }
        public long ProvinceId { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

