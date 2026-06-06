using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class FixPersonCategoryTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

UPDATE PersonCategories
SET Title = N'مشتری - زیر دسته 1'
WHERE PersonTypeId = 1 AND Title = N'زیر دسته 1';

UPDATE PersonCategories
SET Title = N'مشتری - زیر دسته 2'
WHERE PersonTypeId = 1 AND Title = N'زیر دسته 2';

UPDATE PersonCategories
SET Title = N'مشتری - زیر دسته 3'
WHERE PersonTypeId = 1 AND Title = N'زیر دسته 3';

UPDATE PersonCategories
SET Title = N'مشتری - زیر دسته 4'
WHERE PersonTypeId = 1 AND Title = N'زیر دسته 4';

UPDATE PersonCategories
SET Title = N'مشتری - زیر دسته 5'
WHERE PersonTypeId = 1 AND Title = N'زیر دسته 5';

UPDATE PersonCategories
SET Title = N'مشتری - سطح سوم 1'
WHERE PersonTypeId = 1 AND Title = N'سطح سوم 1';

UPDATE PersonCategories
SET Title = N'مشتری - سطح سوم 2'
WHERE PersonTypeId = 1 AND Title = N'سطح سوم 2';


UPDATE PersonCategories
SET Title = N'پرسنل - زیر دسته 1'
WHERE PersonTypeId = 2 AND Title = N'زیر دسته 1';

UPDATE PersonCategories
SET Title = N'پرسنل - زیر دسته 2'
WHERE PersonTypeId = 2 AND Title = N'زیر دسته 2';

UPDATE PersonCategories
SET Title = N'پرسنل - زیر دسته 3'
WHERE PersonTypeId = 2 AND Title = N'زیر دسته 3';

UPDATE PersonCategories
SET Title = N'پرسنل - زیر دسته 4'
WHERE PersonTypeId = 2 AND Title = N'زیر دسته 4';

UPDATE PersonCategories
SET Title = N'پرسنل - زیر دسته 5'
WHERE PersonTypeId = 2 AND Title = N'زیر دسته 5';

UPDATE PersonCategories
SET Title = N'پرسنل - سطح سوم 1'
WHERE PersonTypeId = 2 AND Title = N'سطح سوم 1';

UPDATE PersonCategories
SET Title = N'پرسنل - سطح سوم 2'
WHERE PersonTypeId = 2 AND Title = N'سطح سوم 2';


UPDATE PersonCategories
SET Title = N'تامین کننده - زیر دسته 1'
WHERE PersonTypeId = 3 AND Title = N'زیر دسته 1';

UPDATE PersonCategories
SET Title = N'تامین کننده - زیر دسته 2'
WHERE PersonTypeId = 3 AND Title = N'زیر دسته 2';

UPDATE PersonCategories
SET Title = N'تامین کننده - زیر دسته 3'
WHERE PersonTypeId = 3 AND Title = N'زیر دسته 3';

UPDATE PersonCategories
SET Title = N'تامین کننده - زیر دسته 4'
WHERE PersonTypeId = 3 AND Title = N'زیر دسته 4';

UPDATE PersonCategories
SET Title = N'تامین کننده - زیر دسته 5'
WHERE PersonTypeId = 3 AND Title = N'زیر دسته 5';

UPDATE PersonCategories
SET Title = N'تامین کننده - سطح سوم 1'
WHERE PersonTypeId = 3 AND Title = N'سطح سوم 1';

UPDATE PersonCategories
SET Title = N'تامین کننده - سطح سوم 2'
WHERE PersonTypeId = 3 AND Title = N'سطح سوم 2';


UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - زیر دسته 1'
WHERE PersonTypeId = 4 AND Title = N'زیر دسته 1';

UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - زیر دسته 2'
WHERE PersonTypeId = 4 AND Title = N'زیر دسته 2';

UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - زیر دسته 3'
WHERE PersonTypeId = 4 AND Title = N'زیر دسته 3';

UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - زیر دسته 4'
WHERE PersonTypeId = 4 AND Title = N'زیر دسته 4';

UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - زیر دسته 5'
WHERE PersonTypeId = 4 AND Title = N'زیر دسته 5';

UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - سطح سوم 1'
WHERE PersonTypeId = 4 AND Title = N'سطح سوم 1';

UPDATE PersonCategories
SET Title = N'مشتری و تامین کننده - سطح سوم 2'
WHERE PersonTypeId = 4 AND Title = N'سطح سوم 2';

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

UPDATE PersonCategories
SET Title = REPLACE(Title, N'مشتری - ', N'')
WHERE PersonTypeId = 1;

UPDATE PersonCategories
SET Title = REPLACE(Title, N'پرسنل - ', N'')
WHERE PersonTypeId = 2;

UPDATE PersonCategories
SET Title = REPLACE(Title, N'تامین کننده - ', N'')
WHERE PersonTypeId = 3;

UPDATE PersonCategories
SET Title = REPLACE(Title, N'مشتری و تامین کننده - ', N'')
WHERE PersonTypeId = 4;

");
        }
    }
}
