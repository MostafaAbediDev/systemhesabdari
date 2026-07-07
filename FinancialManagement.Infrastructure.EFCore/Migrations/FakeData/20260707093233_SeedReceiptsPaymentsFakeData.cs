using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedReceiptsPaymentsFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;


DECLARE @BranchId BIGINT;
DECLARE @FinancialPeriodId BIGINT;

DECLARE @PersonId BIGINT;
DECLARE @FundId BIGINT;
DECLARE @CompanyBankAccountId BIGINT;
DECLARE @ChequeId BIGINT;
DECLARE @AccountingDocumentId BIGINT;


DECLARE @PaymentMethod INT;
DECLARE @Type INT;


DECLARE @Amount DECIMAL(18,2);



WHILE @Counter <= 500
BEGIN


    SET @PersonId = NULL;
    SET @FundId = NULL;
    SET @CompanyBankAccountId = NULL;
    SET @ChequeId = NULL;
    SET @AccountingDocumentId = NULL;



    -- Branch
    SELECT TOP 1
        @BranchId = Id
    FROM Branches
    ORDER BY NEWID();



    -- Financial Period
    SELECT TOP 1
        @FinancialPeriodId = Id
    FROM FinancialPeriods
    ORDER BY NEWID();



    -- Receipt / Payment
    SET @Type =
        CASE
            WHEN @Counter % 2 = 0 THEN 1
            ELSE 2
        END;



    -- Payment Method
    SET @PaymentMethod =
        1 + ABS(CHECKSUM(NEWID())) % 4;



    SET @Amount =
        1000000 +
        ABS(CHECKSUM(NEWID())) % 100000000;



    -- ارتباط با شخص (اکثراً)
    IF ABS(CHECKSUM(NEWID())) % 100 < 70
    BEGIN

        SELECT TOP 1
            @PersonId = Id
        FROM Persons
        ORDER BY NEWID();

    END



    -- Cash
    IF @PaymentMethod = 1
    BEGIN
        SET @FundId = NULL;
        SET @CompanyBankAccountId = NULL;
        SET @ChequeId = NULL;
    END



    -- Bank Transfer
    IF @PaymentMethod = 2
    BEGIN

        SELECT TOP 1
            @CompanyBankAccountId = Id
        FROM CompanyBankAccounts
        ORDER BY NEWID();

    END



    -- Cheque
    IF @PaymentMethod = 3
    BEGIN

        SELECT TOP 1
            @ChequeId = Id
        FROM Cheques
        ORDER BY NEWID();

    END



    -- Fund
    IF @PaymentMethod = 4
    BEGIN

        SELECT TOP 1
            @FundId = Id
        FROM Funds
        ORDER BY NEWID();

    END



    -- بعضی تراکنش‌ها سند حسابداری داشته باشند
    IF @Counter % 3 = 0
    BEGIN

        SELECT TOP 1
            @AccountingDocumentId = Id
        FROM AccountingDocuments
        ORDER BY NEWID();

    END



    INSERT INTO ReceiptsPayments
    (
        Description,

        Amount,

        Type,

        PaymentMethod,

        TransactionDate,

        BranchId,

        FinancialPeriodId,

        PersonId,

        FundId,

        CompanyBankAccountId,

        ChequeId,

        AccountingDocumentId,


        IsDeleted,

        IsActive,

        DeletedAt,

        DeletedBy,

        CreationDate
    )

    VALUES
    (

        N'Fake Receipt Payment '
        + CAST(@Counter AS NVARCHAR(10)),


        @Amount,


        @Type,


        @PaymentMethod,


        DATEADD
        (
            DAY,
            -ABS(CHECKSUM(NEWID())) % 365,
            GETDATE()
        ),


        @BranchId,


        @FinancialPeriodId,


        @PersonId,


        @FundId,


        @CompanyBankAccountId,


        @ChequeId,


        @AccountingDocumentId,



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

DELETE FROM ReceiptsPayments
WHERE Description LIKE N'Fake Receipt Payment %';

");
        }
    }
}
