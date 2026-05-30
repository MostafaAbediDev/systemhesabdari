using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreatePersonFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
;WITH N AS (
    SELECT TOP (500) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_objects
),
B AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS brn
    FROM Branches
)
INSERT INTO Persons
(
    FullName,
    NationalCode,
    EconomicCode,
    RegistrationNumber,
    PersonTypeId,
    IsLegal,
    CreditLimit,
    AvailableCredit,
    BranchId,
    CreationDate,
    IsDeleted,
    IsActive
)
SELECT
    N'شخص ' + CAST(n.rn AS NVARCHAR(10)),

    CASE WHEN n.rn % 2 = 0
        THEN NULL
        ELSE RIGHT('0000000000' + CAST(1000000000 + n.rn AS VARCHAR(10)), 10)
    END,

    CASE WHEN n.rn % 2 = 0
        THEN RIGHT('000000000000' + CAST(100000000000 + n.rn AS VARCHAR(12)), 12)
        ELSE NULL
    END,

    CASE WHEN n.rn % 2 = 0
        THEN CAST(20000 + n.rn AS NVARCHAR(20))
        ELSE NULL
    END,

    ((n.rn - 1) % 3) + 1,
    CASE WHEN n.rn % 2 = 0 THEN 1 ELSE 0 END,

    CAST(10000000 + (n.rn * 1000) AS DECIMAL(18,2)),
    CAST(10000000 + (n.rn * 1000) AS DECIMAL(18,2)),

    b.Id,

    GETDATE(),
    0,
    1
FROM N
JOIN B ON B.brn = ((n.rn - 1) % (SELECT COUNT(*) FROM Branches)) + 1;

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM Persons
WHERE FullName LIKE N'شخص %'
");
        }
    }
}
