using _0_Framework.Application;
using CodeManagement.Application.Contracts.Code;
using CodeManagement.Domain.CodeAgg;

namespace CodeManagement.Application
{
    public class CodeApplication : ICodeApplication
    {
        private readonly ICodeRepository _codeRepository;
        private readonly ICodeGeneratorService _codegenService;

        public CodeApplication(ICodeRepository codeRepository, ICodeGeneratorService codegenService)
        {
            _codeRepository = codeRepository;
            _codegenService = codegenService;
        }

        public OperationResult Create(CreateCode command)
        {
            var operation = new OperationResult();

            // تعیین نهایی مقدار کد
            var finalValue = command.IsAutomatic
                ? _codegenService.Generate(command.OwnerType)
                : command.Value;

            if (string.IsNullOrWhiteSpace(finalValue))
                return operation.Failed("مقدار کد نمی‌تواند خالی باشد.");

            if (_codeRepository.Exists(x => x.Value == finalValue))
                return operation.Failed("این کد قبلاً در سیستم ثبت شده است.");

            var code = new Codes(finalValue, command.OwnerId, (CodeOwnerType)command.OwnerType);

            _codeRepository.Create(code);
            _codeRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditCode command)
        {
            var operation = new OperationResult();

            var code = _codeRepository.Get(command.Id);
            if (code == null)
                return operation.Failed("کد مورد نظر پیدا نشد.");

            if (_codeRepository.Exists(x => x.Value == command.Value && x.Id != command.Id))
                return operation.Failed("کد جدید تکراری است.");

            code.Edit(command.Value, command.OwnerId, (CodeOwnerType)command.OwnerType);

            _codeRepository.SaveChanges();

            return operation.Succedded();
        }

        public CodeViewModel GetByOwner(long ownerId, CodeOwnerTypeDTO ownerType)
        {
            var code = _codeRepository.GetByOwner(ownerId, (CodeOwnerType)ownerType);

            if (code == null) return null;

            return new CodeViewModel
            {
                Id = code.Id,
                Value = code.Value,
                CreationDate = code.CreationDate.ToString("yyyy/MM/dd")
            };
        }

        public List<CodeViewModel> Search(CodeSearchModel searchModel)
        {
            return _codeRepository.Search(searchModel);
        }

        public OperationResult SetCode(CreateCode command)
        {
            var operation = new OperationResult();

            // تشخیص مقدار نهایی (اتوماتیک یا دستی)
            var finalValue = command.IsAutomatic
                ? _codegenService.Generate(command.OwnerType)
                : command.Value;

            if (string.IsNullOrWhiteSpace(finalValue))
                return operation.Failed("خطا در تولید یا دریافت کد.");

            var existingCode = _codeRepository.GetByOwner(command.OwnerId, (CodeOwnerType)command.OwnerType);

            if (existingCode == null)
            {
                // اگر وجود نداشت، ثبت کن
                if (_codeRepository.Exists(x => x.Value == finalValue))
                    return operation.Failed("کد تولید شده یا وارد شده تکراری است.");

                var newCode = new Codes(finalValue, command.OwnerId, (CodeOwnerType)command.OwnerType);
                _codeRepository.Create(newCode);
            }
            else
            {
                // اگر وجود داشت، ویرایش کن
                if (_codeRepository.Exists(x => x.Value == finalValue && x.Id != existingCode.Id))
                    return operation.Failed("کد جدید با رکورد دیگری تکراری است.");

                existingCode.Edit(finalValue, command.OwnerId, (CodeOwnerType)command.OwnerType);
            }

            _codeRepository.SaveChanges();

            return operation.Succedded();
        }
    }
}
