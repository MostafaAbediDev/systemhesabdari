using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedAccountDocumnetFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @BranchId BIGINT;
DECLARE @FinancialPeriodId BIGINT;
DECLARE @CreatorId BIGINT;
DECLARE @ApproverId BIGINT;

DECLARE @Status INT;


WHILE @Counter <= 500
BEGIN


    -- Random Branch
    SELECT TOP 1 @BranchId = Id
    FROM Branches
    ORDER BY NEWID();


    -- Random Financial Period
    SELECT TOP 1 @FinancialPeriodId = Id
    FROM FinancialPeriods
    ORDER BY NEWID();


    -- Random Creator
    SELECT TOP 1 @CreatorId = Id
    FROM Persons
    ORDER BY NEWID();



    -- Random Status
    SET @Status = 
        CASE 
            WHEN @Counter % 10 = 0 THEN 3 -- Approved
            WHEN @Counter % 15 = 0 THEN 4 -- Rejected
            WHEN @Counter % 20 = 0 THEN 5 -- Cancelled
            ELSE 1 -- Draft
        END;



    SET @ApproverId = NULL;


    IF @Status IN (3,4)
    BEGIN

        SELECT TOP 1 @ApproverId = Id
        FROM Persons
        WHERE Id <> @CreatorId
        ORDER BY NEWID();

    END



    INSERT INTO AccountingDocuments
    (
        DocumentNumber,
        DocumentDate,
        DocumentType,
        Status,
        Description,
        ReferenceType,
        ReferenceId,
        ApprovedBy,
        ApprovedAt,
        CreatedBy,
        BranchId,
        FinancialPeriodId,
        IsDeleted,
        IsActive,
        DeletedAt,
        DeletedBy,
        CreationDate
    )

    VALUES
    (

        -- شماره سند یکتا
        100000 + @Counter,


        -- تاریخ تصادفی در 3 سال گذشته
        DATEADD
        (
            DAY,
            -ABS(CHECKSUM(NEWID())) % 1095,
            CAST(GETDATE() AS DATE)
        ),


        -- نوع سند
        1 + ABS(CHECKSUM(NEWID())) % 5,


        @Status,


        N'Fake Accounting Document ' 
        + CAST(@Counter AS NVARCHAR(10)),


        -- ReferenceType
        ABS(CHECKSUM(NEWID())) % 10,


        NULL,


        @ApproverId,


        CASE 
            WHEN @Status IN (3,4)
            THEN DATEADD
            (
                DAY,
                -ABS(CHECKSUM(NEWID())) % 30,
                GETDATE()
            )
            ELSE NULL
        END,


        @CreatorId,


        @BranchId,


        @FinancialPeriodId,


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

DELETE FROM AccountingDocuments
WHERE Description LIKE N'Fake Accounting Document %';

");
        }
    }
}
