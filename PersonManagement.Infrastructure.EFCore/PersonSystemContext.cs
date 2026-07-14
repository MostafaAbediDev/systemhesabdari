using BankManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Domain.Person.ContactTypeAgg;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonCategoryAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
using PersonManagement.Domain.Person.PersonTypeAgg;
using PersonManagement.Infrastructure.EFCore.Mapping;

namespace PersonManagement.Infrastructure.EFCore
{
    public class PersonSystemContext : DbContext
    {
        public DbSet<Persons> Persons { get; set; }
        public DbSet<ContactTypes> ContactTypes { get; set; }
        public DbSet<PersonAddresses> PersonAddresses { get; set; }
        public DbSet<PersonBanks> PersonBanks { get; set; }
        public DbSet<PersonContacts> PersonContacts { get; set; }
        public DbSet<PersonType> PersonTypes { get; set; }
        public DbSet<PersonCategory> PersonCategories { get; set; }



        public PersonSystemContext(DbContextOptions<PersonSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<GeneralInfoManagement.Domain.BaseInfo.BranchesAgg.Location>();


            var bankAssembly = typeof(BankBrancheMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(bankAssembly);

            var assembly = typeof(PersonMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
