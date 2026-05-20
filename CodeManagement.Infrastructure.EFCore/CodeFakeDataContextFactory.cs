using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CodeManagement.Infrastructure.EFCore
{
    public class CodeFakeDataContextFactory : IDesignTimeDbContextFactory<CodeFakeDataContext>
    {
        public CodeFakeDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<CodeFakeDataContext>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new CodeFakeDataContext(options);
        }
    }
}
