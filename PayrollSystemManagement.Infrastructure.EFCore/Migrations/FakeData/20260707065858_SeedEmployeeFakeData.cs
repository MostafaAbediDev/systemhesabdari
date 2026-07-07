using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedEmployeeFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @Counter INT = 1;

DECLARE @PersonId BIGINT;
DECLARE @BranchId BIGINT;
DECLARE @DepartmentId BIGINT;
DECLARE @JobTitleId BIGINT;

DECLARE @EmployeeCode NVARCHAR(50);
DECLARE @InsuranceNumber NVARCHAR(50);

WHILE @Counter <= 500
BEGIN

    -- انتخاب یک Person که هنوز Employee ندارد
    SELECT TOP 1 @PersonId = p.Id
    FROM Persons p
    LEFT JOIN Employees e ON e.PersonId = p.Id
    WHERE e.PersonId IS NULL
    ORDER BY NEWID();

    IF @PersonId IS NULL
        BREAK;

    SELECT TOP 1 @BranchId = Id
    FROM Branches
    ORDER BY NEWID();

    SELECT TOP 1 @DepartmentId = Id
    FROM Departments
    ORDER BY NEWID();

    SELECT TOP 1 @JobTitleId = Id
    FROM JobTitles
    ORDER BY NEWID();

    SET @EmployeeCode =
        'EMP' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)),5);

    SET @InsuranceNumber =
        CAST(1000000000 + ABS(CHECKSUM(NEWID())) % 899999999 AS VARCHAR(20));

    INSERT INTO Employees
    (
        EmployeeCode,
        InsuranceNumber,
        Description,
        HireDate,
        TerminationDate,
        BaseSalary,
        BranchId,
        PersonId,
        DepartmentId,
        JobTitleId,
        ContractType,
        IsDeleted,
        IsActive,
        DeletedAt,
        DeletedBy,
        CreationDate
    )
    VALUES
    (
        @EmployeeCode,
        @InsuranceNumber,
        N'Fake Employee ' + CAST(@Counter AS NVARCHAR(10)),
        DATEADD(DAY,-ABS(CHECKSUM(NEWID())) % 1825,GETDATE()),
        NULL,
        (30000000 + ABS(CHECKSUM(NEWID())) % 70000000),
        @BranchId,
        @PersonId,
        @DepartmentId,
        @JobTitleId,
        1 + ABS(CHECKSUM(NEWID())) % 5,
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
DELETE FROM Employees
WHERE EmployeeCode LIKE 'EMP%';
");
        }
    }
}
