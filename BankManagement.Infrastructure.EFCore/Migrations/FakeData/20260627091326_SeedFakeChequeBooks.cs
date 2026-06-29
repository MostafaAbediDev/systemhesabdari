using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakeChequeBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @Total INT = 500;

;WITH AccountsCTE AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY NEWID()) AS RN
    FROM CompanyBankAccounts
    WHERE ISNULL(IsDeleted,0)=0
),
Numbers AS
(
    SELECT TOP (@Total)
           ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS N
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
),
Counts AS
(
    SELECT COUNT(*) AS AccCount
    FROM AccountsCTE
),
Data AS
(
    SELECT
        n.N,
        ((n.N - 1) % c.AccCount) + 1 AS AccRN
    FROM Numbers n
    CROSS JOIN Counts c
)
INSERT INTO ChequeBooks
(
    ChequeCount,
    FirstChequeCode,
    LastChequeCode,
    Status,
    SerialNumber,
    ReceiveDate,
    CompanyBankAccountId,
    IsDeleted,
    IsActive,
    DeletedAt,
    DeletedBy,
    CreationDate
)
SELECT
    -- تعداد چک‌ها
    (ABS(CHECKSUM(NEWID())) % 25) + 10 AS ChequeCount,

    -- شماره شروع چک
    RIGHT('0000000' + CAST(ABS(CHECKSUM(NEWID())) % 10000000 AS NVARCHAR(10)), 7),

    -- شماره پایان چک (وابسته به count)
    RIGHT('0000000' + CAST(
        (ABS(CHECKSUM(NEWID())) % 10000000) +
        ((ABS(CHECKSUM(NEWID())) % 25) + 10)
    AS NVARCHAR(10)), 7),

    -- Status رندوم
    ((ABS(CHECKSUM(NEWID())) % 4) + 1),

    -- سریال یکتا
    'CHQ-' + RIGHT('000000' + CAST(n.N AS NVARCHAR(10)), 6),

    -- تاریخ دریافت
    DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 365), GETDATE()),

    -- ارتباط با حساب بانکی
    (SELECT Id FROM AccountsCTE WHERE RN = d.AccRN),

    0,
    1,
    NULL,
    NULL,
    GETDATE()
FROM Data d
CROSS JOIN Numbers n;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM ChequeBooks
WHERE SerialNumber LIKE 'CHQ-%';
");
        }
    }
}
