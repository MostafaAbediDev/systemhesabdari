using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class BankBranchesAddedAndFixBankProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_Pictures_PictureId",
                table: "Banks");


            migrationBuilder.DropIndex(
                name: "IX_Banks_PictureId",
                table: "Banks");


            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Banks");


            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BankBranches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    ProvinceId = table.Column<long>(type: "bigint", nullable: false),
                    CityId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankBranches_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankBranches_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankBranches_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

           

            migrationBuilder.CreateIndex(
                name: "IX_BankBranches_BankId_BranchCode",
                table: "BankBranches",
                columns: new[] { "BankId", "BranchCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankBranches_CityId",
                table: "BankBranches",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_BankBranches_ProvinceId",
                table: "BankBranches",
                column: "ProvinceId");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "BankBranches");

           
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Banks");

           

            migrationBuilder.AddColumn<long>(
                name: "PictureId",
                table: "Banks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);


            migrationBuilder.CreateIndex(
                name: "IX_Banks_PictureId",
                table: "Banks",
                column: "PictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_Pictures_PictureId",
                table: "Banks",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }
    }
}
