using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdatePictureFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

                WITH CTE_Pictures AS (
                    SELECT TOP (500)
                           Id,
                           ROW_NUMBER() OVER (ORDER BY Id) AS NewOwnerId
                    FROM Pictures
                    WHERE IsDeleted = 0
                )
                UPDATE p
                SET 
                    p.OwnerId = c.NewOwnerId,
                    p.OwnerType = 1, -- Product
                    p.Url = CONCAT('/assets/images/product_', c.NewOwnerId, '.jpg'),
                    p.IsMain = CASE WHEN c.NewOwnerId = 1 THEN 1 ELSE 0 END,
                    p.IsActive = 1
                FROM Pictures p
                INNER JOIN CTE_Pictures c ON p.Id = c.Id;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRINT 'Rollback of Picture update data is not automatically supported without backup.';
            ");
        }
    }
}
