using _0_FrameWork.Infrastructure;
using CodeManagement.Application.Contracts.Code;
using CodeManagement.Domain.CodeAgg;
using Microsoft.EntityFrameworkCore;

namespace CodeManagement.Infrastructure.EFCore.Repository
{
    public class CodeRepository : RepositoryBase<long, Codes>, ICodeRepository
    {
        private readonly CodeFakeDataContext _context;

        public CodeRepository(CodeFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public Codes GetByOwner(long ownerId, CodeOwnerType ownerType)
        {
            return _context.Codes.FirstOrDefault(x => x.OwnerId == ownerId && x.OwnerType == ownerType);
        }

        public List<Codes> GetByOwners(List<long> ownerIds, CodeOwnerType ownerType)
        {
            return _context.Codes.Where(x => ownerIds.Contains(x.OwnerId) && x.OwnerType == ownerType).ToList();
        }

        public EditCode GetDetails(long id)
        {
            return _context.Codes
                            .Select(x => new EditCode
                            {
                                Id = x.Id,
                                Value = x.Value,
                                OwnerId = x.OwnerId,
                                OwnerType = (CodeOwnerTypeDTO)x.OwnerType
                            })
                            .FirstOrDefault(x => x.Id == id);
        }

        public List<CodeViewModel> Search(CodeSearchModel searchModel)
        {
            var query = _context.Codes
                            .AsNoTracking()
                            .Select(x => new CodeViewModel
                            {
                                Id = x.Id,
                                Value = x.Value,
                                CreationDate = x.CreationDate.ToString("yyyy-MM-dd")
                            });

            if (!string.IsNullOrWhiteSpace(searchModel.Value))
                query = query.Where(x => x.Value.Contains(searchModel.Value));

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
