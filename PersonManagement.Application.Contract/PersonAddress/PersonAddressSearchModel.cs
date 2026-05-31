namespace PersonManagement.Application.Contract.PersonAddress
{
    public class PersonAddressSearchModel
    {
        public string Title { get; set; }
        public long PersonId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
    }
}
