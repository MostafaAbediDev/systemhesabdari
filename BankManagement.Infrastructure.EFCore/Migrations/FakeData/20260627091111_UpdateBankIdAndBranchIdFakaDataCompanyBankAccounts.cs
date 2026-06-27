using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateBankIdAndBranchIdFakaDataCompanyBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @Total INT = 500;

;WITH BanksCTE AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY NEWID()) AS RN
    FROM Banks
    WHERE ISNULL(IsDeleted,0)=0
),
BranchesCTE AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY NEWID()) AS RN
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
Counts AS
(
    SELECT 
        (SELECT COUNT(*) FROM BanksCTE) AS BankCount,
        (SELECT COUNT(*) FROM BranchesCTE) AS BranchCount
),
Data AS
(
    SELECT
        n.N,
        ((n.N - 1) % c.BankCount) + 1 AS BankRN,
        ((n.N - 1) % c.BranchCount) + 1 AS BranchRN
    FROM Numbers n
    CROSS JOIN Counts c
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

    (SELECT Id FROM BranchesCTE WHERE RN = d.BranchRN),
    (SELECT Id FROM BanksCTE WHERE RN = d.BankRN),

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
