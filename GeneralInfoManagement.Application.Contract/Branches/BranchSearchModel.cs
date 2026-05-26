namespace GeneralInfoManagement.Application.Contract.Branches
{
    public class BranchSearchModel
    {
        public string Title { get; set; }
        public string NationalId { get; set; }
        public string EconomicCode { get; set; }
        public string RegisterNumber { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
    }
}
