using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Domain.Person.PersonAddressAgg
{
    public class PersonAddresses : EntityBase
    {
        public string Address { get; private set; }
        public string PostalCode { get; private set; }
        public bool IsDefault { get; private set; }
        public long PersonId { get; private set; }
        public long ProvinceId { get; private set; }
        public long CityId { get; private set; }
        public Persons Persons { get; private set; }
        public Provinces Provinces { get; private set; }
        public Cities Cities { get; private set; }

        public PersonAddresses(long personId, long provinceId, long cityId,
        string address, string postalCode)
        {
            PersonId = personId;
            ProvinceId = provinceId;
            CityId = cityId;
            Address = address;
            PostalCode = postalCode;
            IsDefault = false;
        }

        public void Edit(long personId, long provinceId, long cityId,
        string address, string postalCode)
        {
            PersonId = personId;
            ProvinceId = provinceId;
            CityId = cityId;
            Address = address;
            PostalCode = postalCode;
            IsDefault = false;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
