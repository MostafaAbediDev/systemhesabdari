using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class RemoveBankNameToPersonBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BankBranchId",
                table: "PersonBanks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.DropColumn(
                 name: "BankName",
                 table: "PersonBanks");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "BankBranchId",
                table: "PersonBanks");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "PersonBanks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
