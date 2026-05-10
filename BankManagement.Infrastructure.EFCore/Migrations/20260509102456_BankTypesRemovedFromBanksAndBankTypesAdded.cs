using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class BankTypesRemovedFromBanksAndBankTypesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankType",
                table: "Banks");


            migrationBuilder.CreateTable(
                name: "BankTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTypes", x => x.Id);
                });


            migrationBuilder.InsertData(
                table: "BankTypes",
                columns: new[] { "Id", "Code", "CreationDate", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Title" },
                values: new object[,]
                {
                    { 1L, 1, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "دولتی" },
                    { 2L, 2, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "خصوصی" },
                    { 3L, 3, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "قرض‌الحسنه" }
                });

           
            migrationBuilder.CreateIndex(
                name: "IX_Banks_BankTypeId",
                table: "Banks",
                column: "BankTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_BankTypes_BankTypeId",
                table: "Banks",
                column: "BankTypeId",
                principalTable: "BankTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_BankTypes_BankTypeId",
                table: "Banks");

          
            migrationBuilder.DropTable(
                name: "BankTypes");


            migrationBuilder.DropIndex(
                name: "IX_Banks_BankTypeId",
                table: "Banks");

            migrationBuilder.AddColumn<int>(
                name: "BankType",
                table: "Banks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
