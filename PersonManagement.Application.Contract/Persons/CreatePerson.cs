namespace PersonManagement.Application.Contract.Persons
{
    public class CreatePerson
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public string NationalCode { get; set; }
        public string EconomicCode { get; set; }
        public string RegistrationNumber { get; set; }
        public long BranchId { get; set; }
        public bool IsLegal { get; set; }
        public long PersonTypeId { get; set; }
        public bool IsActive { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
