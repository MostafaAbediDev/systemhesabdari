using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollSystemManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedPayrollDetailFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @PayrollId BIGINT;
DECLARE @PayrollItemId BIGINT;

DECLARE @Counter INT = 1;
DECLARE @DetailCount INT;

DECLARE @Quantity DECIMAL(18,2);
DECLARE @Rate DECIMAL(18,2);


DECLARE PayrollCursor CURSOR FOR
SELECT Id
FROM Payrolls;


OPEN PayrollCursor;

FETCH NEXT FROM PayrollCursor INTO @PayrollId;


WHILE @@FETCH_STATUS = 0
BEGIN


    -- تعداد آیتم برای هر حقوق
    SET @DetailCount = 2 + ABS(CHECKSUM(NEWID())) % 4;


    SET @Counter = 1;


    WHILE @Counter <= @DetailCount
    BEGIN


        -- انتخاب آیتم حقوقی تصادفی
        SELECT TOP 1
            @PayrollItemId = Id
        FROM PayrollItems
        ORDER BY NEWID();



        SET @Quantity =
            1 + ABS(CHECKSUM(NEWID())) % 5;



        SET @Rate =
            500000 +
            ABS(CHECKSUM(NEWID())) % 20000000;



        INSERT INTO PayrollDetails
        (
            Quantity,
            Rate,
            Amount,
            Description,
            PayrollId,
            PayrollItemId,

            IsDeleted,
            IsActive,
            DeletedAt,
            DeletedBy,
            CreationDate
        )

        VALUES
        (

            @Quantity,

            @Rate,

            (@Quantity * @Rate),

            N'Fake Payroll Detail',

            @PayrollId,

            @PayrollItemId,


            0,

            1,

            NULL,

            NULL,

            GETDATE()

        );


        SET @Counter = @Counter + 1;


    END



    FETCH NEXT FROM PayrollCursor INTO @PayrollId;


END



CLOSE PayrollCursor;

DEALLOCATE PayrollCursor;





-- بروزرسانی مجموع حقوق‌ها

UPDATE P
SET

    TotalBenefits =
    ISNULL
    (
        (
            SELECT SUM(PD.Amount)
            FROM PayrollDetails PD
            INNER JOIN PayrollItems PI
                ON PI.Id = PD.PayrollItemId
            WHERE 
                PD.PayrollId = P.Id
                AND PI.ItemType = 1
        ),
        0
    ),


    TotalDeduction =
    ISNULL
    (
        (
            SELECT SUM(PD.Amount)
            FROM PayrollDetails PD
            INNER JOIN PayrollItems PI
                ON PI.Id = PD.PayrollItemId
            WHERE 
                PD.PayrollId = P.Id
                AND PI.ItemType = 2
        ),
        0
    )

FROM Payrolls P;


");
        }



        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DELETE FROM PayrollDetails;

");
        }
    }
}
