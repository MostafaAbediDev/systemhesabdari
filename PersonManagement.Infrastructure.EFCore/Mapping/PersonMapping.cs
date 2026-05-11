using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonAgg;
using System.Reflection.Emit;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonMapping : IEntityTypeConfiguration<Persons>
    {
        public void Configure(EntityTypeBuilder<Persons> builder)
        {
            builder.ToTable("Persons");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.NationalCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.EconomicCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.RegistrationNumber).HasMaxLength(50).IsRequired();

            builder.HasOne(x => x.Branches).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.PersonType).WithMany(x => x.Persons).HasForeignKey(x => x.PersonTypeId);

            builder.HasMany(s => s.PersonBanks).WithOne(s => s.Persons).HasForeignKey(s => s.PersonId);
            builder.HasMany(s => s.PersonAddresses).WithOne(s => s.Persons).HasForeignKey(s => s.PersonId);
            builder.HasMany(s => s.PersonContacts).WithOne(s => s.Persons).HasForeignKey(s => s.PersonId);

        }
    }
}
