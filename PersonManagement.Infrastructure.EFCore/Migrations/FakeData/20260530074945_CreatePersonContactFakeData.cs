using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreatePersonContactFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
;WITH P AS (
    SELECT Id AS PersonId,
           ROW_NUMBER() OVER (ORDER BY Id) AS prn
    FROM Persons
    WHERE IsDeleted = 0
),
T AS (
    SELECT Id AS ContactTypeId,
           ROW_NUMBER() OVER (ORDER BY Id) AS trn
    FROM ContactTypes
),
Cnt AS (
    SELECT 
        (SELECT COUNT(*) FROM P) AS PersonCount,
        (SELECT COUNT(*) FROM T) AS TypeCount
),
X AS (
    -- برای هر شخص 2 کانتکت تولید می‌کنیم
    SELECT 
        p.PersonId,
        p.prn,
        v.idx
    FROM P p
    CROSS APPLY (VALUES (1),(2)) v(idx)
),
Pick AS (
    -- انتخاب ContactType برای هر ردیف به صورت چرخشی
    SELECT
        x.PersonId,
        x.idx,
        t.ContactTypeId
    FROM X x
    CROSS JOIN Cnt
    JOIN T t
      ON t.trn = (((x.prn - 1) * 2 + (x.idx - 1)) % Cnt.TypeCount) + 1
),
Data AS (
    SELECT
        PersonId,
        ContactTypeId,
        idx,
        -- Value فیک (اگر نوع تماس را نشناسیم، یک مقدار عمومی می‌دهیم)
        CASE 
            WHEN idx = 1 THEN N'09' + RIGHT('000000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000000 AS VARCHAR(9)), 9)
            WHEN idx = 2 THEN N'user' + CAST(PersonId AS NVARCHAR(20)) + N'@fake.local'
            ELSE N'VAL-' + CAST(PersonId AS NVARCHAR(20)) + N'-' + CAST(idx AS NVARCHAR(10))
        END AS Value,
        N'FAKE-CONTACT-' + CAST(idx AS NVARCHAR(10)) AS Description,
        CASE WHEN idx = 1 THEN 1 ELSE 0 END AS IsDefault
    FROM Pick
)
INSERT INTO PersonContacts
(
    Value,
    Description,
    IsDefault,
    PersonId,
    ContactTypeId,
    CreationDate,
    IsDeleted,
    IsActive
)
SELECT
    d.Value,
    d.Description,
    d.IsDefault,
    d.PersonId,
    d.ContactTypeId,
    GETDATE(),
    0,
    1
FROM Data d;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM PersonContacts
WHERE Description LIKE N'FAKE-CONTACT-%';
");
        }
    }
}
