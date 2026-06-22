using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class RemoveBanksIdFromChequeBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. حذف ایندکس وابسته به ستون
            migrationBuilder.DropIndex(
                name: "IX_ChequeBooks_CompanyBankAccountId",
                table: "ChequeBooks");

            // 2. حذف ستون
            migrationBuilder.DropColumn(
                name: "BanksId",
                table: "ChequeBooks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BanksId",
                table: "ChequeBooks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ChequeBooks_CompanyBankAccountId",
                table: "ChequeBooks",
                column: "CompanyBankAccountId");
        }
    }
}
