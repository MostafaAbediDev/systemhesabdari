using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreatePersonAddressFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
;WITH P AS (
    SELECT Id AS PersonId,
           ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Persons
    WHERE IsDeleted = 0
),
C AS (
    SELECT Id AS CityId,
           ProvinceId,
           ROW_NUMBER() OVER (ORDER BY ProvinceId, Id) AS rn
    FROM Cities
),
Cnt AS (
    SELECT 
        (SELECT COUNT(*) FROM P) AS PersonCount,
        (SELECT COUNT(*) FROM C) AS CityCount
),
A AS (
    SELECT 
        p.PersonId,
        p.rn,
        v.addrIndex
    FROM P p
    CROSS APPLY (VALUES (1),(2)) v(addrIndex)
),
Pick AS (
    SELECT
        a.PersonId,
        a.addrIndex,
        c.CityId,
        c.ProvinceId,
        ROW_NUMBER() OVER (ORDER BY a.PersonId, a.addrIndex) AS xrn
    FROM A a
    CROSS JOIN Cnt
    JOIN C c 
      ON c.rn = (( (a.rn - 1) * 2 + (a.addrIndex - 1) ) % Cnt.CityCount) + 1
)
INSERT INTO PersonAddresses
(
    Title,
    Address,
    PostalCode,
    IsDefault,
    PersonId,
    ProvinceId,
    CityId,
    CreationDate,
    IsDeleted,
    IsActive
)
SELECT
    N'آدرس ' + CAST(p.addrIndex AS NVARCHAR(10)),
    N'خیابان تست ' + CAST(p.PersonId AS NVARCHAR(20)) + N' - پلاک ' + CAST(100 + p.addrIndex AS NVARCHAR(10)),
    RIGHT('0000000000' + CAST(1000000000 + (ABS(CHECKSUM(NEWID())) % 999999999) AS VARCHAR(10)), 10),
    CASE WHEN p.addrIndex = 1 THEN 1 ELSE 0 END,
    p.PersonId,
    p.ProvinceId,
    p.CityId,
    GETDATE(),
    0,
    1
FROM Pick p;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM PersonAddresses
WHERE Title IN (N'آدرس 1', N'آدرس 2');
");
        }
    }
}
