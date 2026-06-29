using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakeCheques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @Total INT = 500;

IF OBJECT_ID('tempdb..#CB') IS NOT NULL DROP TABLE #CB;
IF OBJECT_ID('tempdb..#BR') IS NOT NULL DROP TABLE #BR;

-- ChequeBooks temp
SELECT Id,
       ROW_NUMBER() OVER (ORDER BY NEWID()) AS RN
INTO #CB
FROM ChequeBooks
WHERE ISNULL(IsDeleted,0)=0;

-- Branches temp
SELECT Id,
       ROW_NUMBER() OVER (ORDER BY NEWID()) AS RN
INTO #BR
FROM Branches
WHERE ISNULL(IsDeleted,0)=0;

-- Numbers
;WITH Numbers AS
(
    SELECT TOP (@Total)
           ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS N
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
)

INSERT INTO Cheques
(
    ChequeType,
    ChequeNumber,
    Amount,
    DueDate,
    Status,
    ReferenceType,
    ReferenceId,
    ChequeBookId,
    BranchId,
    IsDeleted,
    IsActive,
    DeletedAt,
    DeletedBy,
    CreationDate
)
SELECT
    ((ABS(CHECKSUM(NEWID())) % 2) + 1),

    'CH-' + RIGHT('0000000000' + CAST(n.N AS NVARCHAR(10)), 10),

    (ABS(CHECKSUM(NEWID())) % 50000000) + 50000,

    DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, GETDATE()),

    ((ABS(CHECKSUM(NEWID())) % 6) + 1),

    (ABS(CHECKSUM(NEWID())) % 5),

    ABS(CHECKSUM(NEWID())) % 10000,

    cb.Id,
    br.Id,

    0,
    1,
    NULL,
    NULL,
    GETDATE()
FROM Numbers n
JOIN #CB cb ON cb.RN = ((n.N - 1) % (SELECT COUNT(*) FROM #CB)) + 1
JOIN #BR br ON br.RN = ((n.N - 1) % (SELECT COUNT(*) FROM #BR)) + 1;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM Cheques
WHERE ChequeNumber LIKE 'CH-%';
");
        }
    }
}
