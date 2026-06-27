using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class RemoveBanksIdAndBranchesIdFromCompanyBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBankAccounts_Branches_BranchesId",
                table: "CompanyBankAccounts");

            // 2. حذف Index (اگر وجود داشته باشد)
            migrationBuilder.DropIndex(
                name: "IX_CompanyBankAccounts_BranchesId",
                table: "CompanyBankAccounts");

            // 3. حذف ستون
            migrationBuilder.DropColumn(
                name: "BranchesId",
                table: "CompanyBankAccounts");


            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBankAccounts_Banks_BanksId",
                table: "CompanyBankAccounts");

            // 2. حذف Index (اگر وجود داشته باشد)
            migrationBuilder.DropIndex(
                name: "IX_CompanyBankAccounts_BanksId",
                table: "CompanyBankAccounts");

            // 3. حذف ستون
            migrationBuilder.DropColumn(
                name: "BanksId",
                table: "CompanyBankAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BranchesId",
                table: "CompanyBankAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_BranchesId",
                table: "CompanyBankAccounts",
                column: "BranchesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBankAccounts_Branches_BranchesId",
                table: "CompanyBankAccounts",
                column: "BranchesId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);


            migrationBuilder.AddColumn<long>(
                name: "BanksId",
                table: "CompanyBankAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_BanksId",
                table: "CompanyBankAccounts",
                column: "BanksId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBankAccounts_Banks_BanksId",
                table: "CompanyBankAccounts",
                column: "BanksId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
