namespace PersonManagement.Application.Contract.PersonCategory
{
    public class PersonCategoryTreeViewModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public List<PersonCategoryTreeViewModel> Children { get; set; } = new();
    }
}
