namespace PersonManagement.Application.Contract.PersonContact
{
    public class CreatePersonContact
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public long PersonId { get; set; }
        public long ContactTypeId { get; set; }
        public bool IsDefault { get; set; }
    }
}
