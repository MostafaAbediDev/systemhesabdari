namespace PersonManagement.Application.Contract.PersonAddress
{
    public class CreatePersonAddress
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public long PersonId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
        public bool IsDefault { get; set; }
    }
}
