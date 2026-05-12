using CsvHelper;
using CsvHelper.Configuration;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using GeneralInfoManagement.Infrastructure.EFCore.Seed.GeneralSeeders.Model;
using System.Globalization;

namespace GeneralInfoManagement.Infrastructure.EFCore.Seed.GeneralSeeders
{
    public static class GeneralSeeders
    {
        public static async Task SeedAsync(GeneralInfoFakeDataContext context)
        {
            // =========================
            // Seed Provinces
            // =========================
            if (!context.Provinces.Any())
            {
                var provincePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Seed",
                    "SeedData",
                    "ostan.csv");

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using var reader = new StreamReader(provincePath);
                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<OstanRecord>().ToList();

                var provinces = new List<Provinces>();

                foreach (var item in records)
                {
                    var province = new Provinces(item.name);

                    province.Id = item.id;

                    provinces.Add(province);
                }

                await context.Provinces.AddRangeAsync(provinces);
                await context.SaveChangesAsync();
            }

            // =========================
            // Seed Cities
            // =========================
            if (!context.Cities.Any())
            {
                var cityPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Seed",
                    "SeedData",
                    "shahr.csv");

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using var reader = new StreamReader(cityPath);
                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<ShahrRecord>().ToList();

                var cities = new List<Cities>();

                foreach (var item in records)
                {
                    // ✅ ستون صحیح در CSV: ostan
                    var city = new Cities(item.name, item.ostan);

                    city.Id = item.id;

                    cities.Add(city);
                }

                await context.Cities.AddRangeAsync(cities);
                await context.SaveChangesAsync();
            }
        }
    }
}
