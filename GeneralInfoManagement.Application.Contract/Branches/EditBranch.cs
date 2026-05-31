namespace GeneralInfoManagement.Application.Contract.Branches
{
    public class EditBranch : CreateBranches
    {
        public long Id { get; set; }
        public string? CurrentCode { get; set; }

    }
}
