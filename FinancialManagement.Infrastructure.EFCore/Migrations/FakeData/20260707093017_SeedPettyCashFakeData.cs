using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedPettyCashFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @BranchId BIGINT;
DECLARE @ResponsiblePersonId BIGINT;
DECLARE @HolderPersonId BIGINT;

DECLARE @AccountId BIGINT;
DECLARE @SettlementAccountId BIGINT;


DECLARE @InitialAmount DECIMAL(18,2);
DECLARE @MaxLimit DECIMAL(18,2);



WHILE @Counter <= 500
BEGIN


    -- Branch تصادفی
    SELECT TOP 1
        @BranchId = Id
    FROM Branches
    ORDER BY NEWID();



    -- مسئول صندوق
    SELECT TOP 1
        @ResponsiblePersonId = Id
    FROM Persons
    ORDER BY NEWID();



    -- تحویل گیرنده صندوق
    SELECT TOP 1
        @HolderPersonId = Id
    FROM Persons
    WHERE Id <> @ResponsiblePersonId
    ORDER BY NEWID();



    -- حساب صندوق
    SELECT TOP 1
        @AccountId = Id
    FROM Accounts
    ORDER BY NEWID();



    -- حساب تسویه
    SELECT TOP 1
        @SettlementAccountId = Id
    FROM Accounts
    WHERE Id <> @AccountId
    ORDER BY NEWID();



    SET @InitialAmount =
        5000000 +
        ABS(CHECKSUM(NEWID())) % 50000000;



    SET @MaxLimit =
        @InitialAmount +
        ABS(CHECKSUM(NEWID())) % 50000000;



    INSERT INTO PettyCashes
    (
        Title,
        Description,

        InitialAmount,
        CurrentBalance,
        MaxLimit,

        LastSettlementDate,

        BranchId,

        ResponsiblePersonId,

        HolderPersonId,

        AccountId,

        SettlementAccountId,


        IsDeleted,

        IsActive,

        DeletedAt,

        DeletedBy,

        CreationDate
    )

    VALUES
    (

        N'Petty Cash ' + CAST(@Counter AS NVARCHAR(10)),


        N'Fake petty cash record',


        @InitialAmount,


        @InitialAmount,


        @MaxLimit,



        DATEADD
        (
            DAY,
            -ABS(CHECKSUM(NEWID())) % 365,
            GETDATE()
        ),



        @BranchId,


        @ResponsiblePersonId,


        @HolderPersonId,


        @AccountId,


        @SettlementAccountId,



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

DELETE FROM PettyCashes
WHERE Description = N'Fake petty cash record';

");
        }
    }
}
