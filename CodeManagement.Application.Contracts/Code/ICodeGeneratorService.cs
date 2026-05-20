namespace CodeManagement.Application.Contracts.Code
{
    public interface ICodeGeneratorService
    {
        string Generate(CodeOwnerTypeDTO ownerType);

    }
}
