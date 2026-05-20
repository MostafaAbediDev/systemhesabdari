using _0_Framework.Application;

namespace CodeManagement.Application.Contracts.Code
{
    public interface ICodeApplication
    {
        OperationResult Create(CreateCode command);
        OperationResult Edit(EditCode command);
        OperationResult SetCode(CreateCode command);
        CodeViewModel GetByOwner(long ownerId, CodeOwnerTypeDTO ownerType);
        List<CodeViewModel> Search(CodeSearchModel searchModel);
    }
}
