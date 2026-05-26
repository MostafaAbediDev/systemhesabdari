using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class CitiesAndProvincesAddToBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "Branches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "Branches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CityId",
                table: "Branches",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ProvinceId",
                table: "Branches",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Cities_CityId",
                table: "Branches",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Provinces_ProvinceId",
                table: "Branches",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Cities_CityId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Provinces_ProvinceId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_CityId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_ProvinceId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Branches");
        }
    }
}
