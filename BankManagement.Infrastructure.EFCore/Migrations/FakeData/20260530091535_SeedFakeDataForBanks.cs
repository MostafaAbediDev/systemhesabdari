using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakeDataForBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM BankTypes WHERE ISNULL(IsDeleted, 0) = 0)
   AND NOT EXISTS (SELECT 1 FROM Banks)
BEGIN
    ;WITH BankTypeRows AS
    (
        SELECT 
            Id,
            ROW_NUMBER() OVER (ORDER BY Id) AS RowNum
        FROM BankTypes
        WHERE ISNULL(IsDeleted, 0) = 0
    )
    INSERT INTO Banks
    (
        Title,
        BankTypeId,
        Country,
        Description,
        Logo,
        IsDeleted,
        IsActive,
        CreationDate
    )
    SELECT 
        CASE RowNum
            WHEN 1 THEN N'بانک ملی ایران'
            WHEN 2 THEN N'بانک ملت'
            WHEN 3 THEN N'بانک صادرات ایران'
            WHEN 4 THEN N'بانک تجارت'
            WHEN 5 THEN N'بانک سپه'
            WHEN 6 THEN N'بانک پاسارگاد'
            WHEN 7 THEN N'بانک پارسیان'
            WHEN 8 THEN N'بانک آینده'
            WHEN 9 THEN N'بانک سامان'
            WHEN 10 THEN N'بانک شهر'
            ELSE N'بانک نمونه ' + CAST(RowNum AS NVARCHAR(10))
        END AS Title,
        Id AS BankTypeId,
        N'ایران' AS Country,
        N'داده آزمایشی برای بانک' AS Description,
        N'default-bank-logo.png' AS Logo,
        0 AS IsDeleted,
        1 AS IsActive,
        GETDATE() AS CreationDate
    FROM BankTypeRows;
END
");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM Banks
WHERE Title IN
(
    N'بانک ملی ایران',
    N'بانک ملت',
    N'بانک صادرات ایران',
    N'بانک تجارت',
    N'بانک سپه',
    N'بانک پاسارگاد',
    N'بانک پارسیان',
    N'بانک آینده',
    N'بانک سامان',
    N'بانک شهر'
)
OR Title LIKE N'بانک نمونه %'
");
        }
    }
}
