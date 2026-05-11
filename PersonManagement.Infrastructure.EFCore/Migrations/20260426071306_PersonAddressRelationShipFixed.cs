using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class PersonAddressRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "PersonAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "PersonAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            

            migrationBuilder.CreateIndex(
                name: "IX_PersonAddresses_CityId",
                table: "PersonAddresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAddresses_ProvinceId",
                table: "PersonAddresses",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonAddresses_Cities_CityId",
                table: "PersonAddresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonAddresses_Provinces_ProvinceId",
                table: "PersonAddresses",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonAddresses_Cities_CityId",
                table: "PersonAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonAddresses_Provinces_ProvinceId",
                table: "PersonAddresses");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_PersonAddresses_CityId",
                table: "PersonAddresses");

            migrationBuilder.DropIndex(
                name: "IX_PersonAddresses_ProvinceId",
                table: "PersonAddresses");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "PersonAddresses");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "PersonAddresses");
        }
    }
}
