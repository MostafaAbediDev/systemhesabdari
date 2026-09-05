namespace GeneralInfoManagement.Application.Contract.City
{
    public interface ICityApplication
    {
        List<CityViewModel> GetCitiesByProvinceId(long provinceId);
    }
}
