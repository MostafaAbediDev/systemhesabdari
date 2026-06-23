using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class AddSomeTableInBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "ChequeBooks",
    columns: table => new
    {
        Id = table.Column<long>(type: "bigint", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        ChequeCount = table.Column<int>(type: "int", nullable: false),
        FirstChequeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
        LastChequeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
        OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
        BankId = table.Column<long>(type: "bigint", nullable: false),
        BanksId = table.Column<long>(type: "bigint", nullable: false),
        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
        IsActive = table.Column<bool>(type: "bit", nullable: false),
        DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
        DeletedBy = table.Column<long>(type: "bigint", nullable: true),
        CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_ChequeBooks", x => x.Id);
        table.ForeignKey(
            name: "FK_ChequeBooks_Banks_BanksId",
            column: x => x.BanksId,
            principalTable: "Banks",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    });



            migrationBuilder.CreateTable(
                name: "Cheques",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChequeType = table.Column<int>(type: "int", nullable: false),
                    ChequeNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: false),
                    ChequeBookId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ChequeBooksId = table.Column<long>(type: "bigint", nullable: false),
                    BranchesId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cheques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cheques_Branches_BranchesId",
                        column: x => x.BranchesId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cheques_ChequeBooks_ChequeBooksId",
                        column: x => x.ChequeBooksId,
                        principalTable: "ChequeBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyBankAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shaba = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    BranchesId = table.Column<long>(type: "bigint", nullable: false),
                    BanksId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyBankAccounts_Banks_BanksId",
                        column: x => x.BanksId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyBankAccounts_Branches_BranchesId",
                        column: x => x.BranchesId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });




            migrationBuilder.CreateIndex(
                name: "IX_ChequeBooks_BanksId",
                table: "ChequeBooks",
                column: "BanksId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_BranchesId",
                table: "Cheques",
                column: "BranchesId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_ChequeBooksId",
                table: "Cheques",
                column: "ChequeBooksId");




            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_BanksId",
                table: "CompanyBankAccounts",
                column: "BanksId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_BranchesId",
                table: "CompanyBankAccounts",
                column: "BranchesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cheques");

            migrationBuilder.DropTable(
                name: "CompanyBankAccounts");

            migrationBuilder.DropTable(
                name: "ChequeBooks");
        }
    }
}
