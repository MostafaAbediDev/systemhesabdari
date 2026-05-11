using LogManagement.Domain.LogAgg;
using LogManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace LogManagement.Infrastructure.EFCore
{
    public class LogSystemContext : DbContext
    {
        public DbSet<Logs> Logs { get; set; }

        public LogSystemContext(DbContextOptions<LogSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(LogMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
