using _0_FrameWork.Domain;
using PersonManagement.Domain.Person.PersonContactAgg;

namespace PersonManagement.Domain.Person.ContactTypeAgg
{
    public class ContactTypes : EntityBase
    {
        public string Title { get; private set; }
        public List<PersonContacts> PersonContacts { get; private set; }

        protected ContactTypes()
        {
            PersonContacts = new List<PersonContacts>();
        }

        public ContactTypes(string title)
        {
            Title = title;
        }

        public void Edit(string title)
        {
            Title = title;
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
