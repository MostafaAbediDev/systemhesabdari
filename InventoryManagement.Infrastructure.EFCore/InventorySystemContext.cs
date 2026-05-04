using InventoryManagement.Domain.Inventory.BarcodeAgg;
using InventoryManagement.Domain.Inventory.BrandAgg;
using InventoryManagement.Domain.Inventory.CategoryAgg;
using InventoryManagement.Domain.Inventory.FailureTypeAgg;
using InventoryManagement.Domain.Inventory.InventoryTransactionAgg;
using InventoryManagement.Domain.Inventory.Product.PriceTypeAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAgg;
using InventoryManagement.Domain.Inventory.Product.ProductArrributeValueAgg;
using InventoryManagement.Domain.Inventory.Product.ProductAttributeAgg;
using InventoryManagement.Domain.Inventory.Product.ProductBatcheAgg;
using InventoryManagement.Domain.Inventory.Product.ProductCreateSerieAgg;
using InventoryManagement.Domain.Inventory.Product.ProductPriceAgg;
using InventoryManagement.Domain.Inventory.Product.ProductSerialAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageLocationAgg;
using InventoryManagement.Domain.Inventory.Storage.StorageProductAgg;
using InventoryManagement.Domain.Inventory.TaxTypeAgg;
using InventoryManagement.Domain.Inventory.UnitAgg;
using InventoryManagement.Infrastructure.EFCore.Mapping.Product;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InventoryManagement.Infrastructure.EFCore
{
    public class InventorySystemContext : DbContext
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<PriceTypes> PriceTypes { get; set; }
        public DbSet<ProductAttributeValues> ProductAttributeValues { get; set; }
        public DbSet<ProductAttributes> ProductAttributes { get; set; }
        public DbSet<ProductBatches> ProductBatches { get; set; }
        public DbSet<ProductCreateSeries> ProductCreateSeries { get; set; }
        public DbSet<ProductPrices> ProductPrices { get; set; }
        public DbSet<ProductSerials> ProductSerials { get; set; }
        public DbSet<Storages> Storages { get; set; }
        public DbSet<StorageLocations> StorageLocations { get; set; }
        public DbSet<StorageProducts> StorageProducts { get; set; }
        public DbSet<Units> Units { get; set; }
        public DbSet<TaxTypes> TaxTypes { get; set; }
        public DbSet<InventoryTransactions> InventoryTransactions { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Brands> Brands { get; set; }
        public DbSet<FailureTypes> FailureTypes { get; set; }
        public DbSet<Barcodes> Barcodes { get; set; }
        


        public InventorySystemContext(DbContextOptions<InventorySystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(ProductMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
