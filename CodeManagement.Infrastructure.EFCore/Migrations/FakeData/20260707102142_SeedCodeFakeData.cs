using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedCodeFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DECLARE @Counter INT = 1;

DECLARE @OwnerType INT;
DECLARE @OwnerId BIGINT;

DECLARE @Prefix NVARCHAR(10);
DECLARE @CodeValue NVARCHAR(50);



WHILE @Counter <= 500
BEGIN


    SET @OwnerId = NULL;


    -- انتخاب نوع Owner تصادفی
    SET @OwnerType =
        1 + ABS(CHECKSUM(NEWID())) % 4;



    /*
       Branch
    */
    IF @OwnerType = 1
    BEGIN

        SELECT TOP 1
            @OwnerId = B.Id
        FROM Branches B
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM Codes C
            WHERE C.OwnerId = B.Id
            AND C.OwnerType = 1
        )
        ORDER BY NEWID();


        SET @Prefix = 'BR-';

    END



    /*
       Person
    */
    IF @OwnerType = 2
    BEGIN

        SELECT TOP 1
            @OwnerId = P.Id
        FROM Persons P
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM Codes C
            WHERE C.OwnerId = P.Id
            AND C.OwnerType = 2
        )
        ORDER BY NEWID();


        SET @Prefix = 'PE-';

    END



    /*
       Company Bank Account
    */
    IF @OwnerType = 3
    BEGIN

        SELECT TOP 1
            @OwnerId = CBA.Id
        FROM CompanyBankAccounts CBA
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM Codes C
            WHERE C.OwnerId = CBA.Id
            AND C.OwnerType = 3
        )
        ORDER BY NEWID();


        SET @Prefix = 'CBA-';

    END



    /*
       Account
    */
    IF @OwnerType = 4
    BEGIN

        SELECT TOP 1
            @OwnerId = A.Id
        FROM Accounts A
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM Codes C
            WHERE C.OwnerId = A.Id
            AND C.OwnerType = 4
        )
        ORDER BY NEWID();


        SET @Prefix = 'ACC-';

    END



    -- اگر Owner موجود نبود برو رکورد بعدی
    IF @OwnerId IS NOT NULL
    BEGIN


        SET @CodeValue =
            @Prefix +
            CAST(
                10000 + ABS(CHECKSUM(NEWID())) % 89999
                AS NVARCHAR(10)
            );



        WHILE EXISTS
        (
            SELECT 1
            FROM Codes
            WHERE Value = @CodeValue
        )

        BEGIN

            SET @CodeValue =
                @Prefix +
                CAST(
                    10000 + ABS(CHECKSUM(NEWID())) % 89999
                    AS NVARCHAR(10)
                );

        END



        INSERT INTO Codes
        (
            Value,
            OwnerId,
            OwnerType,

            IsDeleted,
            IsActive,

            DeletedAt,
            DeletedBy,

            CreationDate
        )

        VALUES
        (
            @CodeValue,
            @OwnerId,
            @OwnerType,

            0,
            1,

            NULL,
            NULL,

            GETDATE()
        );


    END



    SET @Counter = @Counter + 1;


END


");
        }



        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DELETE FROM Codes
WHERE CreationDate >= DATEADD(MINUTE,-5,GETDATE());

");
        }
    }
}
