using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeeIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) حذف FK اشتباه (اگر وجود داشته باشد)
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeesId",
                table: "Payrolls");

            // 2) حذف ایندکس اشتباه
            migrationBuilder.DropIndex(
                name: "IX_Payrolls_EmployeesId",
                table: "Payrolls");

            // 3) حذف ستون اشتباه
            migrationBuilder.DropColumn(
                name: "EmployeesId",
                table: "Payrolls");

            // --- این بخش همان است که EF ساخته ---
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");

            // برگرداندن ستون اشتباه
            migrationBuilder.AddColumn<long>(
                name: "EmployeesId",
                table: "Payrolls",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeesId",
                table: "Payrolls",
                column: "EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeesId",
                table: "Payrolls",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

    }
}
