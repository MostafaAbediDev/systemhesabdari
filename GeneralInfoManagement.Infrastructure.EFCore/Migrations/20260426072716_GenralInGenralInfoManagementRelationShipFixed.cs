using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class GenralInGenralInfoManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "Cities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provinces_ProvinceId",
                table: "Cities",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provinces_ProvinceId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Cities");
        }
    }
}
