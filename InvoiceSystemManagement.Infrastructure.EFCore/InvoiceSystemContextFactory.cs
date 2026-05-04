using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InvoiceSystemManagement.Infrastructure.EFCore
{
    public class InvoiceSystemContextFactory : IDesignTimeDbContextFactory<InvoiceSystemContext>
    {
        public InvoiceSystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<InvoiceSystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new InvoiceSystemContext(options);
        }
    }
}
