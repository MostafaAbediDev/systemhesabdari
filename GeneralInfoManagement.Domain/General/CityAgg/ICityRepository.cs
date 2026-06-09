using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.City;

namespace GeneralInfoManagement.Domain.General.CityAgg
{
    public interface ICityRepository : IRepository<long, Cities>
    {
        List<CityViewModel> GetCitiesByProvince(long provinceId);
    }
}
