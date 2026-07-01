using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedDepartmentFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ;WITH Numbers AS
        (
            SELECT TOP (500)
                ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
            FROM sys.all_objects
        )
        INSERT INTO Departments
        (
            Name,
            Description,
            IsActive,
            IsDeleted,
            CreationDate
        )
        SELECT
            CONCAT(N'واحد ', N),
            CONCAT(N'توضیحات تستی برای واحد ', N),
            1,
            0,
            DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE())
        FROM Numbers;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM Departments
        WHERE Name LIKE N'واحد %'
          AND Description LIKE N'توضیحات تستی برای واحد %';
    ");
        }
    }
}
