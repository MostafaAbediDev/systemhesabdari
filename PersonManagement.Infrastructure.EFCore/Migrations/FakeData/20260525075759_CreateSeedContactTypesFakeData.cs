using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreateSeedContactTypesFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

                DECLARE @i INT = 1;
                DECLARE @titles TABLE (Title NVARCHAR(100));
                
                INSERT INTO @titles VALUES 
                (N'موبایل'), (N'تلفن ثابت'), (N'ایمیل'), (N'فکس'), 
                (N'واتس‌اپ'), (N'تلگرام'), (N'لینکدین'), (N'اینستاگرام'),
                (N'وب‌سایت'), (N'آدرس پستی');

                DECLARE @currentTitle NVARCHAR(100);
                DECLARE title_cursor CURSOR FOR SELECT Title FROM @titles;

                OPEN title_cursor;
                FETCH NEXT FROM title_cursor INTO @currentTitle;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    INSERT INTO ContactTypes (Title, IsDeleted, IsActive, CreationDate)
                    VALUES (@currentTitle, 0, 1, GETDATE());

                    FETCH NEXT FROM title_cursor INTO @currentTitle;
                END;

                CLOSE title_cursor;
                DEALLOCATE title_cursor;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ContactTypes 
                WHERE Title IN (N'موبایل', N'تلفن ثابت', N'ایمیل', N'فکس', 
                                N'واتس‌اپ', N'تلگرام', N'لینکدین', N'اینستاگرام',
                                N'وب‌سایت', N'آدرس پستی');
            ");
        }
    }
}
