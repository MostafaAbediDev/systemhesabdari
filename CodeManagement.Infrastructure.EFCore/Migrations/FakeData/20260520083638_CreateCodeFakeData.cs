using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreateCodeFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO Codes (Value, OwnerId, OwnerType, CreationDate, IsDeleted, IsActive)
        SELECT 
            'BR-' + RIGHT('00000' + CAST(ROW_NUMBER() OVER (ORDER BY Id) AS VARCHAR), 5),
            Id,
            1,
            GETDATE(),
            0,
            1
        FROM Branches
        WHERE NOT EXISTS (SELECT 1 FROM Codes c WHERE c.OwnerId = Branches.Id AND c.OwnerType = 1)
    ");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM Codes
        WHERE OwnerType = 1
    ");
        }
    }
}
