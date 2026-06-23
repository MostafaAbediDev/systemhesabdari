using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSomeFiledFromCheques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChequeBooks_Banks_BankId",
                table: "ChequeBooks");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "ChequeBooks");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "ChequeBooks",
                newName: "CompanyBankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_ChequeBooks_BankId",
                table: "ChequeBooks",
                newName: "IX_ChequeBooks_CompanyBankAccountId");


            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveDate",
                table: "ChequeBooks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "ChequeBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChequeBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ChequeBooks_CompanyBankAccounts_CompanyBankAccountId",
                table: "ChequeBooks",
                column: "CompanyBankAccountId",
                principalTable: "CompanyBankAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChequeBooks_CompanyBankAccounts_CompanyBankAccountId",
                table: "ChequeBooks");

            migrationBuilder.DropColumn(
                name: "ReceiveDate",
                table: "ChequeBooks");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "ChequeBooks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChequeBooks");

            migrationBuilder.RenameColumn(
                name: "CompanyBankAccountId",
                table: "ChequeBooks",
                newName: "BankId");

            migrationBuilder.RenameIndex(
                name: "IX_ChequeBooks_CompanyBankAccountId",
                table: "ChequeBooks",
                newName: "IX_ChequeBooks_BankId");

            
            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "ChequeBooks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ChequeBooks_Banks_BankId",
                table: "ChequeBooks",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");
        }
    }
}
