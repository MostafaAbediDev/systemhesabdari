using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class PersonRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Persons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContactTypeId",
                table: "PersonContacts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "PersonContacts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "PersonBanks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "PersonAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            

            migrationBuilder.CreateIndex(
                name: "IX_Persons_BranchId",
                table: "Persons",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonContacts_ContactTypeId",
                table: "PersonContacts",
                column: "ContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonContacts_PersonId",
                table: "PersonContacts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonBanks_PersonId",
                table: "PersonBanks",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAddresses_PersonId",
                table: "PersonAddresses",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonAddresses_Persons_PersonId",
                table: "PersonAddresses",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonBanks_Persons_PersonId",
                table: "PersonBanks",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonContacts_ContactTypes_ContactTypeId",
                table: "PersonContacts",
                column: "ContactTypeId",
                principalTable: "ContactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonContacts_Persons_PersonId",
                table: "PersonContacts",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Branches_BranchId",
                table: "Persons",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonAddresses_Persons_PersonId",
                table: "PersonAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonBanks_Persons_PersonId",
                table: "PersonBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonContacts_ContactTypes_ContactTypeId",
                table: "PersonContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonContacts_Persons_PersonId",
                table: "PersonContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Branches_BranchId",
                table: "Persons");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Persons_BranchId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_PersonContacts_ContactTypeId",
                table: "PersonContacts");

            migrationBuilder.DropIndex(
                name: "IX_PersonContacts_PersonId",
                table: "PersonContacts");

            migrationBuilder.DropIndex(
                name: "IX_PersonBanks_PersonId",
                table: "PersonBanks");

            migrationBuilder.DropIndex(
                name: "IX_PersonAddresses_PersonId",
                table: "PersonAddresses");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ContactTypeId",
                table: "PersonContacts");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "PersonContacts");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "PersonBanks");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "PersonAddresses");
        }
    }
}
