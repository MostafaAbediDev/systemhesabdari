using _0_Framework.Application;
using GeneralInfoManagement.Application.Contract.Branches;

namespace GeneralInfoManagement.Application
{
    public class BranchApplication : IBranchApplication
    {
        public OperationResult Activate(long id)
        {
            throw new NotImplementedException();
        }

        public OperationResult Create(CreateBranches command)
        {
            throw new NotImplementedException();
        }

        public OperationResult Deactivate(long id)
        {
            throw new NotImplementedException();
        }

        public OperationResult Edit(EditBranch command)
        {
            throw new NotImplementedException();
        }

        public List<BranchViewModel> GetBranches()
        {
            throw new NotImplementedException();
        }

        public EditBranch GetDetails(long id)
        {
            throw new NotImplementedException();
        }

        public OperationResult IsMain(long id)
        {
            throw new NotImplementedException();
        }

        public OperationResult IsNotMain(long id)
        {
            throw new NotImplementedException();
        }

        public OperationResult Remove(long id)
        {
            throw new NotImplementedException();
        }

        public OperationResult Restore(long id)
        {
            throw new NotImplementedException();
        }

        public List<BranchViewModel> Search(BranchSearchModel searchModel)
        {
            throw new NotImplementedException();
        }
    }
}
