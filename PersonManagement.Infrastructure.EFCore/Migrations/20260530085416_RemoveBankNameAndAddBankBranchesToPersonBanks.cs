using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBankNameAndAddBankBranchesToPersonBanks : Migration
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

            

            migrationBuilder.CreateIndex(
                name: "IX_PersonBanks_BankBranchId",
                table: "PersonBanks",
                column: "BankBranchId");


            migrationBuilder.AddForeignKey(
                name: "FK_PersonBanks_BankBranches_BankBranchId",
                table: "PersonBanks",
                column: "BankBranchId",
                principalTable: "BankBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropForeignKey(
                name: "FK_PersonBanks_BankBranches_BankBranchId",
                table: "PersonBanks");


            migrationBuilder.DropIndex(
                name: "IX_PersonBanks_BankBranchId",
                table: "PersonBanks");

        }
    }
}
