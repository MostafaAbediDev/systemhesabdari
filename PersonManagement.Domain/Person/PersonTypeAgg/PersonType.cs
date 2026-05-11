using _0_FrameWork.Domain;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Domain.Person.PersonTypeAgg
{
    public class PersonType : EntityBase
    {
        public int Code { get; private set; }
        public string Title { get; private set; }
        public List<Persons> Persons { get; private set; }

        protected PersonType()
        {
            Persons = new List<Persons>();
        }

        public PersonType(int code, string title)
        {
            Code = code;
            Title = title;
        }

        public void Edit(int code, string title)
        {
            Code = code;
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
