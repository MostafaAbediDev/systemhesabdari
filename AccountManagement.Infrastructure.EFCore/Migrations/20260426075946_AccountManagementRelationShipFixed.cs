using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AccountManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Accounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "AccountLinks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "AccountLinks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "AccountingEntries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountingDocumentId",
                table: "AccountingEntries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "AccountingEntries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "AccountingDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FinancialPeriodId",
                table: "AccountingDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLinks_AccountId",
                table: "AccountLinks",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLinks_CompanyId",
                table: "AccountLinks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_AccountId",
                table: "AccountingEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_AccountingDocumentId",
                table: "AccountingEntries",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_PersonId",
                table: "AccountingEntries",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_BranchId",
                table: "AccountingDocuments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_CreatedBy",
                table: "AccountingDocuments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_FinancialPeriodId",
                table: "AccountingDocuments",
                column: "FinancialPeriodId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingDocuments_Branches_BranchId",
                table: "AccountingDocuments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingDocuments_FinancialPeriods_FinancialPeriodId",
                table: "AccountingDocuments",
                column: "FinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingDocuments_Persons_CreatedBy",
                table: "AccountingDocuments",
                column: "CreatedBy",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingEntries_AccountingDocuments_AccountingDocumentId",
                table: "AccountingEntries",
                column: "AccountingDocumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);


            migrationBuilder.AddForeignKey(
                name: "FK_AccountingEntries_Accounts_AccountId",
                table: "AccountingEntries",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingEntries_Persons_PersonId",
                table: "AccountingEntries",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLinks_Accounts_AccountId",
                table: "AccountLinks",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLinks_Companies_CompanyId",
                table: "AccountLinks",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Companies_CompanyId",
                table: "Accounts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingDocuments_Branches_BranchId",
                table: "AccountingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingDocuments_FinancialPeriods_FinancialPeriodId",
                table: "AccountingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingDocuments_Persons_CreatedBy",
                table: "AccountingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingEntries_AccountingDocuments_AccountingDocumentId",
                table: "AccountingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingEntries_Accounts_AccountId",
                table: "AccountingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingEntries_Persons_PersonId",
                table: "AccountingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountLinks_Accounts_AccountId",
                table: "AccountLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountLinks_Companies_CompanyId",
                table: "AccountLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Companies_CompanyId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "BranchArchive");

            migrationBuilder.DropTable(
                name: "FinancialPeriods");

            migrationBuilder.DropTable(
                name: "PersonAddresses");

            migrationBuilder.DropTable(
                name: "PersonBanks");

            migrationBuilder.DropTable(
                name: "PersonContacts");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ContactTypes");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountLinks_AccountId",
                table: "AccountLinks");

            migrationBuilder.DropIndex(
                name: "IX_AccountLinks_CompanyId",
                table: "AccountLinks");

            migrationBuilder.DropIndex(
                name: "IX_AccountingEntries_AccountId",
                table: "AccountingEntries");

            migrationBuilder.DropIndex(
                name: "IX_AccountingEntries_AccountingDocumentId",
                table: "AccountingEntries");


            migrationBuilder.DropIndex(
                name: "IX_AccountingEntries_PersonId",
                table: "AccountingEntries");

            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_BranchId",
                table: "AccountingDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_CreatedBy",
                table: "AccountingDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_FinancialPeriodId",
                table: "AccountingDocuments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AccountLinks");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AccountLinks");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AccountingEntries");

            migrationBuilder.DropColumn(
                name: "AccountingDocumentId",
                table: "AccountingEntries");


            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "AccountingEntries");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AccountingDocuments");

            migrationBuilder.DropColumn(
                name: "FinancialPeriodId",
                table: "AccountingDocuments");
        }
    }
}
