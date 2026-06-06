using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class PersonCategoriesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonCategories_PersonCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PersonCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonCategories_PersonTypes_PersonTypeId",
                        column: x => x.PersonTypeId,
                        principalTable: "PersonTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PersonTypes",
                columns: new[] { "Id", "CreationDate", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Title", "TitleId" },
                values: new object[] { 4L, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "مشتری و تامین کننده", 4 });

            migrationBuilder.CreateIndex(
                name: "IX_PersonCategories_ParentId",
                table: "PersonCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonCategories_PersonTypeId",
                table: "PersonCategories",
                column: "PersonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonCategories_Title",
                table: "PersonCategories",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonCategories");

            migrationBuilder.DeleteData(
                table: "PersonTypes",
                keyColumn: "Id",
                keyValue: 4L);
        }
    }
}
