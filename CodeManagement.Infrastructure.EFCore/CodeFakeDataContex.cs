using CodeManagement.Domain.CodeAgg;
using CodeManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace CodeManagement.Infrastructure.EFCore
{
    public class CodeFakeDataContex : DbContext
    {
        public DbSet<Codes> Codes { get; set; }

        public CodeFakeDataContex(DbContextOptions<CodeFakeDataContex> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(CodeMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}