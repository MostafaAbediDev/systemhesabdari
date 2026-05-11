namespace PersonManagement.Application.Contract.PersonAddress
{
    public class PersonAddressViewModel
    {
        public long Id { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public bool IsDefault { get; set; }

        public long ProvinceId { get; set; }

        public long CityId { get; set; }

        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
