namespace PersonManagement.Application.Contract.Persons
{
    public class CreatePerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public bool IsLegal { get; set; }
        public string? NationalCode { get; set; }
        public string? EconomicCode { get; set; }
        public string? RegistrationNumber { get; set; }
        public long PersonTypeId { get; set; }
        public long BranchId { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsCodeAutomatic { get; set; } = true; 
        public string? ManualCode { get; set; } 
    }
}
