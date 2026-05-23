using _0_Framework.Application;
using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.BranchArchice;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;

namespace GeneralInfoManagement.Application
{
    public class BranchArchiveApplication : IBranchArchiveApplication
    {
        private readonly IBranchArchiveRepository _branchArchiveRepository;

        public BranchArchiveApplication(IBranchArchiveRepository branchArchiveRepository)
        {
            _branchArchiveRepository = branchArchiveRepository;
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();

            var archive = _branchArchiveRepository.Get(id);
            if (archive == null) return new OperationResult().Failed("یافت نشد.");

            archive.Active();

            _branchArchiveRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Create(CreateBranchArchive command)
        {
            var operation = new OperationResult();

            if (_branchArchiveRepository.Exists(x => x.Title == command.Title))
                return operation.Failed("آرشیو شعبه‌ای با این عنوان قبلاً ثبت شده است.");

            var archive = new BranchArchive(command.Title, command.Description, command.File);

            _branchArchiveRepository.Create(archive);

            _branchArchiveRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var archive = _branchArchiveRepository.Get(id);
            if (archive == null) return new OperationResult().Failed("یافت نشد.");

            archive.NotActive();
            _branchArchiveRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditBranchArchive command)
        {
            var operation = new OperationResult();
            var archive = _branchArchiveRepository.Get(command.Id);

            if (archive == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            if (_branchArchiveRepository.Exists(x => x.Title == command.Title && x.Id != command.Id))
                return operation.Failed("عنوان تکراری است.");

            archive.Edit(command.Title, command.Description, command.File);

            _branchArchiveRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<BranchArchiveViewModel> GetBranchArchives()
        {
            return _branchArchiveRepository.GetBranchArchives();
        }

        public EditBranchArchive GetDetails(long id)
        {
            return _branchArchiveRepository.GetDetails(id);
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var archive = _branchArchiveRepository.Get(id);

            if (archive == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            archive.Remove();
            _branchArchiveRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var archive = _branchArchiveRepository.Get(id);
            if (archive == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            archive.Restore();

            _branchArchiveRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<BranchArchiveViewModel> Search(BranchArchiveSearchModel searchModel)
        {
            return _branchArchiveRepository.Search(searchModel);    
        }
    }
}
