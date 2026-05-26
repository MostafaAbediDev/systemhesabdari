using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class FixAndRandomizeBranchesLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE B
                SET 
                    B.ProvinceId = RandomLoc.ProvinceId,
                    B.CityId = RandomLoc.Id,
                    B.TelePhone = '0' + CAST((ABS(CHECKSUM(NEWID())) % 80 + 11) AS NVARCHAR(2)) 
                                + CAST((ABS(CHECKSUM(NEWID())) % 8999999 + 1000000) AS NVARCHAR(10))
                FROM Branches B
                CROSS APPLY (
                    SELECT TOP 1 Id, ProvinceId 
                    FROM Cities 
                    ORDER BY NEWID() 
                ) AS RandomLoc
                WHERE B.Id BETWEEN 1001 AND 1500;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
