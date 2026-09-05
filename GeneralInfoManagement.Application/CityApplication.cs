using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Domain.General.CityAgg;

namespace GeneralInfoManagement.Application
{
    public class CityApplication : ICityApplication
    {
        private readonly ICityRepository _cityRepository;

        public CityApplication(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public List<CityViewModel> GetCitiesByProvinceId(long provinceId)
        {
            return _cityRepository.GetCitiesByProvinceId(provinceId);
        }
    }
}
