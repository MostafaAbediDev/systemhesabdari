using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class FixFakePersonContactsValueByType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE pc
SET pc.Value =
    CASE ct.Title
        WHEN N'موبایل' THEN
            N'09' + RIGHT('000000000' + CAST(pc.PersonId % 1000000000 AS VARCHAR(9)), 9)

        WHEN N'واتس‌اپ' THEN
            N'09' + RIGHT('000000000' + CAST((pc.PersonId + 1000) % 1000000000 AS VARCHAR(9)), 9)

        WHEN N'تلگرام' THEN
            N'tg_user_' + CAST(pc.PersonId AS NVARCHAR(20))

        WHEN N'اینستاگرام' THEN
            N'insta_user_' + CAST(pc.PersonId AS NVARCHAR(20))

        WHEN N'لینکدین' THEN
            N'https://linkedin.com/in/user-' + CAST(pc.PersonId AS NVARCHAR(20))

        WHEN N'ایمیل' THEN
            N'user' + CAST(pc.PersonId AS NVARCHAR(20)) + N'@fake.local'

        WHEN N'تلفن ثابت' THEN
            N'021' + RIGHT('00000000' + CAST(pc.PersonId % 100000000 AS VARCHAR(8)), 8)

        WHEN N'فکس' THEN
            N'021' + RIGHT('00000000' + CAST((pc.PersonId + 5000) % 100000000 AS VARCHAR(8)), 8)

        WHEN N'وب‌سایت' THEN
            N'https://example.com/u/' + CAST(pc.PersonId AS NVARCHAR(20))

        WHEN N'آدرس پستی' THEN
            N'تهران، خیابان تست، پلاک ' + CAST((pc.PersonId % 999) + 1 AS NVARCHAR(10))

        ELSE
            N'VAL-' + CAST(pc.PersonId AS NVARCHAR(20)) + N'-' + CAST(pc.ContactTypeId AS NVARCHAR(20))
    END
FROM PersonContacts pc
JOIN ContactTypes ct ON ct.Id = pc.ContactTypeId
WHERE pc.Description LIKE N'FAKE-CONTACT-%';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
