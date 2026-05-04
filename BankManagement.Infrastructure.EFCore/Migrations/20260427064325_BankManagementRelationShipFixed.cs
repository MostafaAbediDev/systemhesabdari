using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class BankManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountingDocumentId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ChequeId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CompanyBankAccountId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FinancialPeriodId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FundId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "ReceiptsPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReceiptPaymentId",
                table: "ReceiptPaymentInvoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "PettyCashes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "PettyCashes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "HolderPersonId",
                table: "PettyCashes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ResponsiblePersonId",
                table: "PettyCashes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SettlementAccountId",
                table: "PettyCashes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Funds",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Funds",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "CompanyBankAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BankId",
                table: "CompanyBankAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "CompanyBankAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Cheques",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ChequeBookId",
                table: "Cheques",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BankId",
                table: "ChequeBooks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PictureId",
                table: "Banks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_AccountingDocumentId",
                table: "ReceiptsPayments",
                column: "AccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_BranchId",
                table: "ReceiptsPayments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_ChequeId",
                table: "ReceiptsPayments",
                column: "ChequeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_CompanyBankAccountId",
                table: "ReceiptsPayments",
                column: "CompanyBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_FinancialPeriodId",
                table: "ReceiptsPayments",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_FundId",
                table: "ReceiptsPayments",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsPayments_PersonId",
                table: "ReceiptsPayments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptPaymentInvoices_ReceiptPaymentId",
                table: "ReceiptPaymentInvoices",
                column: "ReceiptPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_AccountId",
                table: "PettyCashes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_BranchId",
                table: "PettyCashes",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_ResponsiblePersonId",
                table: "PettyCashes",
                column: "ResponsiblePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_HolderPersonId",
                table: "PettyCashes",
                column: "HolderPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_AccountId",
                table: "Funds",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_BranchId",
                table: "Funds",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_AccountId",
                table: "CompanyBankAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_BankId",
                table: "CompanyBankAccounts",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBankAccounts_BranchId",
                table: "CompanyBankAccounts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_BranchId",
                table: "Cheques",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_ChequeBookId",
                table: "Cheques",
                column: "ChequeBookId");

            migrationBuilder.CreateIndex(
                name: "IX_ChequeBooks_BankId",
                table: "ChequeBooks",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_PictureId",
                table: "Banks",
                column: "PictureId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_Pictures_PictureId",
                table: "Banks",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChequeBooks_Banks_BankId",
                table: "ChequeBooks",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheques_Branches_BranchId",
                table: "Cheques",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheques_ChequeBooks_ChequeBookId",
                table: "Cheques",
                column: "ChequeBookId",
                principalTable: "ChequeBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBankAccounts_Accounts_AccountId",
                table: "CompanyBankAccounts",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBankAccounts_Banks_BankId",
                table: "CompanyBankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBankAccounts_Branches_BranchId",
                table: "CompanyBankAccounts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Funds_Accounts_AccountId",
                table: "Funds",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Funds_Branches_BranchId",
                table: "Funds",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PettyCashes_Accounts_AccountId",
                table: "PettyCashes",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PettyCashes_Branches_BranchId",
                table: "PettyCashes",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PettyCashes_Persons_ResponsiblePersonId",
                table: "PettyCashes",
                column: "ResponsiblePersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PettyCashes_Persons_HolderPersonId",
                table: "PettyCashes",
                column: "HolderPersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptPaymentInvoices_ReceiptsPayments_ReceiptPaymentId",
                table: "ReceiptPaymentInvoices",
                column: "ReceiptPaymentId",
                principalTable: "ReceiptsPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_AccountingDocuments_AccountingDocumentId",
                table: "ReceiptsPayments",
                column: "AccountingDocumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_Branches_BranchId",
                table: "ReceiptsPayments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_Cheques_ChequeId",
                table: "ReceiptsPayments",
                column: "ChequeId",
                principalTable: "Cheques",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_CompanyBankAccounts_CompanyBankAccountId",
                table: "ReceiptsPayments",
                column: "CompanyBankAccountId",
                principalTable: "CompanyBankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_FinancialPeriods_FinancialPeriodId",
                table: "ReceiptsPayments",
                column: "FinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_Funds_FundId",
                table: "ReceiptsPayments",
                column: "FundId",
                principalTable: "Funds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptsPayments_Persons_PersonId",
                table: "ReceiptsPayments",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_Pictures_PictureId",
                table: "Banks");

            migrationBuilder.DropForeignKey(
                name: "FK_ChequeBooks_Banks_BankId",
                table: "ChequeBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Cheques_Branches_BranchId",
                table: "Cheques");

            migrationBuilder.DropForeignKey(
                name: "FK_Cheques_ChequeBooks_ChequeBookId",
                table: "Cheques");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBankAccounts_Accounts_AccountId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBankAccounts_Banks_BankId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBankAccounts_Branches_BranchId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Funds_Accounts_AccountId",
                table: "Funds");

            migrationBuilder.DropForeignKey(
                name: "FK_Funds_Branches_BranchId",
                table: "Funds");

            migrationBuilder.DropForeignKey(
                name: "FK_PettyCashes_Accounts_AccountId",
                table: "PettyCashes");

            migrationBuilder.DropForeignKey(
                name: "FK_PettyCashes_Branches_BranchId",
                table: "PettyCashes");

            migrationBuilder.DropForeignKey(
                name: "FK_PettyCashes_Persons_ResponsiblePersonId",
                table: "PettyCashes");

            migrationBuilder.DropForeignKey(
                name: "FK_PettyCashes_Persons_HolderPersonId",
                table: "PettyCashes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptPaymentInvoices_ReceiptsPayments_ReceiptPaymentId",
                table: "ReceiptPaymentInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_AccountingDocuments_AccountingDocumentId",
                table: "ReceiptsPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_Branches_BranchId",
                table: "ReceiptsPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_Cheques_ChequeId",
                table: "ReceiptsPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_CompanyBankAccounts_CompanyBankAccountId",
                table: "ReceiptsPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_FinancialPeriods_FinancialPeriodId",
                table: "ReceiptsPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_Funds_FundId",
                table: "ReceiptsPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptsPayments_Persons_PersonId",
                table: "ReceiptsPayments");

            

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_AccountingDocumentId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_BranchId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_ChequeId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_CompanyBankAccountId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_FinancialPeriodId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_FundId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsPayments_PersonId",
                table: "ReceiptsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptPaymentInvoices_ReceiptPaymentId",
                table: "ReceiptPaymentInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashes_AccountId",
                table: "PettyCashes");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashes_BranchId",
                table: "PettyCashes");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashes_ResponsiblePersonId",
                table: "PettyCashes");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashes_HolderPersonId",
                table: "PettyCashes");

            migrationBuilder.DropIndex(
                name: "IX_Funds_AccountId",
                table: "Funds");

            migrationBuilder.DropIndex(
                name: "IX_Funds_BranchId",
                table: "Funds");

            migrationBuilder.DropIndex(
                name: "IX_CompanyBankAccounts_AccountId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CompanyBankAccounts_BankId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CompanyBankAccounts_BranchId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Cheques_BranchId",
                table: "Cheques");

            migrationBuilder.DropIndex(
                name: "IX_Cheques_ChequeBookId",
                table: "Cheques");

            migrationBuilder.DropIndex(
                name: "IX_ChequeBooks_BankId",
                table: "ChequeBooks");

            migrationBuilder.DropIndex(
                name: "IX_Banks_PictureId",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "AccountingDocumentId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "ChequeId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "CompanyBankAccountId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "FinancialPeriodId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "FundId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "ReceiptsPayments");

            migrationBuilder.DropColumn(
                name: "ReceiptPaymentId",
                table: "ReceiptPaymentInvoices");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "PettyCashes");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "PettyCashes");

            migrationBuilder.DropColumn(
                name: "HolderPersonId",
                table: "PettyCashes");

            migrationBuilder.DropColumn(
                name: "ResponsiblePersonId",
                table: "PettyCashes");

            migrationBuilder.DropColumn(
                name: "SettlementAccountId",
                table: "PettyCashes");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "CompanyBankAccounts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ChequeBookId",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "ChequeBooks");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Banks");
        }
    }
}
