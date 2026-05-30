using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakeBankBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM Banks WHERE ISNULL(IsDeleted,0)=0)
   AND EXISTS (SELECT 1 FROM Provinces WHERE ISNULL(IsDeleted,0)=0)
   AND EXISTS (SELECT 1 FROM Cities WHERE ISNULL(IsDeleted,0)=0)
BEGIN
    DECLARE @BranchesPerBank INT = 5;

    ;WITH B AS
    (
        SELECT Id AS BankId,
               ROW_NUMBER() OVER (ORDER BY Id) AS BRN
        FROM Banks
        WHERE ISNULL(IsDeleted,0)=0
    ),
    P AS
    (
        SELECT Id AS ProvinceId,
               ROW_NUMBER() OVER (ORDER BY Id) AS PRN
        FROM Provinces
        WHERE ISNULL(IsDeleted,0)=0
    ),
    C AS
    (
        SELECT Id AS CityId,
               ProvinceId,
               ROW_NUMBER() OVER (PARTITION BY ProvinceId ORDER BY Id) AS CRN
        FROM Cities
        WHERE ISNULL(IsDeleted,0)=0
    ),
    N AS
    (
        -- تولید 1..@BranchesPerBank
        SELECT TOP (@BranchesPerBank)
               ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS N
        FROM sys.all_objects
    ),
    Generated AS
    (
        SELECT
            b.BankId,
            -- Province چرخشی بین استان‌ها
            p.ProvinceId,
            -- یک City از همان Province (اولین شهر آن استان)
            c.CityId,
            n.N AS BranchNo
        FROM B b
        JOIN P p ON p.PRN = ((b.BRN - 1) % (SELECT COUNT(1) FROM P)) + 1
        JOIN C c ON c.ProvinceId = p.ProvinceId AND c.CRN = 1
        CROSS JOIN N n
    )
    INSERT INTO BankBranches
    (
        Title,
        BranchCode,
        Address,
        Telephone,
        BankId,
        ProvinceId,
        CityId,
        IsDeleted,
        IsActive,
        CreationDate
    )
    SELECT
        N'شعبه ' + CAST(g.BranchNo AS NVARCHAR(10)) + N' - ' + bnk.Title AS Title,
        RIGHT('0000' + CAST(g.BranchNo AS NVARCHAR(10)), 4) 
            + N'-' + RIGHT('000000' + CAST(g.BankId AS NVARCHAR(20)), 6) AS BranchCode,
        N'آدرس فیک - استان ' + CAST(g.ProvinceId AS NVARCHAR(20)) + N' - شهر ' + CAST(g.CityId AS NVARCHAR(20)) AS Address,
        N'021-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(NEWID())) % 10000000 AS NVARCHAR(10)), 7) AS Telephone,
        g.BankId,
        g.ProvinceId,
        g.CityId,
        0 AS IsDeleted,
        1 AS IsActive,
        GETDATE() AS CreationDate
    FROM Generated g
    JOIN Banks bnk ON bnk.Id = g.BankId
    WHERE NOT EXISTS
    (
        SELECT 1 FROM BankBranches bb
        WHERE bb.BankId = g.BankId
          AND bb.BranchCode = RIGHT('0000' + CAST(g.BranchNo AS NVARCHAR(10)), 4) 
                             + N'-' + RIGHT('000000' + CAST(g.BankId AS NVARCHAR(20)), 6)
    );
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // فقط شعبه‌هایی که با الگوی BranchCode این seed ساخته شده پاک می‌کنیم
            migrationBuilder.Sql(@"
DELETE bb
FROM BankBranches bb
WHERE bb.BranchCode LIKE N'____-______';
");
        }
    }
}
