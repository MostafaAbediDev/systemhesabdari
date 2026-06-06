namespace PersonManagement.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application;
using PersonManagement.Application.Contract.ContactTypes;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Application.Contract.PersonCategory;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Application.Contract.PersonTypes;
using PersonManagement.Domain.Person.ContactTypeAgg;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonCategoryAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
using PersonManagement.Domain.Person.PersonTypeAgg;
using PersonManagement.Infrastructure.EFCore;
using PersonManagement.Infrastructure.EFCore.Repository;

public class PersonManagementBoostrapper
{
    public static void Configure(IServiceCollection services, string connectionString)
    {

        services.AddTransient<IContactTypeRepository, ContactTypeRepository>();
        services.AddTransient<IContactTypeApplication, ContactTypeApplication>();

        services.AddTransient<IPersonAddressRepository, PersonAddressRepository>();
        services.AddTransient<IPersonAddressApplication, PersonAddressApplication>();

        services.AddTransient<IPersonRepository, PersonRepository>();
        services.AddTransient<IPersonApplication, PersonApplication>();

        services.AddTransient<IPersonBankRepository, PersonBankRepository>();
        services.AddTransient<IPersonBankApplication, PersonBankApplication>();

        services.AddTransient<IPersonCategoryRepository, PersonCategoryRepository>();
        services.AddTransient<IPersonCategoryApplication, PersonCategoryApplication>();


        services.AddTransient<IPersonContactRepository, PersonContactRepository>();
        services.AddTransient<IPersonContactApplication, PersonContactApplication>();


        services.AddTransient<IPersonTypeRepository, PersonTypeRepository>();
        services.AddTransient<IPersonTypeApplication, PersonTypeApplication>();

        services.AddDbContext<PersonSystemContext>(x => x.UseSqlServer(connectionString));
        services.AddDbContext<PersonFakeDataContext>(x => x.UseSqlServer(connectionString));
    }
}

