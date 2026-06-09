using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Province;

namespace GeneralInfoManagement.Domain.General.ProvinceAgg
{
    public interface IProvinceRepository : IRepository<long, Provinces>
    {
        List<ProvinceViewModel> GetProvincesForSelectList();
    }
}
