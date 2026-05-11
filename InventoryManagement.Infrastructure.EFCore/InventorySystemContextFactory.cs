using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.Infrastructure.EFCore
{
    public class InventorySystemContextFactory : IDesignTimeDbContextFactory<InventorySystemContext>
    {
        public InventorySystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<InventorySystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new InventorySystemContext(options);
        }
    }
}
