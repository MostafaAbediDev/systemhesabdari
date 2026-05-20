using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CodeManagement.Infrastructure.EFCore
{
    public class CodeFakeDataContextFactory : IDesignTimeDbContextFactory<CodeFakeDataContex>
    {
        public CodeFakeDataContex CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<CodeFakeDataContex>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new CodeFakeDataContex(options);
        }
    }
}
