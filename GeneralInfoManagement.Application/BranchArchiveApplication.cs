using _0_Framework.Application;
using GeneralInfoManagement.Application.Contract.BranchArchice;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg.FileUpload;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace GeneralInfoManagement.Application
{
    public class BranchArchiveApplication : IBranchArchiveApplication
    {
        private readonly IBranchArchiveRepository _branchArchiveRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchArchiveFileUploader _fileUploader;

        public BranchArchiveApplication(IBranchArchiveRepository branchArchiveRepository, IBranchArchiveFileUploader fileUploader, 
            IBranchRepository branchRepository)
        {
            _branchArchiveRepository = branchArchiveRepository;
            _fileUploader = fileUploader;
            _branchRepository = branchRepository;
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

            var branch = _branchRepository.Get(command.BranchId);

            if (branch == null)
                return operation.Failed("شعبه مورد نظر یافت نشد.");

            if (_branchArchiveRepository.Exists(x =>
                    x.Title == command.Title &&
                    x.BranchId == command.BranchId &&
                    !x.IsDeleted))
            {
                return operation.Failed(
                    "آرشیوی با این عنوان برای این شعبه قبلاً ثبت شده است.");
            }

            var relativePath = _fileUploader.Upload(
                command.File,
                branch.CompanyId,
                command.BranchId);

            var archive = new BranchArchive(
                command.Title,
                command.Description,
                relativePath,
                command.BranchId);

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

            // بررسی عنوان تکراری در همان شعبه
            if (_branchArchiveRepository.Exists(x =>
                    x.Title == command.Title &&
                    x.BranchId == archive.BranchId &&
                    x.Id != command.Id &&
                    !x.IsDeleted))
            {
                return operation.Failed(
                    "آرشیوی با این عنوان برای این شعبه قبلاً ثبت شده است.");
            }

            // اگر فایل جدید انتخاب شده باشد
            if (!string.IsNullOrWhiteSpace(command.File))
            {
                // پیدا کردن شعبه برای به دست آوردن CompanyId
                var branch = _branchRepository.Get(archive.BranchId);

                if (branch == null)
                    return operation.Failed("شعبه مورد نظر یافت نشد.");

                // مسیر فایل قبلی را قبل از تغییر نگه می‌داریم
                var oldFile = archive.File;

                // ذخیره فایل جدید
                var newFile = _fileUploader.Upload(
                    command.File,
                    branch.CompanyId,
                    archive.BranchId);

                // بروزرسانی اطلاعات آرشیو
                archive.Edit(
                    command.Title,
                    command.Description,
                    newFile);

                _branchArchiveRepository.SaveChanges();

                // بعد از ذخیره موفق، فایل قبلی حذف می‌شود
                if (!string.IsNullOrWhiteSpace(oldFile))
                    _fileUploader.Delete(oldFile);
            }
            else
            {
                // فایل جدید انتخاب نشده؛ فقط اطلاعات متنی تغییر می‌کند
                archive.Edit(
                    command.Title,
                    command.Description,
                    archive.File);

                _branchArchiveRepository.SaveChanges();
            }

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

        public string GetFilePath(long id)
        {
            var archive = _branchArchiveRepository.Get(id);

            if (archive == null)
                return null;

            return _fileUploader.GetFullPath(archive.File);
        }
    }
}
