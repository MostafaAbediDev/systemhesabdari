namespace PersonManagement.Application.Contract.PersonContact
{
    public class PersonContactViewModel
    {
        public long Id { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public long ContactTypeId { get; set; }

        public string ContactTypeTitle { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
