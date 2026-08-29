namespace GeneralInfoManagement.Application.Contract.City
{
    public interface ICityApplication
    {
        List<CityViewModel> GetCitiesByProvince(long provinceId);
    }
}
