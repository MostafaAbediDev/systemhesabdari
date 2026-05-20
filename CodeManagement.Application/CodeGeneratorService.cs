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
                _ => "GEN-"
            };

            // تولید مقدار تصادفی به شکل ۵ رقمی
            var random = new Random();
            var number = random.Next(10000, 99999);

            return $"{prefix}{number}";
        }
    }
}
