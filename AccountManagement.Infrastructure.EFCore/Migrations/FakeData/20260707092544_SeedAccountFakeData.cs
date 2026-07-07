using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedAccountFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @CompanyId BIGINT;
DECLARE @ParentId BIGINT;


WHILE @Counter <= 500
BEGIN


    -- انتخاب شرکت تصادفی
    SELECT TOP 1
        @CompanyId = Id
    FROM Companies
    ORDER BY NEWID();



    SET @ParentId = NULL;


    -- از رکورد دوم به بعد بعضی حساب‌ها زیرمجموعه شوند
    IF @Counter > 20 
       AND ABS(CHECKSUM(NEWID())) % 100 < 80
    BEGIN

        SELECT TOP 1
            @ParentId = Id
        FROM Accounts
        ORDER BY NEWID();

    END



    INSERT INTO Accounts
    (
        Title,
        Description,

        Level,
        AccountType,
        Nature,

        IsPersonRelated,
        IsProductRelated,
        IsBankRelated,
        IsFundRelated,
        IsEmployeeRelated,

        AllowManualEntry,
        IsControlAccount,

        UpdatedAt,

        ParentId,
        CompanyId,

        IsDeleted,
        IsActive,

        DeletedAt,
        DeletedBy,

        CreationDate
    )

    VALUES
    (

        N'Account ' + CAST(@Counter AS NVARCHAR(10)),


        N'Fake accounting account',


        -- AccountLevel
        1 + ABS(CHECKSUM(NEWID())) % 4,


        -- AccountType
        1 + ABS(CHECKSUM(NEWID())) % 5,


        -- Nature
        1 + ABS(CHECKSUM(NEWID())) % 3,



        -- Person Related
        CASE 
            WHEN @Counter % 7 = 0 THEN 1
            ELSE 0
        END,


        -- Product Related
        CASE 
            WHEN @Counter % 11 = 0 THEN 1
            ELSE 0
        END,


        -- Bank Related
        CASE 
            WHEN @Counter % 13 = 0 THEN 1
            ELSE 0
        END,


        -- Fund Related
        CASE 
            WHEN @Counter % 17 = 0 THEN 1
            ELSE 0
        END,


        -- Employee Related
        CASE 
            WHEN @Counter % 19 = 0 THEN 1
            ELSE 0
        END,



        -- Allow Manual Entry
        1,


        -- Control Account
        CASE
            WHEN @Counter % 10 = 0 THEN 1
            ELSE 0
        END,



        GETDATE(),


        @ParentId,


        @CompanyId,


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

DELETE FROM Accounts
WHERE Description = N'Fake accounting account';

");
        }
    }
}
