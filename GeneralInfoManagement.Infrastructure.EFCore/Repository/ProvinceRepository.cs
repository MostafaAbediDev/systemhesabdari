using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using Microsoft.EntityFrameworkCore;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class ProvinceRepository : RepositoryBase<long, Provinces>, IProvinceRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public ProvinceRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<ProvinceViewModel> GetProvinces()
        {
            return _context.Provinces
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Title)
                .Select(x => new ProvinceViewModel
                {
                    Id = x.Id,
                    Title = x.Title
                })
                .ToList();
        }
    }
}
