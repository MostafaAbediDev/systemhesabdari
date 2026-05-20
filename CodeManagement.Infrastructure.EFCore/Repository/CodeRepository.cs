using _0_FrameWork.Infrastructure;
using CodeManagement.Domain.CodeAgg;

namespace CodeManagement.Infrastructure.EFCore.Repository
{
    public class CodeRepository
    {
        //private readonly CodeFakeDataContext _context;

        //public CodeRepository(CodeFakeDataContext context) : base(context)
        //{
        //    _context = context;
        //}

        public Codes GetByOwner(long ownerId, CodeOwnerType ownerType)
        {
            throw new NotImplementedException();
        }
    }
}
