using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonMapping : IEntityTypeConfiguration<Persons>
    {
        public void Configure(EntityTypeBuilder<Persons> builder)
        {
            builder.ToTable("Persons");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ContactFirstName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ContactLastName).HasMaxLength(200).IsRequired();

            builder.Property(x => x.NationalCode).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.EconomicCode).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.RegistrationNumber).HasMaxLength(50).IsRequired(false);

            builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
            builder.Property(x => x.AvailableCredit).HasPrecision(18, 2);

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PersonType).WithMany(x => x.Persons).HasForeignKey(x => x.PersonTypeId);
            builder.HasOne(x => x.PersonCategory).WithMany(x => x.Persons).HasForeignKey(x => x.PersonCategoryId);

            builder.HasMany(s => s.PersonBanks).WithOne(s => s.Persons).HasForeignKey(s => s.PersonId);
            builder.HasMany(s => s.PersonAddresses).WithOne(s => s.Persons).HasForeignKey(s => s.PersonId);
            builder.HasMany(s => s.PersonContacts).WithOne(s => s.Persons).HasForeignKey(s => s.PersonId);

        }
    }
}
