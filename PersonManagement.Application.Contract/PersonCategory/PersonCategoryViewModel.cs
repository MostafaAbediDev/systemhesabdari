namespace PersonManagement.Application.Contract.PersonCategory
{
    public class PersonCategoryViewModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public long PersonTypeId { get; set; }

        public string PersonTypeTitle { get; set; }

        public long? ParentId { get; set; }

        public string ParentTitle { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string CreationDate { get; set; }
    }
}

