using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GeneralInfoManagement.Infrastructure.EFCore
{
    public class GeneralInfoSystemContextFactory : IDesignTimeDbContextFactory<GeneralInfoSystemContext>
    {
        public GeneralInfoSystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<GeneralInfoSystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new GeneralInfoSystemContext(options);
        }
    }
}
