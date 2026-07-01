using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedJobTitleFakeData : Migration
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
        ),
        DeptCount AS
        (
            SELECT COUNT(*) AS Cnt FROM Departments
        )
        INSERT INTO JobTitles
        (
            Title,
            Description,
            DepartmentId,
            IsActive,
            IsDeleted,
            CreationDate
        )
        SELECT
            CONCAT(N'سمت شغلی ', N),
            CONCAT(N'توضیحات فیک برای شغل شماره ', N),
            D.Id,
            1,
            0,
            DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE())
        FROM Numbers
        CROSS JOIN DeptCount DC
        CROSS APPLY
        (
            SELECT TOP 1 Id
            FROM Departments
            ORDER BY CHECKSUM(NEWID())
        ) D;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM JobTitles
        WHERE Title LIKE N'سمت شغلی %'
          AND Description LIKE N'توضیحات فیک برای شغل شماره %';
    ");
        }
    }
}
