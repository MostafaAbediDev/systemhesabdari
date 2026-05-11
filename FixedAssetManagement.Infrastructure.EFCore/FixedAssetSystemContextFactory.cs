using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FixedAssetManagement.Infrastructure.EFCore
{
    public class FixedAssetSystemContextFactory : IDesignTimeDbContextFactory<FixedAssetSystemContext>
    {
        public FixedAssetSystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<FixedAssetSystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new FixedAssetSystemContext(options);
        }
    }
}
