using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Fix_AccountingEntries_ExtraDocumentFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop ForeignKey if exists
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingEntries_AccountingDocuments_AccountingDocumentsId",
                table: "AccountingEntries");

            // Drop Index if exists
            migrationBuilder.DropIndex(
                name: "IX_AccountingEntries_AccountingDocumentsId",
                table: "AccountingEntries");

            // Drop Column if exists
            migrationBuilder.DropColumn(
                name: "AccountingDocumentsId",
                table: "AccountingEntries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate Column
            migrationBuilder.AddColumn<Guid>(
                name: "AccountingDocumentsId",
                table: "AccountingEntries",
                type: "uniqueidentifier",
                nullable: true);

            // Recreate Index
            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_AccountingDocumentsId",
                table: "AccountingEntries",
                column: "AccountingDocumentsId");

            // Recreate FK
            migrationBuilder.AddForeignKey(
                name: "FK_AccountingEntries_AccountingDocuments_AccountingDocumentsId",
                table: "AccountingEntries",
                column: "AccountingDocumentsId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id");
        }
    }
}
