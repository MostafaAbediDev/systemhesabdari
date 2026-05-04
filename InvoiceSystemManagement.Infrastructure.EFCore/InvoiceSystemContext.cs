using InvoiceSystemManagement.Domain.Invoice.InvoiceAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoiceItemAgg;
using InvoiceSystemManagement.Domain.Invoice.InvoicePaymentAgg;
using InvoiceSystemManagement.Domain.Invoice.ReceiptPaymentInvoiceAgg;
using InvoiceSystemManagement.Infrastructure.EFCore.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InvoiceSystemManagement.Infrastructure.EFCore
{
    public class InvoiceSystemContext : DbContext
    {
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<InvoiceItems> InvoiceItems { get; set; }
        public DbSet<InvoicePayments> InvoicePayments { get; set; }
        public DbSet<ReceiptPaymentInvoices> ReceiptPaymentInvoices { get; set; }



        public InvoiceSystemContext(DbContextOptions<InvoiceSystemContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = typeof(InvoiceMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
