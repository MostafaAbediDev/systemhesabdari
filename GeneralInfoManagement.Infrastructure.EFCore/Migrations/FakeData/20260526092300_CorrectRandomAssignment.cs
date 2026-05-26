using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CorrectRandomAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                WITH NumberedBranches AS (
                    SELECT Id, 
                           ROW_NUMBER() OVER (ORDER BY Id) as RowNum
                    FROM Branches
                    WHERE Id BETWEEN 1001 AND 1500
                ),
                RandomCities AS (
                    SELECT Id as CityId, ProvinceId,
                           ROW_NUMBER() OVER (ORDER BY NEWID()) as RowNum
                    FROM Cities
                )
                UPDATE B
                SET 
                    B.ProvinceId = RC.ProvinceId,
                    B.CityId = RC.CityId,
                    B.TelePhone = '0' + CAST((ABS(CHECKSUM(NEWID(), B.Id)) % 80 + 11) AS NVARCHAR(2)) 
                                + CAST((ABS(CHECKSUM(NEWID(), B.Id)) % 8999999 + 1000000) AS NVARCHAR(10))
                FROM Branches B
                JOIN NumberedBranches NB ON B.Id = NB.Id
                JOIN RandomCities RC ON RC.RowNum = ((NB.RowNum - 1) % (SELECT COUNT(*) FROM Cities)) + 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
