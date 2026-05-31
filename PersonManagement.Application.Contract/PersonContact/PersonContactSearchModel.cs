namespace PersonManagement.Application.Contract.PersonContact
{
    public class PersonContactSearchModel
    {
        public string Value { get; set; }
        public long? PersonId { get; set; }
        public long? ContactTypeId { get; set; }
    }
}
