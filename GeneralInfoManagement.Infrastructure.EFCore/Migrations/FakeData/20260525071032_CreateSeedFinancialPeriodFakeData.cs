using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralInfoManagement.Infrastructure.EFCore.Migrations.FakeData
{
    /// <inheritdoc />
    public partial class CreateSeedFinancialPeriodFakeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @branchId INT = 1001;

                WHILE @branchId <= 1500
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
                        CONCAT(N'دوره مالی شعبه ', @branchId),
                        '2024-03-21',
                        '2025-03-20',
                        @branchId,
                        0,
                        1,
                        GETDATE()
                    );

                    SET @branchId = @branchId + 1;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM FinancialPeriods
                WHERE Title LIKE N'دوره مالی شعبه %'
                  AND BranchId BETWEEN 1001 AND 1500
            ");
        }
    }
}
