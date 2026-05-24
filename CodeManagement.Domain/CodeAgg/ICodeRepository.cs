using _0_FrameWork.Domain;
using CodeManagement.Application.Contracts.Code;

namespace CodeManagement.Domain.CodeAgg
{
    public interface ICodeRepository : IRepository<long, Codes>
    {
        Codes GetByOwner(long ownerId, CodeOwnerType ownerType);
        EditCode GetDetails(long id);
        List<CodeViewModel> Search(CodeSearchModel searchModel);
        List<Codes> GetByOwners(List<long> ownerIds, CodeOwnerType ownerType);

    }
}
