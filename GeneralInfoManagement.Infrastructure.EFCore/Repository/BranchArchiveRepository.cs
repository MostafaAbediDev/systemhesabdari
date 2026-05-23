using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.BranchArchice;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using Microsoft.EntityFrameworkCore;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class BranchArchiveRepository : RepositoryBase<long, BranchArchive>, IBranchArchiveRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public BranchArchiveRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<BranchArchiveViewModel> GetBranchArchives()
        {
            return _context.BranchArchives
                .Select(x => new BranchArchiveViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    File = x.File,
                    BranchTitle = x.Branch.Title,
                    CreationDate = x.CreationDate.ToString()
                })
                .ToList();
        }

        public EditBranchArchive GetDetails(long id)
        {
            return _context.BranchArchives
                .Select(x => new EditBranchArchive
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    File = x.File,
                    BranchId = x.BranchId
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<BranchArchiveViewModel> Search(BranchArchiveSearchModel searchModel)
        {
            var query = _context.BranchArchives
                .Include(x => x.Branch)
                .Select(x => new BranchArchiveViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    BranchId = x.BranchId,
                    BranchTitle = x.Branch.Title,
                    CreationDate = x.CreationDate.ToString()
                });

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));


            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}
