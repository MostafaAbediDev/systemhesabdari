using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdatePersonCategoryIdFakeDataInPersonsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        UPDATE P
SET PersonCategoryId = X.Id
FROM Persons P
CROSS APPLY
(
    SELECT TOP (1) Id
    FROM PersonCategories
    WHERE CHECKSUM(NEWID(), P.Id) IS NOT NULL
    ORDER BY CHECKSUM(NEWID(), P.Id)
) X
WHERE P.Id BETWEEN 1002 AND 1501;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
