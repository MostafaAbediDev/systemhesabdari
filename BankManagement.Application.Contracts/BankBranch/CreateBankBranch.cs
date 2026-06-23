namespace BankManagement.Application.Contracts.BankBranch
{
    public class CreateBankBranch
    {
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public long BankId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
    }
}
