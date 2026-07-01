using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class PersonCategoryIdAddedToPersons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PersonCategoryId",
                table: "Persons",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_PersonCategoryId",
                table: "Persons",
                column: "PersonCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_PersonCategories_PersonCategoryId",
                table: "Persons",
                column: "PersonCategoryId",
                principalTable: "PersonCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_PersonCategories_PersonCategoryId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_PersonCategoryId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "PersonCategoryId",
                table: "Persons");
        }
    }
}
