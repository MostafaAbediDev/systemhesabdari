using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonContactAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonContactMapping : IEntityTypeConfiguration<PersonContacts>
    {
        public void Configure(EntityTypeBuilder<PersonContacts> builder)
        {
            builder.ToTable("PersonContacts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Persons).WithMany(x => x.PersonContacts).HasForeignKey(x => x.PersonId);
            builder.HasOne(x => x.ContactTypes).WithMany(x => x.PersonContacts).HasForeignKey(x => x.ContactTypeId);
        }
    }
}
