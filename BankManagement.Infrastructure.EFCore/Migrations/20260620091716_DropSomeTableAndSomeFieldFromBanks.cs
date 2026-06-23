using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class DropSomeTableAndSomeFieldFromBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBankAccounts_Accounts_AccountId",
                table: "CompanyBankAccounts");

           
            migrationBuilder.DropTable(
                name: "PettyCashes");

            migrationBuilder.DropForeignKey(
                 name: "FK_InvoicePayments_ReceiptsPayments_ReceiptPaymentId",
                 table: "InvoicePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptPaymentInvoices_ReceiptsPayments_ReceiptPaymentId",
                table: "ReceiptPaymentInvoices");

            migrationBuilder.DropTable(
                name: "ReceiptsPayments");

            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DropIndex(
                name: "IX_CompanyBankAccounts_AccountId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "CompanyBankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "LastChequeCode",
                table: "ChequeBooks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FirstChequeCode",
                table: "ChequeBooks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "CompanyBankAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CompanyBankAccounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "CompanyBankAccounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "CompanyBankAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "LastChequeCode",
                table: "ChequeBooks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "FirstChequeCode",
                table: "ChequeBooks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            

            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funds_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Funds_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PettyCashes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    SettlementAccountId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    HolderPersonId = table.Column<long>(type: "bigint", nullable: false),
                    InitialAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastSettlementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ResponsiblePersonId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PettyCashes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PettyCashes_Accounts_SettlementAccountId",
                        column: x => x.SettlementAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PettyCashes_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptsPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountingDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ChequeId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyBankAccountId = table.Column<long>(type: "bigint", nullable: false),
                    FinancialPeriodId = table.Column<long>(type: "bigint", nullable: false),
                    FundId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptsPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_AccountingDocuments_AccountingDocumentId",
                        column: x => x.AccountingDocumentId,
                        principalTable: "AccountingDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Cheques_ChequeId",
                        column: x => x.ChequeId,
                        principalTable: "Cheques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_CompanyBankAccounts_CompanyBankAccountId",
                        column: x => x.CompanyBankAccountId,
                        principalTable: "CompanyBankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_FinancialPeriods_FinancialPeriodId",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_AccountId",
                table: "CompanyBankAccounts",
                column: "AccountId");

         

            migrationBuilder.CreateIndex(
                name: "IX_Funds_AccountId",
                table: "Funds",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_BranchId",
                table: "Funds",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_BranchId",
                table: "PettyCashes",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_SettlementAccountId",
                table: "PettyCashes",
                column: "SettlementAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_AccountingDocumentId",
                table: "ReceiptsPayments",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_BranchId",
                table: "ReceiptsPayments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_ChequeId",
                table: "ReceiptsPayments",
                column: "ChequeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_CompanyBankAccountId",
                table: "ReceiptsPayments",
                column: "CompanyBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_FinancialPeriodId",
                table: "ReceiptsPayments",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_FundId",
                table: "ReceiptsPayments",
                column: "FundId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBankAccounts_Accounts_AccountId",
                table: "CompanyBankAccounts",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
