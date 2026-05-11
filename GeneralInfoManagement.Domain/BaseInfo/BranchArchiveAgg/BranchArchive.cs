using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg
{
    public class BranchArchive : EntityBase
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string File { get; private set; }
        public long BranchId { get; private set; }
        public Branches Branch { get; private set; }

        public BranchArchive(string title, string description, string file)
        {
            Title = title;
            Description = description;
            File = file;
        }

        public void Edit(string title, string description, string file)
        {
            Title = title;
            Description = description;
            File = file;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
