namespace CodeManagement.Application.Contracts.Code
{
    public class CreateCode
    {
        public string Value { get; set; }
        public long OwnerId { get; set; }
        public CodeOwnerTypeDTO OwnerType { get; set; }
        public bool IsAutomatic { get; set; }
    }
}
