using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFundsFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @BranchId BIGINT;
DECLARE @AccountId BIGINT;


WHILE @Counter <= 500
BEGIN


    -- Branch تصادفی
    SELECT TOP 1
        @BranchId = Id
    FROM Branches
    ORDER BY NEWID();



    -- Account تصادفی
    SELECT TOP 1
        @AccountId = Id
    FROM Accounts
    ORDER BY NEWID();



    INSERT INTO Funds
    (
        Title,

        BranchId,

        AccountId,

        IsDeleted,

        IsActive,

        DeletedAt,

        DeletedBy,

        CreationDate
    )

    VALUES
    (

        N'Fund ' + CAST(@Counter AS NVARCHAR(10)),


        @BranchId,


        @AccountId,


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

DELETE FROM Funds
WHERE Title LIKE N'Fund %';

");
        }
    }
}
