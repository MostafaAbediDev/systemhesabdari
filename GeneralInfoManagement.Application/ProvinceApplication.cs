using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace GeneralInfoManagement.Application
{
    public class ProvinceApplication : IProvinceApplication
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceApplication(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public List<ProvinceViewModel> GetProvinces()
        {
            return _provinceRepository.GetProvinces();
        }
    }
}
