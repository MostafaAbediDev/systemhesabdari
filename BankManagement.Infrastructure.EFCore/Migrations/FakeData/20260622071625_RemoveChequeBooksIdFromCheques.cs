using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class RemoveChequeBooksIdFromCheques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // اگر ایندکس یا FK دارد اول باید حذف شود
            migrationBuilder.DropForeignKey(
                name: "FK_Cheques_ChequeBooks_ChequeBooksId",
                table: "Cheques");

            migrationBuilder.DropIndex(
                name: "IX_Cheques_ChequeBooksId",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ChequeBooksId",
                table: "Cheques");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChequeBooksId",
                table: "Cheques",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_ChequeBooksId",
                table: "Cheques",
                column: "ChequeBooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheques_ChequeBooks_ChequeBooksId",
                table: "Cheques",
                column: "ChequeBooksId",
                principalTable: "ChequeBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
