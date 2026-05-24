namespace CodeManagement.Application.Contracts.Code
{
    public class CodeViewModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string CreationDate { get; set; }
        public long OwnerId { get; set; }
    }
}
