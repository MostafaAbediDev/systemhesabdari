using CsvHelper.Configuration;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Seed.GeneralSeeders.CsvMaps
{
    public class ProvinceCsvMap : ClassMap<Provinces>
    {
        public ProvinceCsvMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.Title).Name("name");
        }
    }
}
