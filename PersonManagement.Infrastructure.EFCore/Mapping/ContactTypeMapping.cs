using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.ContactTypeAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class ContactTypeMapping : IEntityTypeConfiguration<ContactTypes>
    {
        public void Configure(EntityTypeBuilder<ContactTypes> builder)
        {
            builder.ToTable("ContactTypes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(50).IsRequired();
            
            builder.HasMany(s => s.PersonContacts).WithOne(s => s.ContactTypes).HasForeignKey(s => s.ContactTypeId);

        }
    }
}
