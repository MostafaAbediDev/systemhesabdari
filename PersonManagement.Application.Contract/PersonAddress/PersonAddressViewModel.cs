namespace PersonManagement.Application.Contract.PersonAddress
{
    public class PersonAddressViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public long PersonId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public long ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public long CityId { get; set; }
        public string CityName { get; set; }
    }
}
