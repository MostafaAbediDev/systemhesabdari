using InventoryManagement.Domain.Inventory.CategoryAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.EFCore.Mapping
{
    public class CategoryMapping : IEntityTypeConfiguration<Categories>
    {
        public void Configure(EntityTypeBuilder<Categories> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

            builder.HasOne(x => x.Companies).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId);

            builder.HasMany(s => s.Children).WithOne(s => s.Parent).HasForeignKey(s => s.ParentId);
            builder.HasMany(s => s.Products).WithOne(s => s.Categories).HasForeignKey(s => s.CategoryId);

        }
    }
}
