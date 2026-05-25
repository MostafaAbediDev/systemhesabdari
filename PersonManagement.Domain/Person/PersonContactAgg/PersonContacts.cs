using _0_FrameWork.Domain;
using PersonManagement.Domain.Person.ContactTypeAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Domain.Person.PersonContactAgg
{
    public class PersonContacts : EntityBase
    {
        public string Value { get; private set; }
        public string Description { get; private set; }
        public bool IsDefault { get; private set; }
        public  long PersonId { get; private set; }
        public long ContactTypeId { get; private set; }
        public Persons Persons { get; private set; }
        public ContactTypes ContactTypes { get; private set; }

        public PersonContacts(string value, string description, long personId, long contactTypeId)
        {
            Value = value;
            Description = description;
            PersonId = personId;
            ContactTypeId = contactTypeId;
        }

        public void Edit(string value, string description, long contactTypeId)
        {
            Value = value;
            Description = description;
            ContactTypeId = contactTypeId;
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
