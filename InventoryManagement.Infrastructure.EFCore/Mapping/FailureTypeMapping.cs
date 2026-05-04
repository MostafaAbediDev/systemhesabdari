using InventoryManagement.Domain.Inventory.FailureTypeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class FailureTypeMapping : IEntityTypeConfiguration<FailureTypes>
    {
        public void Configure(EntityTypeBuilder<FailureTypes> builder)
        {
            builder.ToTable("FailureTypes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();

            builder.HasMany(x => x.InventoryTransactions).WithOne(x => x.FailureTypes).HasForeignKey(x => x.FailureTypeId);

        }
    }
}
