namespace PersonManagement.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Infrastructure.EFCore;

public class PersonManagementBoostrapper
{
    public static void Configure(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PersonSystemContext>(x => x.UseSqlServer(connectionString));
        services.AddDbContext<PersonFakeDataContext>(x => x.UseSqlServer(connectionString));
    }
}

