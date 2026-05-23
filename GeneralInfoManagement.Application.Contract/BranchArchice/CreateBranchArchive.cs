namespace GeneralInfoManagement.Application.Contract.BranchArchice
{
    public class CreateBranchArchive
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public long BranchId { get; set; }

    }
}