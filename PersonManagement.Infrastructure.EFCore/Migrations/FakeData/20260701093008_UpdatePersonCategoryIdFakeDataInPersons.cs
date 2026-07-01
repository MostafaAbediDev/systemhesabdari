using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdatePersonCategoryIdFakeDataInPersons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        UPDATE P
        SET PersonCategoryId =
        (
            SELECT TOP (1) PC.Id
            FROM PersonCategories PC
            ORDER BY NEWID()
        )
        FROM Persons P
        WHERE P.Id BETWEEN 1002 AND 1501;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
