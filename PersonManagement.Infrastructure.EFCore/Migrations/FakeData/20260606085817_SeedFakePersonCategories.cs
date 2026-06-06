using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakePersonCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

IF EXISTS (SELECT 1 FROM PersonTypes WHERE ISNULL(IsDeleted,0)=0)
AND NOT EXISTS (SELECT 1 FROM PersonCategories)
BEGIN

    DECLARE @PersonTypeId BIGINT;
    DECLARE @PersonTypeTitle NVARCHAR(200);

    DECLARE PersonTypeCursor CURSOR FOR
    SELECT Id, Title
    FROM PersonTypes
    WHERE ISNULL(IsDeleted,0)=0;

    OPEN PersonTypeCursor;

    FETCH NEXT FROM PersonTypeCursor
    INTO @PersonTypeId, @PersonTypeTitle;

    WHILE @@FETCH_STATUS = 0
    BEGIN

        DECLARE @Root1 BIGINT;
        DECLARE @Root2 BIGINT;
        DECLARE @Root3 BIGINT;

        ------------------------------------
        -- سطح اول
        ------------------------------------

        INSERT INTO PersonCategories
        (
            Title,
            PersonTypeId,
            ParentId,
            IsDeleted,
            IsActive,
            CreationDate
        )
        VALUES
        (@PersonTypeTitle + N' دسته 1', @PersonTypeId, NULL, 0, 1, GETDATE());

        SET @Root1 = SCOPE_IDENTITY();

        INSERT INTO PersonCategories
        (
            Title,
            PersonTypeId,
            ParentId,
            IsDeleted,
            IsActive,
            CreationDate
        )
        VALUES
        (@PersonTypeTitle + N' دسته 2', @PersonTypeId, NULL, 0, 1, GETDATE());

        SET @Root2 = SCOPE_IDENTITY();

        INSERT INTO PersonCategories
        (
            Title,
            PersonTypeId,
            ParentId,
            IsDeleted,
            IsActive,
            CreationDate
        )
        VALUES
        (@PersonTypeTitle + N' دسته 3', @PersonTypeId, NULL, 0, 1, GETDATE());

        SET @Root3 = SCOPE_IDENTITY();

        ------------------------------------
        -- سطح دوم
        ------------------------------------

        INSERT INTO PersonCategories
        (
            Title,
            PersonTypeId,
            ParentId,
            IsDeleted,
            IsActive,
            CreationDate
        )
        VALUES
        (N'زیر دسته 1', @PersonTypeId, @Root1, 0, 1, GETDATE()),
        (N'زیر دسته 2', @PersonTypeId, @Root1, 0, 1, GETDATE()),
        (N'زیر دسته 3', @PersonTypeId, @Root1, 0, 1, GETDATE());

        INSERT INTO PersonCategories
        (
            Title,
            PersonTypeId,
            ParentId,
            IsDeleted,
            IsActive,
            CreationDate
        )
        VALUES
        (N'زیر دسته 4', @PersonTypeId, @Root2, 0, 1, GETDATE()),
        (N'زیر دسته 5', @PersonTypeId, @Root2, 0, 1, GETDATE());

        ------------------------------------
        -- سطح سوم
        ------------------------------------

        DECLARE @ChildId BIGINT;

        SELECT TOP 1 @ChildId = Id
        FROM PersonCategories
        WHERE ParentId = @Root1
        ORDER BY Id;

        INSERT INTO PersonCategories
        (
            Title,
            PersonTypeId,
            ParentId,
            IsDeleted,
            IsActive,
            CreationDate
        )
        VALUES
        (N'سطح سوم 1', @PersonTypeId, @ChildId, 0, 1, GETDATE()),
        (N'سطح سوم 2', @PersonTypeId, @ChildId, 0, 1, GETDATE());

        FETCH NEXT FROM PersonTypeCursor
        INTO @PersonTypeId, @PersonTypeTitle;

    END

    CLOSE PersonTypeCursor;
    DEALLOCATE PersonTypeCursor;

END

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

DELETE PC
FROM PersonCategories PC
INNER JOIN PersonTypes PT
    ON PT.Id = PC.PersonTypeId
WHERE
    PC.Title LIKE N'%دسته %'
    OR PC.Title LIKE N'زیر دسته %'
    OR PC.Title LIKE N'سطح سوم %'

");
        }
    }
}
