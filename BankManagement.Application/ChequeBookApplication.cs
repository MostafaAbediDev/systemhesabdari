using _0_Framework.Application;
using BankManagement.Application.Contracts.ChequeBook;
using BankManagement.Domain.Bank.ChequeBookAgg;

namespace BankManagement.Application
{
    public class ChequeBookApplication : IChequeBookApplication
    {
        private readonly IChequeBookRepository _chequeBookRepository;

        public ChequeBookApplication(IChequeBookRepository chequeBookRepository)
        {
            _chequeBookRepository = chequeBookRepository;
        }

        public OperationResult ChangeStatus(ChangeChequeBookStatus command)
        {
            var operation = new OperationResult();

            var cheque = _chequeBookRepository.Get(command.Id);

            if (cheque == null)
                return operation.Failed("چک یافت نشد.");

            cheque.ChangeStatus((ChequeBookStatus)command.Status);

            _chequeBookRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Create(CreateChequeBook command)
        {
            var operation = new OperationResult();

            if (_chequeBookRepository.Exists(x => x.SerialNumber == command.SerialNumber))
                return operation.Failed("شماره سریال تکراری است.");

            var chequeBook = new ChequeBooks(
                command.ChequeCount,
                command.FirstChequeCode,
                command.LastChequeCode,
                command.SerialNumber,
                command.ReceiveDate,
                command.CompanyBankAccountId);

            _chequeBookRepository.Create(chequeBook);
            _chequeBookRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditChequeBook command)
        {
            var operation = new OperationResult();

            var chequeBook = _chequeBookRepository.Get(command.Id);

            if (chequeBook == null)
                return operation.Failed("دسته چک مورد نظر یافت نشد.");

            if (_chequeBookRepository.Exists(x =>
                x.SerialNumber == command.SerialNumber &&
                x.Id != command.Id))
            {
                return operation.Failed("شماره سریال تکراری است.");
            }

            chequeBook.Edit(
                command.ChequeCount,
                command.FirstChequeCode,
                command.LastChequeCode,
                command.SerialNumber,
                command.ReceiveDate,
                command.CompanyBankAccountId);

            _chequeBookRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<ChequeBookViewModel> GetChequeBooks()
        {
            return _chequeBookRepository.GetChequeBooks();
        }

        public EditChequeBook GetDetails(long id)
        {
            return _chequeBookRepository.GetDetails(id);
        }

        public List<ChequeBookViewModel> Search(ChequeBookSearchModel searchModel)
        {
            return _chequeBookRepository.Search(searchModel);
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var person = _chequeBookRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Remove();
            _chequeBookRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var person = _chequeBookRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Restore();
            _chequeBookRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var person = _chequeBookRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Active();
            _chequeBookRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var person = _chequeBookRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.NotActive();
            _chequeBookRepository.SaveChanges();
            return operation.Succedded();
        }
    }
}
