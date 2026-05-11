using Microsoft.EntityFrameworkCore;
using PersonManagement.Domain.Person.ContactTypeAgg;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
<<<<<<< HEAD
using PersonManagement.Infrastructure.EFCore.Mapping;
using System.Collections.Generic;
=======
using PersonManagement.Domain.Person.PersonTypeAgg;
using PersonManagement.Infrastructure.EFCore.Mapping;
>>>>>>> master

namespace PersonManagement.Infrastructure.EFCore
{
    public class PersonSystemContext : DbContext
    {
        public DbSet<Persons> Persons { get; set; }
        public DbSet<ContactTypes> ContactTypes { get; set; }
        public DbSet<PersonAddresses> PersonAddresses { get; set; }
        public DbSet<PersonBanks> PersonBanks { get; set; }
        public DbSet<PersonContacts> PersonContacts { get; set; }
<<<<<<< HEAD
=======
        public DbSet<PersonType> PersonTypes { get; set; }
>>>>>>> master


        public PersonSystemContext(DbContextOptions<PersonSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(PersonMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
