using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateBranchesWithRandomData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @MinProv BIGINT, @MaxProv BIGINT, @MinCity BIGINT, @MaxCity BIGINT;
                
                SELECT @MinProv = MIN(Id), @MaxProv = MAX(Id) FROM Provinces;
                SELECT @MinCity = MIN(Id), @MaxCity = MAX(Id) FROM Cities;

                UPDATE Branches
                SET 
                    ProvinceId = ABS(CHECKSUM(NEWID())) % (@MaxProv - @MinProv + 1) + @MinProv,
                    CityId = ABS(CHECKSUM(NEWID())) % (@MaxCity - @MinCity + 1) + @MinCity

                WHERE Id BETWEEN 1001 AND 1500;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
