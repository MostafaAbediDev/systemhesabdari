using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class AddPictureOwnerTypeInPictures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Pictures");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Pictures",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Pictures",
                newName: "OwnerId");

            migrationBuilder.AddColumn<int>(
                name: "OwnerType",
                table: "Pictures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_OwnerId_OwnerType",
                table: "Pictures",
                columns: new[] { "OwnerId", "OwnerType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pictures_OwnerId_OwnerType",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "Pictures");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Pictures",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Pictures",
                newName: "EntityId");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Pictures",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
