using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedPayrollFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @EmployeeId BIGINT;
DECLARE @BranchId BIGINT;
DECLARE @FinancialPeriodId BIGINT;
DECLARE @AccountingDocumentId BIGINT;

DECLARE @Status INT;

DECLARE @TotalBenefits DECIMAL(18,2);
DECLARE @TotalDeduction DECIMAL(18,2);


WHILE @Counter <= 500
BEGIN


    -- Employee random
    SELECT TOP 1 
        @EmployeeId = Id
    FROM Employees
    ORDER BY NEWID();



    -- Branch random
    SELECT TOP 1 
        @BranchId = Id
    FROM Branches
    ORDER BY NEWID();



    -- Financial Period random
    SELECT TOP 1
        @FinancialPeriodId = Id
    FROM FinancialPeriods
    ORDER BY NEWID();



    -- وضعیت Payroll
    SET @Status =
    CASE
        WHEN @Counter % 20 = 0 THEN 5 -- Cancelled
        WHEN @Counter % 10 = 0 THEN 3 -- Paid
        WHEN @Counter % 5 = 0 THEN 2 -- Approved
        ELSE 1 -- Draft
    END;



    SET @AccountingDocumentId = NULL;


    -- اگر Paid باشد سند حسابداری وصل می‌کنیم
    IF @Status = 3
    BEGIN

        SELECT TOP 1
            @AccountingDocumentId = Id
        FROM AccountingDocuments
        ORDER BY NEWID();

    END



    SET @TotalBenefits =
        20000000 + ABS(CHECKSUM(NEWID())) % 50000000;


    SET @TotalDeduction =
        1000000 + ABS(CHECKSUM(NEWID())) % 10000000;




    INSERT INTO Payrolls
    (
        BranchId,
        EmployeeId,
        FinancialPeriodId,
        AccountingDocumentId,
        Status,
        TotalBenefits,
        TotalDeduction,

        IsDeleted,
        IsActive,
        DeletedAt,
        DeletedBy,
        CreationDate
    )

    VALUES
    (

        @BranchId,

        @EmployeeId,

        @FinancialPeriodId,

        @AccountingDocumentId,

        @Status,

        @TotalBenefits,

        @TotalDeduction,


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

DELETE FROM Payrolls;

");
        }
    }
}
