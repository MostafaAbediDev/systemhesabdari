using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GeneralInfoManagement.Infrastructure.EFCore
{
    internal class GeneralInfoFakeDataContextFactory : IDesignTimeDbContextFactory<GeneralInfoFakeDataContext>
    {
        public GeneralInfoFakeDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<GeneralInfoFakeDataContext>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new GeneralInfoFakeDataContext(options);
        }
    }
}
