using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using Microsoft.EntityFrameworkCore;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class BranchRepository : RepositoryBase<long, Branches>, IBranchRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public BranchRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<BranchViewModel> GetAllBranches()
        {
            return _context.Branches
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new BranchViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    NationalId = x.NationalId,
                    EconomicCode = x.EconomicCode,
                    RegisterNumber = x.RegisterNumber,
                    Email = x.Email,
                    MobilePhone = x.MobilePhone,
                    TelePhone = x.TelePhone,
                    Address = x.Address,
                    PostCode = x.PostCode,
                    Latitude = x.Location.Latitude,
                    Longitude = x.Location.Longitude,
                    CompanyId = x.CompanyId,
                    CityId = x.CityId,
                    CityName = x.Cities.Title,
                    ProvinceName = x.Provinces.Title,
                    ProvinceId = x.ProvinceId,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreationDate
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public EditBranch GetDetails(long id)
        {
            return _context.Branches
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditBranch
                {
                    Id = x.Id,
                    Title = x.Title,
                    NationalId = x.NationalId,
                    EconomicCode = x.EconomicCode,
                    RegisterNumber = x.RegisterNumber,
                    Email = x.Email,
                    MobilePhone = x.MobilePhone,
                    TelePhone = x.TelePhone,
                    Address = x.Address,
                    PostCode = x.PostCode,
                    Latitude = x.Location.Latitude,
                    Longitude = x.Location.Longitude,
                    CompanyId = x.CompanyId,
                    CityId = x.CityId,
                    ProvinceId = x.ProvinceId
                })
                .FirstOrDefault();
        }

        public List<BranchViewModel> Search(BranchSearchModel searchModel)
        {
            var query = _context.Branches
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));

            if (!string.IsNullOrWhiteSpace(searchModel.NationalId))
                query = query.Where(x => x.NationalId.Contains(searchModel.NationalId));

            if (!string.IsNullOrWhiteSpace(searchModel.EconomicCode))
                query = query.Where(x => x.EconomicCode.Contains(searchModel.EconomicCode));

            if (!string.IsNullOrWhiteSpace(searchModel.RegisterNumber))
                query = query.Where(x => x.RegisterNumber.Contains(searchModel.RegisterNumber));

            if (searchModel.ProvinceId > 0)
                query = query.Where(x => x.ProvinceId == searchModel.ProvinceId);

            if (searchModel.CityId > 0)
                query = query.Where(x => x.CityId == searchModel.CityId);


            return query
                .Select(x => new BranchViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    NationalId = x.NationalId,
                    EconomicCode = x.EconomicCode,
                    RegisterNumber = x.RegisterNumber,
                    Email = x.Email,
                    MobilePhone = x.MobilePhone,
                    TelePhone = x.TelePhone,
                    Address = x.Address,
                    PostCode = x.PostCode,
                    Latitude = x.Location.Latitude,
                    Longitude = x.Location.Longitude,
                    CompanyId = x.CompanyId,
                    CityId = x.CityId,
                    ProvinceId = x.ProvinceId,
                    CityName = x.Cities.Title,
                    ProvinceName = x.Provinces.Title,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreationDate
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public void ResetAllMainBranches()
        {
            _context.Branches
                .Where(x => x.IsMain)
                .ExecuteUpdate(x => x.SetProperty(b => b.IsMain, false));
        }
    }
}
