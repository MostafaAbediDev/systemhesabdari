using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixedAssetManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class FixedAssetManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountAssetId",
                table: "FixedAssets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountDepreciationId",
                table: "FixedAssets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountExpenseId",
                table: "FixedAssets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "FixedAssets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "AssetDisposals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountingDocumentId",
                table: "AssetDisposals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BuyerPersonId",
                table: "AssetDisposals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FixedAssetId",
                table: "AssetDisposals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "AssetDisposals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountingDocumentId",
                table: "AssetDepreciations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FixedAssetId",
                table: "AssetDepreciations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            
            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_AccountExpenseId",
                table: "FixedAssets",
                column: "AccountExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_BranchId",
                table: "FixedAssets",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_AccountId",
                table: "AssetDisposals",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_AccountingDocumentId",
                table: "AssetDisposals",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_BuyerPersonId",
                table: "AssetDisposals",
                column: "BuyerPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_FixedAssetId",
                table: "AssetDisposals",
                column: "FixedAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_InvoiceId",
                table: "AssetDisposals",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepreciations_AccountingDocumentId",
                table: "AssetDepreciations",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepreciations_FixedAssetId",
                table: "AssetDepreciations",
                column: "FixedAssetId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDepreciations_AccountingDocuments_AccountingDocumentId",
                table: "AssetDepreciations",
                column: "AccountingDocumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDepreciations_FixedAssets_FixedAssetId",
                table: "AssetDepreciations",
                column: "FixedAssetId",
                principalTable: "FixedAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_AccountingDocuments_AccountingDocumentId",
                table: "AssetDisposals",
                column: "AccountingDocumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_Accounts_AccountId",
                table: "AssetDisposals",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_FixedAssets_FixedAssetId",
                table: "AssetDisposals",
                column: "FixedAssetId",
                principalTable: "FixedAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_Invoices_InvoiceId",
                table: "AssetDisposals",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_Persons_BuyerPersonId",
                table: "AssetDisposals",
                column: "BuyerPersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssets_Accounts_AccountExpenseId",
                table: "FixedAssets",
                column: "AccountExpenseId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssets_Branches_BranchId",
                table: "FixedAssets",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetDepreciations_AccountingDocuments_AccountingDocumentId",
                table: "AssetDepreciations");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDepreciations_FixedAssets_FixedAssetId",
                table: "AssetDepreciations");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_AccountingDocuments_AccountingDocumentId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_Accounts_AccountId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_FixedAssets_FixedAssetId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_Invoices_InvoiceId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_Persons_BuyerPersonId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssets_Accounts_AccountExpenseId",
                table: "FixedAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssets_Branches_BranchId",
                table: "FixedAssets");

           

            migrationBuilder.DropIndex(
                name: "IX_FixedAssets_AccountExpenseId",
                table: "FixedAssets");

            migrationBuilder.DropIndex(
                name: "IX_FixedAssets_BranchId",
                table: "FixedAssets");

            migrationBuilder.DropIndex(
                name: "IX_AssetDisposals_AccountId",
                table: "AssetDisposals");

            migrationBuilder.DropIndex(
                name: "IX_AssetDisposals_AccountingDocumentId",
                table: "AssetDisposals");

            migrationBuilder.DropIndex(
                name: "IX_AssetDisposals_BuyerPersonId",
                table: "AssetDisposals");

            migrationBuilder.DropIndex(
                name: "IX_AssetDisposals_FixedAssetId",
                table: "AssetDisposals");

            migrationBuilder.DropIndex(
                name: "IX_AssetDisposals_InvoiceId",
                table: "AssetDisposals");

            migrationBuilder.DropIndex(
                name: "IX_AssetDepreciations_AccountingDocumentId",
                table: "AssetDepreciations");

            migrationBuilder.DropIndex(
                name: "IX_AssetDepreciations_FixedAssetId",
                table: "AssetDepreciations");

            migrationBuilder.DropColumn(
                name: "AccountAssetId",
                table: "FixedAssets");

            migrationBuilder.DropColumn(
                name: "AccountDepreciationId",
                table: "FixedAssets");

            migrationBuilder.DropColumn(
                name: "AccountExpenseId",
                table: "FixedAssets");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "FixedAssets");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AssetDisposals");

            migrationBuilder.DropColumn(
                name: "AccountingDocumentId",
                table: "AssetDisposals");

            migrationBuilder.DropColumn(
                name: "BuyerPersonId",
                table: "AssetDisposals");

            migrationBuilder.DropColumn(
                name: "FixedAssetId",
                table: "AssetDisposals");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "AssetDisposals");

            migrationBuilder.DropColumn(
                name: "AccountingDocumentId",
                table: "AssetDepreciations");

            migrationBuilder.DropColumn(
                name: "FixedAssetId",
                table: "AssetDepreciations");
        }
    }
}
