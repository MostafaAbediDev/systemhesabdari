using _0_Framework.Application;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace GeneralInfoManagement.Application
{
    public class BranchApplication : IBranchApplication
    {
        private readonly IBranchRepository _branchRepository;

        public BranchApplication(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public OperationResult Create(CreateBranches command)
        {
            var operation = new OperationResult();

            if (_branchRepository.Exists(x => x.NationalId == command.NationalId))
                return operation.Failed("شعبه‌ای با این شناسه ملی قبلاً ثبت شده است.");

            var location = new Location(command.Latitude, command.Longitude);

            var branch = new Branches(command.Title, command.NationalId, command.EconomicCode,
                                      command.RegisterNumber, command.Email, command.Phone,
                                      command.Address, command.PostCode, location, command.CompanyId);

            _branchRepository.Create(branch);

            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Edit(EditBranch command)
        {
            var operation = new OperationResult();
            var branch = _branchRepository.Get(command.Id);

            if (branch == null)
                return operation.Failed("شعبه مورد نظر یافت نشد.");

            if (_branchRepository.Exists(x => x.NationalId == command.NationalId && x.Id != command.Id))
                return operation.Failed("شناسه ملی تکراری است.");

            var location = new Location(command.Latitude, command.Longitude);

            branch.Edit(command.Title, command.NationalId, command.EconomicCode,
                        command.RegisterNumber, command.Email, command.Phone,
                        command.Address, command.PostCode, location, command.CompanyId);

            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();  

            var branch = _branchRepository.Get(id);
            if (branch == null) return new OperationResult().Failed("یافت نشد.");

            branch.Activate();

            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var branch = _branchRepository.Get(id);
            if (branch == null) return new OperationResult().Failed("یافت نشد.");

            branch.Deactivate();
            _branchRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult IsMain(long id)
        {
            var operation = new OperationResult();

            _branchRepository.ResetAllMainBranches(); 

            var current = _branchRepository.Get(id);

            if (current == null) return new OperationResult().Failed("یافت نشد");

            current.SetAsMain();

            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult IsNotMain(long id)
        {
            var operation = new OperationResult();
            var branch = _branchRepository.Get(id);

            if (branch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            branch.UnsetMain();
            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var branch = _branchRepository.Get(id);

            if (branch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            branch.Remove();
            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();  

            var branch = _branchRepository.Get(id);
            if (branch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            branch.Restore();

            _branchRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<BranchViewModel> GetBranches()
        {
            return _branchRepository.GetAllBranches();
        }

        public EditBranch GetDetails(long id)
        {
            return _branchRepository.GetDetails(id);
        }

        public List<BranchViewModel> Search(BranchSearchModel searchModel)
        {
            return _branchRepository.Search(searchModel);
        }
    }
}
