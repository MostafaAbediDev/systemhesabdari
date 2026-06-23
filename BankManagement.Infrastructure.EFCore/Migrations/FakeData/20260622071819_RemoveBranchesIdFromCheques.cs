using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class RemoveBranchesIdFromCheques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. حذف FK (اگر وجود داشته باشد)
            migrationBuilder.DropForeignKey(
                name: "FK_Cheques_Branches_BranchesId",
                table: "Cheques");

            // 2. حذف Index (اگر وجود داشته باشد)
            migrationBuilder.DropIndex(
                name: "IX_Cheques_BranchesId",
                table: "Cheques");

            // 3. حذف ستون
            migrationBuilder.DropColumn(
                name: "BranchesId",
                table: "Cheques");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BranchesId",
                table: "Cheques",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_BranchesId",
                table: "Cheques",
                column: "BranchesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheques_Branches_BranchesId",
                table: "Cheques",
                column: "BranchesId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
