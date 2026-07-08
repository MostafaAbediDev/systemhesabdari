using _0_Framework.Application;
using FinancialManagement.Application.Contracts.ReceiptsPayment;
using FinancialManagement.Domain.ReceiptsPaymentAgg;

namespace FinancialManagement.Application
{
    public class ReceiptsPaymentApplication : IReceiptsPaymentApplication
    {
        private readonly IReceiptsPaymentRepository _receiptsPaymentRepository;

        public ReceiptsPaymentApplication(IReceiptsPaymentRepository receiptsPaymentRepository)
        {
            _receiptsPaymentRepository = receiptsPaymentRepository;
        }

        public OperationResult Create(CreateReceiptsPayment command)
        {
            var operation = new OperationResult();


            if (string.IsNullOrWhiteSpace(command.Description))
                return operation.Failed("شرح تراکنش الزامی است");


            if (command.Amount <= 0)
                return operation.Failed("مبلغ باید بیشتر از صفر باشد");



            if (!Enum.IsDefined(typeof(ReceiptPaymentTypeDTO), command.Type))
                return operation.Failed("نوع تراکنش نامعتبر است");


            if (!Enum.IsDefined(typeof(PaymentMethodDTO), command.PaymentMethod))
                return operation.Failed("روش پرداخت نامعتبر است");



            var receiptsPayment = new ReceiptsPayments(
                command.Description,
                command.Amount,
                (ReceiptPaymentType)command.Type,
                (PaymentMethod)command.PaymentMethod,
                command.BranchId,
                command.FinancialPeriodId
            );


            _receiptsPaymentRepository.Create(receiptsPayment);

            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }


        public OperationResult Edit(EditReceiptsPayment command)
        {
            var operation = new OperationResult();


            var receiptsPayment = _receiptsPaymentRepository.Get(command.Id);


            if (receiptsPayment == null)
                return operation.Failed("تراکنش یافت نشد");



            if (string.IsNullOrWhiteSpace(command.Description))
                return operation.Failed("شرح تراکنش الزامی است");


            if (command.Amount <= 0)
                return operation.Failed("مبلغ باید بیشتر از صفر باشد");



            receiptsPayment.Edit(
                command.Description,
                command.Amount,
                (ReceiptPaymentType)command.Type,
                (PaymentMethod)command.PaymentMethod
            );



            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }


        public OperationResult LinkToPerson(LinkToPersonDTO command)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(command.Id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.LinkToPerson(command.PersonId);


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }

        public OperationResult LinkToFund(LinkToFundDTO command)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(command.Id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.LinkToFund(command.FundId);


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }


        public OperationResult LinkToBankAccount(LinkToBankAccountDTO command)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(command.Id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.LinkToBankAccount(command.CompanyBankAccountId);


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }

        public OperationResult LinkToCheque(LinkToChequeDTO command)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(command.Id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.LinkToCheque(command.ChequeId);


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }


        public OperationResult LinkToAccountingDocument(LinkToAccountingDocumentDTO command)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(command.Id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.LinkToAccountingDocument(command.AccountingDocumentId);


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.Remove();


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }


        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.Restore();


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.Active();


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();


            var entity = _receiptsPaymentRepository.Get(id);


            if (entity == null)
                return operation.Failed("تراکنش یافت نشد");


            entity.NotActive();


            _receiptsPaymentRepository.SaveChanges();


            return operation.Succedded();
        }

        public EditReceiptsPayment GetDetails(long id)
        {
            return _receiptsPaymentRepository.GetDetails(id);
        }

        public List<ReceiptsPaymentViewModel> GetReceiptsPayments()
        {
            return _receiptsPaymentRepository.GetReceiptsPayments();
        }

        public List<ReceiptsPaymentViewModel> Search(ReceiptsPaymentSearchModel searchModel)
        {
            return _receiptsPaymentRepository.Search(searchModel);
        }
    }
}
