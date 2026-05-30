using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakePersonBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM Persons WHERE ISNULL(IsDeleted,0)=0)
   AND EXISTS (SELECT 1 FROM BankBranches WHERE ISNULL(IsDeleted,0)=0)
BEGIN
    DECLARE @AccountsPerPerson INT = 2;

    ;WITH P AS
    (
        SELECT Id AS PersonId,
               ROW_NUMBER() OVER (ORDER BY Id) AS PRN
        FROM Persons
        WHERE ISNULL(IsDeleted,0)=0
    ),
    BB AS
    (
        SELECT Id AS BankBranchId,
               ROW_NUMBER() OVER (ORDER BY Id) AS BRN
        FROM BankBranches
        WHERE ISNULL(IsDeleted,0)=0
    ),
    N AS
    (
        SELECT TOP (@AccountsPerPerson)
               ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS N
        FROM sys.all_objects
    ),
    Gen AS
    (
        SELECT
            p.PersonId,
            n.N AS AccNo,
            bb.BankBranchId
        FROM P p
        CROSS JOIN N n
        JOIN BB bb ON bb.BRN = ((p.PRN + n.N - 2) % (SELECT COUNT(1) FROM BB)) + 1
    )
    INSERT INTO PersonBanks
    (
        AccountNumber,
        CardNumber,
        Shaba,
        IsDefault,
        PersonId,
        BankBranchId,
        IsDeleted,
        IsActive,
        CreationDate
    )
    SELECT
        -- AccountNumber: 16 رقمی ساختگی
        RIGHT('0000000000000000' + CAST(ABS(CHECKSUM(NEWID())) AS NVARCHAR(40)), 16) AS AccountNumber,
        -- CardNumber: 16 رقمی ساختگی
        RIGHT('0000000000000000' + CAST(ABS(CHECKSUM(NEWID())) AS NVARCHAR(40)), 16) AS CardNumber,
        -- Shaba: IR + 24 رقم (ساختگی)
        N'IR' + RIGHT('000000000000000000000000' + CAST(ABS(CHECKSUM(NEWID())) AS NVARCHAR(40)), 24) AS Shaba,
        CASE WHEN g.AccNo = 1 THEN 1 ELSE 0 END AS IsDefault,
        g.PersonId,
        g.BankBranchId,
        0 AS IsDeleted,
        1 AS IsActive,
        GETDATE() AS CreationDate
    FROM Gen g
    WHERE NOT EXISTS
    (
        SELECT 1 FROM PersonBanks pb
        WHERE pb.PersonId = g.PersonId
          AND pb.BankBranchId = g.BankBranchId
    );
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
