using BankManagement.Domain.Bank.BankBrancheAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankManagement.Infrastructure.EFCore.Mapping
{
    public class BankBrancheMapping : IEntityTypeConfiguration<BankBranches>
    {
        public void Configure(EntityTypeBuilder<BankBranches> builder)
        {
            builder.ToTable("BankBranches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BranchName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BranchCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.Telephone)
                .HasMaxLength(50);


            builder.HasOne(x => x.Banks).WithMany(x => x.BankBranches).HasForeignKey(x => x.BankId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Cities).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Provinces).WithMany().HasForeignKey(x => x.ProvinceId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.BankId, x.BranchCode })
                .IsUnique();

        }
    }
}
