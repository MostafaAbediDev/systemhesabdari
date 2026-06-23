namespace PersonManagement.Application.Contract.PersonContact
{
    public class PersonContactViewModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public long PersonId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public long ContactTypeId { get; set; }
        public string ContactTypeTitle { get; set; }
    }
}
