using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class AddSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
           
           
            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                    //table.ForeignKey(
                    //    name: "FK_Funds_Accounts_AccountId",
                    //    column: x => x.AccountId,
                    //    principalTable: "Accounts",
                    //    principalColumn: "Id");
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
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    InitialAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LastSettlementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ResponsiblePersonId = table.Column<long>(type: "bigint", nullable: false),
                    HolderPersonId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    SettlementAccountId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PettyCashes", x => x.Id);
                    //table.ForeignKey(
                    //    name: "FK_PettyCashes_Accounts_AccountId",
                    //    column: x => x.AccountId,
                    //    principalTable: "Accounts",
                    //    principalColumn: "Id");
                    //table.ForeignKey(
                    //    name: "FK_PettyCashes_Accounts_SettlementAccountId",
                    //    column: x => x.SettlementAccountId,
                    //    principalTable: "Accounts",
                    //    principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PettyCashes_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PettyCashes_Persons_HolderPersonId",
                        column: x => x.HolderPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PettyCashes_Persons_ResponsiblePersonId",
                        column: x => x.ResponsiblePersonId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

           
            migrationBuilder.CreateTable(
                name: "ReceiptsPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    FinancialPeriodId = table.Column<long>(type: "bigint", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    FundId = table.Column<long>(type: "bigint", nullable: true),
                    CompanyBankAccountId = table.Column<long>(type: "bigint", nullable: true),
                    ChequeId = table.Column<long>(type: "bigint", nullable: true),
                    AccountingDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptsPayments", x => x.Id);
                    //table.ForeignKey(
                    //    name: "FK_ReceiptsPayments_AccountingDocuments_AccountingDocumentId",
                    //    column: x => x.AccountingDocumentId,
                    //    principalTable: "AccountingDocuments",
                    //    principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Cheques_ChequeId",
                        column: x => x.ChequeId,
                        principalTable: "Cheques",
                        principalColumn: "Id");
                    //table.ForeignKey(
                    //    name: "FK_ReceiptsPayments_CompanyBankAccounts_CompanyBankAccountId",
                    //    column: x => x.CompanyBankAccountId,
                    //    principalTable: "CompanyBankAccounts",
                    //    principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_FinancialPeriods_FinancialPeriodId",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptsPayments_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

          
          
            migrationBuilder.CreateIndex(
                name: "IX_Funds_AccountId",
                table: "Funds",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_BranchId",
                table: "Funds",
                column: "BranchId");

         
            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_AccountId",
                table: "PettyCashes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_BranchId",
                table: "PettyCashes",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_HolderPersonId",
                table: "PettyCashes",
                column: "HolderPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_ResponsiblePersonId",
                table: "PettyCashes",
                column: "ResponsiblePersonId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_PersonId",
                table: "ReceiptsPayments",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropTable(
                name: "PettyCashes");

            migrationBuilder.DropTable(
                name: "ReceiptsPayments");

        
            migrationBuilder.DropTable(
                name: "Funds");

          
        }
    }
}
