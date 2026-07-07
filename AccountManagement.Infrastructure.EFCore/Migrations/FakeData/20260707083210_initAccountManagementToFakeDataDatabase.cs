using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class initAccountManagementToFakeDataDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    Nature = table.Column<int>(type: "int", nullable: false),
                    IsPersonRelated = table.Column<bool>(type: "bit", nullable: false),
                    IsProductRelated = table.Column<bool>(type: "bit", nullable: false),
                    IsBankRelated = table.Column<bool>(type: "bit", nullable: false),
                    IsFundRelated = table.Column<bool>(type: "bit", nullable: false),
                    IsEmployeeRelated = table.Column<bool>(type: "bit", nullable: false),
                    AllowManualEntry = table.Column<bool>(type: "bit", nullable: false),
                    IsControlAccount = table.Column<bool>(type: "bit", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Accounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            

            migrationBuilder.CreateTable(
                name: "AccountLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkType = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountLinks_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountLinks_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            
            migrationBuilder.CreateTable(
                name: "AccountingDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<long>(type: "bigint", nullable: false),
                    DocumentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    FinancialPeriodId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingDocuments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingDocuments_FinancialPeriods_FinancialPeriodId",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingDocuments_Persons_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingDocuments_Persons_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

           
            migrationBuilder.CreateTable(
                name: "AccountingEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    AccountingDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    AccountingDocumentsId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingEntries_AccountingDocuments_AccountingDocumentId",
                        column: x => x.AccountingDocumentId,
                        principalTable: "AccountingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountingEntries_AccountingDocuments_AccountingDocumentsId",
                        column: x => x.AccountingDocumentsId,
                        principalTable: "AccountingDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountingEntries_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_ApprovedBy",
                table: "AccountingDocuments",
                column: "ApprovedBy");

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

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_AccountId",
                table: "AccountingEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_AccountingDocumentId",
                table: "AccountingEntries",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_AccountingDocumentsId",
                table: "AccountingEntries",
                column: "AccountingDocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLinks_AccountId",
                table: "AccountLinks",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLinks_CompanyId",
                table: "AccountLinks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentId",
                table: "Accounts",
                column: "ParentId");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingEntries");

            migrationBuilder.DropTable(
                name: "AccountLinks");

            migrationBuilder.DropTable(
                name: "AccountingDocuments");

            migrationBuilder.DropTable(
                name: "Accounts");

            
        }
    }
}
