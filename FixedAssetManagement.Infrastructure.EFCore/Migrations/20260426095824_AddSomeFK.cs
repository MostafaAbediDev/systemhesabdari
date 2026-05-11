using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixedAssetManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Index for AccountAssetId
            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_AccountAssetId",
                table: "FixedAssets",
                column: "AccountAssetId");

            // Create Index for AccountDepreciationId
            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_AccountDepreciationId",
                table: "FixedAssets",
                column: "AccountDepreciationId");

            // Add FK for AccountAssetId
            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssets_Accounts_AccountAssetId",
                table: "FixedAssets",
                column: "AccountAssetId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Add FK for AccountDepreciationId
            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssets_Accounts_AccountDepreciationId",
                table: "FixedAssets",
                column: "AccountDepreciationId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssets_Accounts_AccountAssetId",
                table: "FixedAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssets_Accounts_AccountDepreciationId",
                table: "FixedAssets");

            migrationBuilder.DropIndex(
                name: "IX_FixedAssets_AccountAssetId",
                table: "FixedAssets");

            migrationBuilder.DropIndex(
                name: "IX_FixedAssets_AccountDepreciationId",
                table: "FixedAssets");
        }
    }
}
