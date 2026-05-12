namespace PersonManagement.Application.Contract.Persons
{
    public class PersonViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string NationalCode { get; set; }
        public string EconomicCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string PersonTypeTitle { get; set; }
        public string BranchName { get; set; }
        public bool IsLegal { get; set; }
        public bool IsActive { get; set; }
    }
}
