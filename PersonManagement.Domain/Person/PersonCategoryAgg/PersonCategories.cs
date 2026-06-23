using _0_FrameWork.Domain;
using PersonManagement.Domain.Person.PersonTypeAgg;

namespace PersonManagement.Domain.Person.PersonCategoryAgg
{
    public class PersonCategory : EntityBase
    {
        public string Title { get; private set; }
        public long PersonTypeId { get; private set; }
        public long? ParentId { get; private set; }
        public PersonType PersonType { get; private set; }
        public PersonCategory Parent { get; private set; }
        public List<PersonCategory> Children { get; private set; }

        protected PersonCategory()
        {
            Children = new List<PersonCategory>();
        }

        public PersonCategory(
            string title,
            long personTypeId,
            long? parentId)
        {
            Title = title;
            PersonTypeId = personTypeId;
            ParentId = parentId;
        }

        public void Edit(
        string title,
        long personTypeId,
        long? parentId)
        {
            Title = title;
            PersonTypeId = personTypeId;
            ParentId = parentId;
        }

        public void ChangeParent(long? parentId)
        {
            ParentId = parentId;
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
