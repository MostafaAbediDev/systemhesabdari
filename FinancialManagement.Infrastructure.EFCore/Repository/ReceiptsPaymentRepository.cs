using _0_FrameWork.Infrastructure;
using FinancialManagement.Application.Contracts.ReceiptsPayment;
using FinancialManagement.Domain.ReceiptsPaymentAgg;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagement.Infrastructure.EFCore.Repository
{
    public class ReceiptsPaymentRepository : RepositoryBase<long, ReceiptsPayments>, IReceiptsPaymentRepository
    {
        private readonly FinancialFakeDataContext _context;

        public ReceiptsPaymentRepository(FinancialFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditReceiptsPayment GetDetails(long id)
        {
            return _context.ReceiptsPayments
                .Select(x => new EditReceiptsPayment
                {
                    Id = x.Id,
                    Description = x.Description,
                    Amount = x.Amount,
                    Type = (ReceiptPaymentTypeDTO)x.Type,
                    PaymentMethod = (PaymentMethodDTO)x.PaymentMethod,
                    BranchId = x.BranchId,
                    FinancialPeriodId = x.FinancialPeriodId
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<ReceiptsPaymentViewModel> GetReceiptsPayments()
        {
            return _context.ReceiptsPayments
        .Include(x => x.Branches)
        .Include(x => x.Persons)
        .Include(x => x.Funds)
        .Include(x => x.CompanyBankAccounts)

        .Select(x => new ReceiptsPaymentViewModel
        {
            Id = x.Id,
            Description = x.Description,
            Amount = x.Amount,

            Type = x.Type == ReceiptPaymentType.Receipt
                ? "دریافت"
                : "پرداخت",

            PaymentMethod = x.PaymentMethod == PaymentMethod.Cash ? "نقدی"
                : x.PaymentMethod == PaymentMethod.BankTransfer ? "انتقال بانکی"
                : x.PaymentMethod == PaymentMethod.Cheque ? "چک"
                : "تنخواه",

            TransactionDate = x.TransactionDate,

            BranchName = x.Branches.Title,

            FinancialPeriodId = x.FinancialPeriodId,

            PersonName = x.Persons != null
                ? $"{x.Persons.FirstName ?? ""} {x.Persons.LastName ?? ""}".Trim()
                : "",

            FundTitle = x.Funds != null
                ? x.Funds.Title
                : "",

            BankAccountTitle = x.CompanyBankAccounts != null
                ? x.CompanyBankAccounts.AccountTitle
                : "",

            ChequeId = x.ChequeId,

            AccountingDocumentId = x.AccountingDocumentId,

            CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
        })
        .OrderByDescending(x => x.TransactionDate)
        .ToList();
        }

        public List<ReceiptsPaymentViewModel> Search(ReceiptsPaymentSearchModel searchModel)
        {
            var query = _context.ReceiptsPayments
                .Include(x => x.Branches)
                .Include(x => x.Persons)
                .Include(x => x.Funds)
                .Include(x => x.CompanyBankAccounts)
                .Select(x => new ReceiptsPaymentViewModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Amount = x.Amount,

                    Type = x.Type == ReceiptPaymentType.Receipt
                ? "دریافت"
                : "پرداخت",

                    PaymentMethod = x.PaymentMethod == PaymentMethod.Cash ? "نقدی"
                : x.PaymentMethod == PaymentMethod.BankTransfer ? "انتقال بانکی"
                : x.PaymentMethod == PaymentMethod.Cheque ? "چک"
                : "تنخواه",

                    TransactionDate = x.TransactionDate,

                    BranchName = x.Branches.Title,

                    FinancialPeriodId = x.FinancialPeriodId,

                    PersonName = x.Persons != null
                        ? $"{x.Persons.FirstName ?? ""} {x.Persons.LastName ?? ""}".Trim()
                        : "",

                    FundTitle = x.Funds != null
                        ? x.Funds.Title
                        : "",

                    BankAccountTitle = x.CompanyBankAccounts != null
                        ? x.CompanyBankAccounts.AccountTitle
                        : "",

                    ChequeId = x.ChequeId,

                    AccountingDocumentId = x.AccountingDocumentId,

                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                });

            if (searchModel.Amount > 0)
                query = query.Where(x => x.Amount == searchModel.Amount);

            if (searchModel.Type != 0)
            {
                var type = searchModel.Type == ReceiptPaymentTypeDTO.Receipt
                    ? "دریافت"
                    : "پرداخت";

                query = query.Where(x => x.Type == type);
            }

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
