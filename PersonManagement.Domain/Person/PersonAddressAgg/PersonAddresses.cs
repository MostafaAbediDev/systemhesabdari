using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Domain.Person.PersonAddressAgg
{
    public class PersonAddresses : EntityBase
    {
        public string Title { get; private set; }
        public string Address { get; private set; }
        public string PostalCode { get; private set; }
        public bool IsDefault { get; private set; }
        public long PersonId { get; private set; }
        public long ProvinceId { get; private set; }
        public long CityId { get; private set; }
        public Persons Persons { get; private set; }
        public Provinces Provinces { get; private set; }
        public Cities Cities { get; private set; }

        public PersonAddresses(string title, long personId, long provinceId, long cityId,
        string address, string postalCode)
        {
            Title = title;
            PersonId = personId;
            ProvinceId = provinceId;
            CityId = cityId;
            Address = address;
            PostalCode = postalCode;
        }

        public void Edit(string title, long provinceId, long cityId,
        string address, string postalCode)
        {
            Title = title;
            ProvinceId = provinceId;
            CityId = cityId;
            Address = address;
            PostalCode = postalCode;
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
        public void SetDefault()
        {
            IsDefault = true;
        }

        public void UnsetDefault()
        {
            IsDefault = false;
        }
    }
}
