using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreateSeedBranchArchiveFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @i INT = 1;
                DECLARE @randomBranchId INT;

                WHILE @i <= 500
                BEGIN
                    -- انتخاب BranchId تصادفی بین 1001 تا 1500
                    SET @randomBranchId = CAST(RAND() * 500 + 1001 AS INT);

                    INSERT INTO BranchArchive
                    (
                        Title,
                        [Description],
                        [File],
                        BranchId,
                        IsDeleted,
                        IsActive,
                        CreationDate
                    )
                    VALUES
                    (
                        CONCAT(N'آرشیو شعبه ', @i),
                        CONCAT(N'توضیحات آرشیو برای رکورد ', @i),
                        CONCAT('branch-archive-', @i, '.pdf'),
                        @randomBranchId,
                        0,
                        1,
                        GETDATE()
                    );

                    SET @i = @i + 1;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM BranchArchives
                WHERE Title LIKE N'آرشیو شعبه %'
            ");
        }
    }
}
