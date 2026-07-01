using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateDepartmentIdInJobTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        UPDATE JT
        SET JT.DepartmentId = D.Id
        FROM JobTitles JT
        CROSS APPLY
        (
            SELECT TOP 1 Id
            FROM Departments
            ORDER BY NEWID()
        ) D;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
