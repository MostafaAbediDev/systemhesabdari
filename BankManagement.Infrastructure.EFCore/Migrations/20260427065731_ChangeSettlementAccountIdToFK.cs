using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSettlementAccountIdToFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_SettlementAccountId",
                table: "PettyCashes",
                column: "SettlementAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PettyCashes_Accounts_SettlementAccountId",
                table: "PettyCashes",
                column: "SettlementAccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PettyCashes_Accounts_SettlementAccountId",
                table: "PettyCashes");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashes_SettlementAccountId",
                table: "PettyCashes");
        }
    }
}
