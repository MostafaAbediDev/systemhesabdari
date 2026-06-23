using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class FullNameSplitToFirstAndLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Persons",
                newName: "LastName");


            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Persons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Persons",
                newName: "FullName");

            
        }
    }
}
