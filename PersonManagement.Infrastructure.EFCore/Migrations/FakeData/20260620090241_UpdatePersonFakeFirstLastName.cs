using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdatePersonFakeFirstLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
;WITH FirstNames AS (
    SELECT v.FirstName
    FROM (VALUES
        (N'علی'),(N'رضا'),(N'حسین'),(N'محمد'),(N'امیر'),
        (N'سینا'),(N'نیما'),(N'پارسا'),(N'کیان'),(N'مهدی'),
        (N'سامان'),(N'پویا'),(N'بهنام'),(N'عرفان'),(N'آرین'),
        (N'آرمین'),(N'محمدرضا'),(N'میلاد'),(N'پرهام'),(N'شایان'),
        (N'یاسین'),(N'دانیال'),(N'رامین'),(N'نوید'),(N'اشکان')
    ) v(FirstName)
),
LastNames AS (
    SELECT v.LastName
    FROM (VALUES
        (N'احمدی'),(N'محمدی'),(N'رضایی'),(N'کریمی'),(N'جعفری'),
        (N'حسینی'),(N'صادقی'),(N'مرادی'),(N'موسوی'),(N'عباسی'),
        (N'کاظمی'),(N'نصیری'),(N'قاسمی'),(N'حیدری'),(N'مرادیان'),
        (N'اکبری'),(N'صالحی'),(N'فراهانی'),(N'یوسفی'),(N'زارعی'),
        (N'بهرامی'),(N'طاهری'),(N'مهدوی'),(N'رستمی'),(N'شریفی')
    ) v(LastName)
),
Combinations AS (
    SELECT 
        f.FirstName,
        l.LastName,
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn
    FROM FirstNames f
    CROSS JOIN LastNames l
),
Target AS (
    SELECT 
        Id,
        ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Persons
    WHERE Id BETWEEN 1002 AND 1501
)
UPDATE P
SET
    FirstName = c.FirstName,
    LastName  = c.LastName
FROM Persons P
JOIN Target t ON P.Id = t.Id
JOIN Combinations c ON c.rn = t.rn;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Persons
SET FirstName = NULL,
    LastName = NULL
WHERE Id BETWEEN 1002 AND 1501;
");
        }
    }
}
