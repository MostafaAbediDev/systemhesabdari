using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedMoreBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM BankTypes WHERE ISNULL(IsDeleted,0)=0)
BEGIN
    ;WITH Seed AS
    (
        SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RN, Title
        FROM (VALUES
            (N'بانک ملی ایران'),
            (N'بانک سپه'),
            (N'بانک ملت'),
            (N'بانک صادرات ایران'),
            (N'بانک تجارت'),
            (N'بانک رفاه کارگران'),
            (N'بانک مسکن'),
            (N'بانک کشاورزی'),
            (N'بانک صنعت و معدن'),
            (N'پست بانک ایران'),
            (N'بانک توسعه صادرات ایران'),
            (N'بانک توسعه تعاون'),
            (N'بانک کارآفرین'),
            (N'بانک پاسارگاد'),
            (N'بانک پارسیان'),
            (N'بانک سامان'),
            (N'بانک اقتصاد نوین'),
            (N'بانک سینا'),
            (N'بانک شهر'),
            (N'بانک دی'),
            (N'بانک سرمایه'),
            (N'بانک گردشگری'),
            (N'بانک خاورمیانه'),
            (N'بانک ایران زمین'),
            (N'بانک آینده'),
            (N'بانک مهر ایران'),
            (N'بانک قرض الحسنه رسالت')
        ) V(Title)
    ),
    Types AS
    (
        SELECT 
            Id,
            ROW_NUMBER() OVER (ORDER BY Id) AS TRN
        FROM BankTypes
        WHERE ISNULL(IsDeleted,0)=0
    ),
    TypeCount AS
    (
        SELECT COUNT(1) AS Cnt FROM Types
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
        s.Title,
        t.Id AS BankTypeId,
        N'ایران' AS Country,
        N'FakeData SeedMoreBanks' AS Description,
        N'default-bank-logo.png' AS Logo,
        0 AS IsDeleted,
        1 AS IsActive,
        GETDATE() AS CreationDate
    FROM Seed s
    CROSS JOIN TypeCount tc
    JOIN Types t
      ON t.TRN = (((s.RN - 1) % tc.Cnt) + 1)
    WHERE NOT EXISTS (SELECT 1 FROM Banks b WHERE b.Title = s.Title);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // فقط رکوردهایی را پاک می‌کنیم که همین migration اضافه کرده (با Description مشخص)
            migrationBuilder.Sql(@"
DELETE FROM Banks
WHERE Description = N'FakeData SeedMoreBanks';
");
        }
    }
}
