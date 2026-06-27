using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakeCompanyBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @Total INT = 500;

;WITH BanksCTE AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY Id) AS RN
    FROM Banks
    WHERE ISNULL(IsDeleted,0)=0
),
BranchesCTE AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY Id) AS RN
    FROM Branches
    WHERE ISNULL(IsDeleted,0)=0
),
Numbers AS
(
    SELECT TOP (@Total)
           ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS N
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
),
Data AS
(
    SELECT
        n.N,
        b.Id AS BankId,
        br.Id AS BranchId
    FROM Numbers n
    CROSS APPLY
    (
        SELECT TOP 1 Id
        FROM BanksCTE
        ORDER BY NEWID()
    ) b
    CROSS APPLY
    (
        SELECT TOP 1 Id
        FROM BranchesCTE
        ORDER BY NEWID()
    ) br
)
INSERT INTO CompanyBankAccounts
(
    AccountTitle,
    AccountNumber,
    CardNumber,
    Shaba,
    BranchId,
    BankId,
    IsDeleted,
    IsActive,
    DeletedAt,
    DeletedBy,
    CreationDate
)
SELECT
    N'حساب تستی ' + CAST(d.N AS NVARCHAR(10)),

    RIGHT('000000000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000000000 AS NVARCHAR(20)), 12),

    RIGHT('0000000000000000' + CAST(ABS(CHECKSUM(NEWID())) % 10000000000000000 AS NVARCHAR(20)), 16),

    N'IR' + RIGHT('000000000000000000000000' +
    CAST(ABS(CHECKSUM(NEWID())) % 100000000000000000000000 AS NVARCHAR(30)), 22),

    d.BranchId,
    d.BankId,
    0,
    1,
    NULL,
    NULL,
    GETDATE()
FROM Data d;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM CompanyBankAccounts
WHERE AccountTitle LIKE N'حساب تستی %';
");
        }
    }
}
