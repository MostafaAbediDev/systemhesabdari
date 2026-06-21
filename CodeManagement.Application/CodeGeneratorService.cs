using CodeManagement.Application.Contracts.Code;

namespace CodeManagement.Application
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        public string Generate(CodeOwnerTypeDTO ownerType)
        {
            var prefix = ownerType switch
            {
                CodeOwnerTypeDTO.Branch => "BR-",
                CodeOwnerTypeDTO.Person => "PE-",
                CodeOwnerTypeDTO.CompanyBankAccount => "CBA-",
                _ => throw new ArgumentOutOfRangeException(nameof(ownerType), ownerType, null)
            };

            var number = Random.Shared.Next(10000, 99999);
            return $"{prefix}{number}";
        }

    }
}
