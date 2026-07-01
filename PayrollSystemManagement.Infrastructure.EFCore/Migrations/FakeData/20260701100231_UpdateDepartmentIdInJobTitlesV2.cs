using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateDepartmentIdInJobTitlesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        UPDATE JT
        SET JT.DepartmentId =
        (
            SELECT TOP 1 D.Id
            FROM Departments D
            ORDER BY CHECKSUM(NEWID(), JT.Id)
        )
        FROM JobTitles JT;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
