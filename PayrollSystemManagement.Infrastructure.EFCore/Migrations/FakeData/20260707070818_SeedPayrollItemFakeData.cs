using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedPayrollItemFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @BranchId BIGINT;
DECLARE @Title NVARCHAR(100);

WHILE @Counter <= 500
BEGIN

    SELECT TOP 1 @BranchId = Id
    FROM Branches
    ORDER BY NEWID();

    SET @Title = N'Payroll Item ' + CAST(@Counter AS NVARCHAR(10));

    INSERT INTO PayrollItems
    (
        Title,
        IsFixed,
        Taxable,
        Insuranceable,
        ItemType,
        RuleType,
        BranchId,
        IsDeleted,
        IsActive,
        DeletedAt,
        DeletedBy,
        CreationDate
    )
    VALUES
    (
        @Title,

        ABS(CHECKSUM(NEWID())) % 2,

        ABS(CHECKSUM(NEWID())) % 2,

        ABS(CHECKSUM(NEWID())) % 2,

        1 + ABS(CHECKSUM(NEWID())) % 2,

        1 + ABS(CHECKSUM(NEWID())) % 3,

        @BranchId,

        0,

        1,

        NULL,

        NULL,

        GETDATE()
    );

    SET @Counter = @Counter + 1;

END

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM PayrollItems
WHERE Title LIKE N'Payroll Item %';
");
        }
    }
}
