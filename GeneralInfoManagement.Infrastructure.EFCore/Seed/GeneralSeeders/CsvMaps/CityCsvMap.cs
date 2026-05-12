using CsvHelper.Configuration;
using GeneralInfoManagement.Domain.General.CityAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Seed.GeneralSeeders.CsvMaps
{
    public class CityCsvMap : ClassMap<Cities>
    {
        public CityCsvMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.Title).Name("name");
            Map(m => m.ProvinceId).Name("ostan");
        }
    }
}
