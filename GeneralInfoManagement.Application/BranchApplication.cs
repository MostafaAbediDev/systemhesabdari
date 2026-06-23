using _0_Framework.Application;
using CodeManagement.Application.Contracts.Code;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace GeneralInfoManagement.Application
{
    public class BranchApplication : IBranchApplication
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ICodeApplication  _codeApplication;

        public BranchApplication(IBranchRepository branchRepository, ICodeApplication codeApplication)
        {
            _branchRepository = branchRepository;
            _codeApplication = codeApplication;
        }

        public OperationResult Create(CreateBranches command)
        {
            var operation = new OperationResult();

            var nationalId = command.NationalId;
            if (string.IsNullOrWhiteSpace(nationalId))
                return operation.Failed("شناسه ملی الزامی است.");

            if (_branchRepository.Exists(x => x.NationalId == nationalId))
                return operation.Failed("شعبه‌ای با این شناسه ملی قبلاً ثبت شده است.");
            if (command.IsMain)
                _branchRepository.ResetAllMainBranches();
            var location = new Location(command.Latitude, command.Longitude);
            var branch = new Branches(
                command.Title,
                nationalId,
                command.EconomicCode,
                command.RegisterNumber,
                command.Email,
                command.MobilePhone,
                command.Address,
                command.PostCode,
                location,
                command.CompanyId,
                command.TelePhone,
                command.ProvinceId,
                command.CityId);

            _branchRepository.Create(branch);
            _branchRepository.SaveChanges();

            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = branch.Id,
                OwnerType = CodeOwnerTypeDTO.Branch,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return operation.Failed(codeResult.Message);

            return operation.Succedded();
        }

        public OperationResult Edit(EditBranch command)
        {
            var operation = new OperationResult();
            var branch = _branchRepository.Get(command.Id);
            if (branch == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            var nationalId = command.NationalId;
            if (string.IsNullOrWhiteSpace(nationalId))
                return operation.Failed("شناسه ملی الزامی است.");

            if (_branchRepository.Exists(x => x.NationalId == nationalId && x.Id != command.Id))
                return operation.Failed("شناسه ملی تکراری است.");

            var location = new Location(command.Latitude, command.Longitude);

            branch.Edit(
                command.Title,
                nationalId,
                command.EconomicCode,
                command.RegisterNumber,
                command.Email,
                command.MobilePhone,
                command.Address,
                command.PostCode,
                location,
                command.CompanyId,
                command.TelePhone,
                command.ProvinceId,
                command.CityId);

            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = branch.Id,
                OwnerType = CodeOwnerTypeDTO.Branch,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return operation.Failed(codeResult.Message);

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
            var branches = _branchRepository.GetAllBranches();
            if (branches == null || branches.Count == 0)
                return branches;

            var branchIds = branches.Select(b => b.Id).ToList();
            var codes = _codeApplication.GetListByOwners(branchIds, CodeOwnerTypeDTO.Branch);

            var codeDict = codes
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var b in branches)
                b.Code = codeDict.TryGetValue(b.Id, out var val) ? val : null;

            return branches;
        }

        public EditBranch GetDetails(long id)
        {
            var details = _branchRepository.GetDetails(id);
            if (details == null) return null;

            var code = _codeApplication.GetByOwner(id, CodeOwnerTypeDTO.Branch);
            details.CurrentCode = code?.Value;
            details.ManualCode = code?.Value;

            return details;
        }

        public List<BranchViewModel> Search(BranchSearchModel searchModel)
        {
            var branches = _branchRepository.Search(searchModel);
            if (branches == null || branches.Count == 0)
                return branches;

            var branchIds = branches.Select(b => b.Id).ToList();
            var codes = _codeApplication.GetListByOwners(branchIds, CodeOwnerTypeDTO.Branch);

            var codeDict = codes
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var b in branches)
                b.Code = codeDict.TryGetValue(b.Id, out var val) ? val : null;

            return branches;
        }
    }
}
