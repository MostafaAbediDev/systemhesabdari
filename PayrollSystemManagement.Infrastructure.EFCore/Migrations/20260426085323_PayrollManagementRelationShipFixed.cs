using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class PayrollManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "PayrollItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PayrollId",
                table: "PayrollDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PayrollItemId",
                table: "PayrollDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Employees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "Employees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_AccountingDocumentId",
                table: "Payrolls",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_BranchId",
                table: "Payrolls",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_FinancialPeriodId",
                table: "Payrolls",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_BranchId",
                table: "PayrollItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDetails_PayrollId",
                table: "PayrollDetails",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDetails_PayrollItemId",
                table: "PayrollDetails",
                column: "PayrollItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchId",
                table: "Employees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PersonId",
                table: "Employees",
                column: "PersonId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Branches_BranchId",
                table: "Employees",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Persons_PersonId",
                table: "Employees",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDetails_PayrollItems_PayrollItemId",
                table: "PayrollDetails",
                column: "PayrollItemId",
                principalTable: "PayrollItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDetails_Payrolls_PayrollId",
                table: "PayrollDetails",
                column: "PayrollId",
                principalTable: "Payrolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollItems_Branches_BranchId",
                table: "PayrollItems",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AccountingDocuments_AccountingDocumentId",
                table: "Payrolls",
                column: "AccountingDocumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Branches_BranchId",
                table: "Payrolls",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");


            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_FinancialPeriods_FinancialPeriodId",
                table: "Payrolls",
                column: "FinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branches_BranchId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Persons_PersonId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDetails_PayrollItems_PayrollItemId",
                table: "PayrollDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollDetails_Payrolls_PayrollId",
                table: "PayrollDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollItems_Branches_BranchId",
                table: "PayrollItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_AccountingDocuments_AccountingDocumentId",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Branches_BranchId",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");


            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_FinancialPeriods_FinancialPeriodId",
                table: "Payrolls");

            

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_AccountingDocumentId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_BranchId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_FinancialPeriodId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_PayrollItems_BranchId",
                table: "PayrollItems");

            migrationBuilder.DropIndex(
                name: "IX_PayrollDetails_PayrollId",
                table: "PayrollDetails");

            migrationBuilder.DropIndex(
                name: "IX_PayrollDetails_PayrollItemId",
                table: "PayrollDetails");

            migrationBuilder.DropIndex(
                name: "IX_Employees_BranchId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PersonId",
                table: "Employees");


            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "PayrollItems");

            migrationBuilder.DropColumn(
                name: "PayrollId",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "PayrollItemId",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Employees");
        }
    }
}
