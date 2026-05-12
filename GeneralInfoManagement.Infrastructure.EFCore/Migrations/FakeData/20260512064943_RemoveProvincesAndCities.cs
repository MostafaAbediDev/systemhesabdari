using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class RemoveProvincesAndCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    DECLARE @sql NVARCHAR(MAX) = '';

    SELECT @sql += 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];'
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('Provinces')
       OR parent_object_id = OBJECT_ID('Provinces')
       OR referenced_object_id = OBJECT_ID('Cities')
       OR parent_object_id = OBJECT_ID('Cities');

    EXEC sp_executesql @sql;
    ");

            migrationBuilder.Sql(@"
    IF OBJECT_ID('Cities', 'U') IS NOT NULL
        DROP TABLE Cities;
    ");

            migrationBuilder.Sql(@"
    IF OBJECT_ID('Provinces', 'U') IS NOT NULL
        DROP TABLE Provinces;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
