using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Domain.General.CityAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class CityRepository : RepositoryBase<long, Cities>, ICityRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public CityRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<CityViewModel> GetCitiesByProvince(long provinceId)
        {
            return _context.Cities.Where(x => x.ProvinceId == provinceId && !x.IsDeleted && x.IsActive)
            .Select(x => new CityViewModel
            {
                Id = x.Id,
                Title = x.Title,
                ProvinceId = x.ProvinceId
            })
            .ToList();
        }
    }
}
