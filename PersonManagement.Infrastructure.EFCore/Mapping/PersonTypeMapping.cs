using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonManagement.Domain.Person.PersonTypeAgg;

namespace PersonManagement.Infrastructure.EFCore.Mapping
{
    public class PersonTypeMapping : IEntityTypeConfiguration<PersonType>
    {
        public void Configure(EntityTypeBuilder<PersonType> builder)
        {
            builder.ToTable("PersonTypes");

            builder.HasKey(x => x.Id);

<<<<<<< HEAD
            builder.Property(x => x.TitleId)
=======
            builder.Property(x => x.Code)
>>>>>>> front
                   .IsRequired();

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreationDate)
                   .IsRequired();

            builder.HasData(
                new
                {
                    Id = 1L,
<<<<<<< HEAD
                    TitleId = 1,
=======
                    Code = 1,
>>>>>>> front
                    Title = "مشتری",
                    IsDeleted = false,
                    IsActive = true,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    CreationDate = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 2L,
<<<<<<< HEAD
                    TitleId = 2,
=======
                    Code = 2,
>>>>>>> front
                    Title = "پرسنل",
                    IsDeleted = false,
                    IsActive = true,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    CreationDate = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 3L,
<<<<<<< HEAD
                    TitleId = 3,
=======
                    Code = 3,
>>>>>>> front
                    Title = "تامین کننده",
                    IsDeleted = false,
                    IsActive = true,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    CreationDate = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            builder.HasMany(x => x.Persons)
                   .WithOne(x => x.PersonType)
                   .HasForeignKey(x => x.PersonTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
