namespace BankManagement.Application.Contracts.BankBranch
{
    public class BankBranchViewModel
    {
        public long Id { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string BankId { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string CreationDate { get; set; }
    }
}
