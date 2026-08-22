using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class IsAutomaticAddedToCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutomatic",
                table: "Codes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutomatic",
                table: "Codes");
        }
    }
}
