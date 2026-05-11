using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init_Log : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ClientDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
