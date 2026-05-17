using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class AddLocationInstedOfLat_logProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat_Log",
                table: "Branches");

           

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Branches",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Branches",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Branches");

           

            migrationBuilder.AddColumn<string>(
                name: "Lat_Log",
                table: "Branches",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
