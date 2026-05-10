using BankManagement.Domain.Bank.BankTypeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class BankTypeMapping : IEntityTypeConfiguration<BankTypes>
    {
        public void Configure(EntityTypeBuilder<BankTypes> builder)
        {
            builder.ToTable("BankTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreationDate)
                .HasColumnType("datetime2");

            builder.HasMany(x => x.Banks)
                .WithOne(x => x.BankTypes)
                .HasForeignKey(x => x.BankTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Seeding Data ---
            var fixedDate = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                new { Id = 1L, Code = 1, Title = "دولتی", IsActive = true, IsDeleted = false, CreationDate = fixedDate },
                new { Id = 2L, Code = 2, Title = "خصوصی", IsActive = true, IsDeleted = false, CreationDate = fixedDate },
                new { Id = 3L, Code = 3, Title = "قرض‌الحسنه", IsActive = true, IsDeleted = false, CreationDate = fixedDate }
            );
        }
    }
}
