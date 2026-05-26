using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class TelePhoneAddedToBranchesAndRenamePhoneToMobilePhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Branches",
                newName: "MobilePhone");

            migrationBuilder.AddColumn<string>(
                name: "TelePhone",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelePhone",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "MobilePhone",
                table: "Branches",
                newName: "Phone");
        }
    }
}
