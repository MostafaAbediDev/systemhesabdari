using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class SeedFakeData500Records : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EstablishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NationalId = table.Column<int>(type: "int", nullable: false),
                    EconomicCode = table.Column<int>(type: "int", nullable: false),
                    RegisterNumber = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Lat_Log = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProvinceId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchArchive",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    File = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchArchive", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchArchive_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialPeriods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialPeriods_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchArchive_BranchId",
                table: "BranchArchive",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompanyId",
                table: "Branches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_BranchId",
                table: "FinancialPeriods",
                column: "BranchId");

            migrationBuilder.Sql(@"

-- =========================================
-- Provinces
-- =========================================
INSERT INTO Provinces
(Id, Title, IsDeleted, IsActive, CreationDate)
VALUES
(1, N'تهران', 0, 1, GETDATE()),
(2, N'اصفهان', 0, 1, GETDATE()),
(3, N'فارس', 0, 1, GETDATE()),
(4, N'خراسان رضوی', 0, 1, GETDATE()),
(5, N'گیلان', 0, 1, GETDATE());



-- =========================================
-- Cities
-- =========================================
INSERT INTO Cities
(Id, Title, ProvinceId, IsDeleted, IsActive, CreationDate)
VALUES
(1, N'تهران', 1, 0, 1, GETDATE()),
(2, N'ری', 1, 0, 1, GETDATE()),
(3, N'اصفهان', 2, 0, 1, GETDATE()),
(4, N'شیراز', 3, 0, 1, GETDATE()),
(5, N'مشهد', 4, 0, 1, GETDATE());



-- =========================================
-- Companies (500)
-- =========================================
DECLARE @i INT = 1;

WHILE @i <= 500
BEGIN

    INSERT INTO Companies
    (
        Title,
        Logo,
        LegalName,
        EstablishedDate,
        IsDeleted,
        IsActive,
        CreationDate
    )
    VALUES
    (
        CONCAT(N'شرکت ', @i),
        CONCAT('/logos/', @i, '.png'),
        CONCAT(N'شرکت حقوقی ', @i),
        GETDATE(),
        0,
        1,
        GETDATE()
    );

    SET @i = @i + 1;
END;



-- =========================================
-- Branches (500)
-- =========================================
SET @i = 1;

WHILE @i <= 500
BEGIN

    INSERT INTO Branches
    (
        Title,
        NationalId,
        EconomicCode,
        RegisterNumber,
        Code,
        Email,
        Phone,
        Lat_Log,
        Address,
        PostCode,
        IsMain,
        CompanyId,
        IsDeleted,
        IsActive,
        CreationDate
    )
    VALUES
    (
        CONCAT(N'شعبه ', @i),
        100000000 + @i,
        200000000 + @i,
        300000 + @i,
        CONCAT('BR-', @i),
        CONCAT('branch', @i, '@test.com'),
        '09120000000',
        '35.6892,51.3890',
        CONCAT(N'آدرس تست ', @i),
        RIGHT(CONCAT('0000000000', @i), 10),
        CASE WHEN @i = 1 THEN 1 ELSE 0 END,
        @i,
        0,
        1,
        GETDATE()
    );

    SET @i = @i + 1;
END;



-- =========================================
-- FinancialPeriods
-- =========================================
SET @i = 1;

WHILE @i <= 500
BEGIN

    INSERT INTO FinancialPeriods
    (
        Title,
        StartDate,
        EndDate,
        BranchId,
        IsDeleted,
        IsActive,
        CreationDate
    )
    VALUES
    (
        CONCAT(N'سال مالی ', @i),
        DATEADD(YEAR, -1, GETDATE()),
        GETDATE(),
        @i,
        0,
        1,
        GETDATE()
    );

    SET @i = @i + 1;
END;



-- =========================================
-- BranchArchive
-- =========================================
SET @i = 1;

WHILE @i <= 500
BEGIN

    INSERT INTO BranchArchive
    (
        Title,
        Description,
        [File],
        BranchId,
        IsDeleted,
        IsActive,
        CreationDate
    )
    VALUES
    (
        CONCAT(N'آرشیو ', @i),
        CONCAT(N'توضیحات آرشیو ', @i),
        CONCAT('/files/archive-', @i, '.pdf'),
        @i,
        0,
        1,
        GETDATE()
    );

    SET @i = @i + 1;
END;



-- =========================================
-- Pictures
-- =========================================
SET @i = 1;

WHILE @i <= 500
BEGIN

    INSERT INTO Pictures
    (
        EntityType,
        EntityId,
        ImageUrl,
        IsMain,
        IsDeleted,
        IsActive,
        CreationDate
    )
    VALUES
    (
        'Branch',
        @i,
        CONCAT('/images/', @i, '.jpg'),
        1,
        0,
        1,
        GETDATE()
    );

    SET @i = @i + 1;
END;

");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DELETE FROM Pictures;");
            migrationBuilder.Sql("DELETE FROM BranchArchive;");
            migrationBuilder.Sql("DELETE FROM FinancialPeriods;");
            migrationBuilder.Sql("DELETE FROM Branches;");
            migrationBuilder.Sql("DELETE FROM Companies;");
            migrationBuilder.Sql("DELETE FROM Cities;");
            migrationBuilder.Sql("DELETE FROM Provinces;");


            migrationBuilder.DropTable(
                name: "BranchArchive");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "FinancialPeriods");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
