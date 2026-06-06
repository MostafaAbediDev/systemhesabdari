namespace PersonManagement.Application.Contract.PersonCategory
{
    public class PersonCategorySearchModel
    {
        public string Title { get; set; }

        public long? PersonTypeId { get; set; }

        public long? ParentId { get; set; }
    }
}

