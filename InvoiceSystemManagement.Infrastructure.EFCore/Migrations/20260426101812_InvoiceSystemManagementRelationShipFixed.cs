using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceSystemManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceSystemManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountingDoucumentId",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FinancialPeriodId",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StorageId",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "InvoicePayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReceiptPaymentId",
                table: "InvoicePayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BatchId",
                table: "InvoiceItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "InvoiceItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "InvoiceItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "InvoiceItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AccountingDoucumentId",
                table: "Invoices",
                column: "AccountingDoucumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BranchId",
                table: "Invoices",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FinancialPeriodId",
                table: "Invoices",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PersonId",
                table: "Invoices",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_StorageId",
                table: "Invoices",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_InvoiceId",
                table: "InvoicePayments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_ReceiptPaymentId",
                table: "InvoicePayments",
                column: "ReceiptPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_BatchId",
                table: "InvoiceItems",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ProductId",
                table: "InvoiceItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_UnitId",
                table: "InvoiceItems",
                column: "UnitId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_ProductBatches_BatchId",
                table: "InvoiceItems",
                column: "BatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Products_ProductId",
                table: "InvoiceItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Units_UnitId",
                table: "InvoiceItems",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicePayments_Invoices_InvoiceId",
                table: "InvoicePayments",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicePayments_ReceiptsPayments_ReceiptPaymentId",
                table: "InvoicePayments",
                column: "ReceiptPaymentId",
                principalTable: "ReceiptsPayments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AccountingDocuments_AccountingDoucumentId",
                table: "Invoices",
                column: "AccountingDoucumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Branches_BranchId",
                table: "Invoices",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialPeriods_FinancialPeriodId",
                table: "Invoices",
                column: "FinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Persons_PersonId",
                table: "Invoices",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Storages_StorageId",
                table: "Invoices",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_ProductBatches_BatchId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Products_ProductId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Units_UnitId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicePayments_Invoices_InvoiceId",
                table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicePayments_ReceiptsPayments_ReceiptPaymentId",
                table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AccountingDocuments_AccountingDoucumentId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Branches_BranchId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialPeriods_FinancialPeriodId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Persons_PersonId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Storages_StorageId",
                table: "Invoices");

            

            migrationBuilder.DropIndex(
                name: "IX_Invoices_AccountingDoucumentId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BranchId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_FinancialPeriodId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PersonId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_StorageId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoicePayments_InvoiceId",
                table: "InvoicePayments");

            migrationBuilder.DropIndex(
                name: "IX_InvoicePayments_ReceiptPaymentId",
                table: "InvoicePayments");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_BatchId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_ProductId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_UnitId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "AccountingDoucumentId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FinancialPeriodId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "InvoicePayments");

            migrationBuilder.DropColumn(
                name: "ReceiptPaymentId",
                table: "InvoicePayments");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "InvoiceItems");
        }
    }
}
