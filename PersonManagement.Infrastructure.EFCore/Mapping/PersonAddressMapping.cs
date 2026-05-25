using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonAddressAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonAddressMapping : IEntityTypeConfiguration<PersonAddresses>
    {
        public void Configure(EntityTypeBuilder<PersonAddresses> builder)
        {
            builder.ToTable("PersonAddresses");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
            builder.Property(x => x.PostalCode).HasMaxLength(20).IsRequired();

            builder.HasOne(x => x.Persons).WithMany(x => x.PersonAddresses).HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.Provinces).WithMany().HasForeignKey(x => x.ProvinceId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Cities).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
