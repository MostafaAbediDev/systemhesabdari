using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateContactFirstNameAndLastNameFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @Names TABLE(Id INT IDENTITY(1,1), FirstName NVARCHAR(100), LastName NVARCHAR(100));

INSERT INTO @Names (FirstName, LastName)
VALUES
(N'علی', N'محمدی'),
(N'رضا', N'احمدی'),
(N'حسین', N'کریمی'),
(N'مهدی', N'حیدری'),
(N'محمد', N'مرادی'),
(N'امیر', N'صادقی'),
(N'سعید', N'رحیمی'),
(N'یوسف', N'اکبری'),
(N'جواد', N'جعفری'),
(N'حمید', N'نادری'),
(N'بهزاد', N'قاسمی'),
(N'مسعود', N'کاظمی'),
(N'احسان', N'زارعی'),
(N'وحید', N'حسینی'),
(N'میلاد', N'زارع'),
(N'نوید', N'شفیعی'),
(N'آرمان', N'خلیلی'),
(N'پویا', N'رستمی'),
(N'فرزاد', N'سلیمانی'),
(N'کامران', N'یوسفی');

UPDATE P
SET
    ContactFirstName = N.FirstName,
    ContactLastName = N.LastName
FROM Persons P
CROSS APPLY
(
    SELECT TOP 1 FirstName, LastName
    FROM @Names
    ORDER BY NEWID()
) N
WHERE
    P.Id BETWEEN 1002 AND 1501
    AND P.IsLegal = 1;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Persons
SET
    ContactFirstName = NULL,
    ContactLastName = NULL
WHERE
    Id BETWEEN 1002 AND 1501
    AND IsLegal = 1;
");
        }
    }
}
