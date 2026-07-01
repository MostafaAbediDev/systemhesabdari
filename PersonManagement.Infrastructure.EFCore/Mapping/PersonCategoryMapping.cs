using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonCategoryAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonCategoryMapping : IEntityTypeConfiguration<PersonCategory>
    {
        public void Configure(EntityTypeBuilder<PersonCategory> builder)
        {
            builder.ToTable("PersonCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.PersonTypeId)
                   .IsRequired();

            builder.Property(x => x.ParentId)
                   .IsRequired(false);

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreationDate)
                   .IsRequired();

            builder.Property(x => x.DeletedAt);

            builder.Property(x => x.DeletedBy);

            builder.HasOne(x => x.PersonType)
                   .WithMany()
                   .HasForeignKey(x => x.PersonTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Parent)
                   .WithMany(x => x.Children)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Persons).WithOne(s => s.PersonCategory).HasForeignKey(s => s.PersonCategoryId);

            builder.HasIndex(x => x.PersonTypeId);

            builder.HasIndex(x => x.ParentId);

            builder.HasIndex(x => x.Title);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
