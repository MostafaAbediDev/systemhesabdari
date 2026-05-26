using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CitiesAndProvincesAddToBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ۱. ابتدا ستون‌ها را به صورت Nullable اضافه می‌کنیم تا از تداخل Foreign Key جلوگیری شود
            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "Branches",
                type: "bigint",
                nullable: true); // موقتا True

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "Branches",
                type: "bigint",
                nullable: true); // موقتا True

            // ۲. اجرای اسکریپت SQL برای تزریق دیتای فیک و واقعی
            migrationBuilder.Sql(@"
                -- پیدا کردن اولین استان و اولین شهر موجود در جدول‌های آماده
                DECLARE @DefaultProvId BIGINT = (SELECT TOP 1 Id FROM Provinces ORDER BY Id);
                DECLARE @DefaultCityId BIGINT = (SELECT TOP 1 Id FROM Cities WHERE ProvinceId = @DefaultProvId ORDER BY Id);

                -- اگر جدول شهرها خالی بود، اولین شهر کل جدول را بردار
                IF (@DefaultCityId IS NULL) SET @DefaultCityId = (SELECT TOP 1 Id FROM Cities ORDER BY Id);

                -- آپدیت رکوردها: هم شهر/استان و هم تولید شماره تلفن فیک برای رکوردهای 1001 تا 1500
                UPDATE Branches 
                SET ProvinceId = @DefaultProvId,
                    CityId = @DefaultCityId,
                    TelePhone = '02188' + CAST((ABS(CHECKSUM(NEWID())) % 900000 + 100000) AS NVARCHAR(10)) 
                WHERE Id BETWEEN 1001 AND 1500;

                -- برای اطمینان، اگر رکوردی خارج از بازه یا بدون مقدار مانده بود
                UPDATE Branches SET ProvinceId = @DefaultProvId, CityId = @DefaultCityId 
                WHERE ProvinceId IS NULL OR CityId IS NULL;
            ");

            // ۳. حالا که تمام سطرها مقدار معتبر دارند، ستون‌ها را به Non-Nullable تغییر می‌دهیم
            migrationBuilder.AlterColumn<long>(
                name: "CityId",
                table: "Branches",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ProvinceId",
                table: "Branches",
                nullable: false,
                oldNullable: true);

            // ۴. ایجاد ایندکس‌ها و روابط Foreign Key
            migrationBuilder.CreateIndex(
                name: "IX_Branches_CityId",
                table: "Branches",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ProvinceId",
                table: "Branches",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Cities_CityId",
                table: "Branches",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Provinces_ProvinceId",
                table: "Branches",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Cities_CityId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Provinces_ProvinceId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_CityId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_ProvinceId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Branches");
        }
    }
}
