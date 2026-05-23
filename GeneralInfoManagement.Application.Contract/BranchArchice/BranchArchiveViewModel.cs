namespace GeneralInfoManagement.Application.Contract.BranchArchice
{
    public class BranchArchiveViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public long BranchId { get; set; }
        public string BranchTitle { get; set; }
        public string CreationDate { get; set; }
    }
}