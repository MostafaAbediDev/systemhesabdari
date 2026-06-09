using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class ProvinceRepository : RepositoryBase<long, Provinces>, IProvinceRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public ProvinceRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<ProvinceViewModel> GetProvincesForSelectList()
        {
            return _context.Provinces.Where(x => !x.IsDeleted && x.IsActive)
            .Select(x => new ProvinceViewModel
            {
                Id = x.Id,
                Title = x.Title
            })
            .ToList();
        }
    }
}
