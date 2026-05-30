using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateCodesToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Codes
                SET 
                    OwnerType = 2, -- Person
                    Value = REPLACE(Value, 'BR-', 'PE-') -- جایگزینی پیشوند اگر لازم است
                WHERE Id BETWEEN 750 AND 1000
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // در صورت برگشت (Rollback)، دوباره به برنچ تبدیل شوند
            migrationBuilder.Sql(@"
                UPDATE Codes
                SET 
                    OwnerType = 1, -- Branch
                    Value = REPLACE(Value, 'PE-', 'BR-')
                WHERE Id BETWEEN 750 AND 1000
            ");
        }
    }
}
