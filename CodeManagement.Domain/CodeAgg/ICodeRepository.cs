using _0_FrameWork.Domain;

namespace CodeManagement.Domain.CodeAgg
{
    public interface ICodeRepository : IRepository<long, Codes>
    {
        Codes GetByOwner(long ownerId, CodeOwnerType ownerType);
    }
}
