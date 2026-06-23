using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class ChangeSomeBankManagementProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "BankTypes",
                newName: "TitleId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "BankBranches",
                newName: "BranchName");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TitleId",
                table: "BankTypes",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "BranchName",
                table: "BankBranches",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
