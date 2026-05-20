using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class UpdateBranchSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @i INT = 1;
                WHILE @i <= 500
                BEGIN
                    INSERT INTO Branches
                    (
                        Title, NationalId, EconomicCode, RegisterNumber, Email, Phone, 
                        [Latitude], [Longitude], Address, PostCode, 
                        IsMain, CompanyId, IsDeleted, IsActive, CreationDate
                    )
                    VALUES
                    (
                        CONCAT(N'شعبه ', @i), 100000000 + @i, 200000000 + @i, 300000 + @i,
                        CONCAT('branch', @i, '@test.com'), '09120000000',
                        35.6892, 51.3890, CONCAT(N'آدرس تست ', @i), 
                        RIGHT(CONCAT('0000000000', @i), 10),
                        CASE WHEN @i = 1 THEN 1 ELSE 0 END, 1, 0, 1, GETDATE()
                    );
                    SET @i = @i + 1;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // در متد Down می‌توانی بنویسی که در صورت Rollback مایگریشن، این دیتاها پاک شوند:
            migrationBuilder.Sql("DELETE FROM Branches WHERE Title LIKE N'شعبه %'");
        }
    }
}
