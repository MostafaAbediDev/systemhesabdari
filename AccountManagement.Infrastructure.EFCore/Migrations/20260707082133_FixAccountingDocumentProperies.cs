using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class FixAccountingDocumentProperies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AlterColumn<long>(
                name: "ReferenceId",
                table: "AccountingDocuments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DocumentNumber",
                table: "AccountingDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DocumentDate",
                table: "AccountingDocuments",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "ApprovedBy",
                table: "AccountingDocuments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_ApprovedBy",
                table: "AccountingDocuments",
                column: "ApprovedBy");

           
            migrationBuilder.AddForeignKey(
                name: "FK_AccountingDocuments_Persons_ApprovedBy",
                table: "AccountingDocuments",
                column: "ApprovedBy",
                principalTable: "Persons",
                principalColumn: "Id");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingDocuments_Persons_ApprovedBy",
                table: "AccountingDocuments");

            
            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_ApprovedBy",
                table: "AccountingDocuments");

           
            migrationBuilder.AlterColumn<int>(
                name: "ReferenceId",
                table: "AccountingDocuments",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DocumentNumber",
                table: "AccountingDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DocumentDate",
                table: "AccountingDocuments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedBy",
                table: "AccountingDocuments",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

        }
    }
}
