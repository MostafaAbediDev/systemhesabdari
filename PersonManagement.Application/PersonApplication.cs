using _0_Framework.Application;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Application
{
    public class PersonApplication : IPersonApplication
    {
        private readonly IPersonRepository _personRepository;

        public PersonApplication(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public OperationResult Create(CreatePerson command)
        {
            var operation = new OperationResult();

            if (_personRepository.Exists(x => x.NationalCode == command.NationalCode))
                return operation.Failed("کد ملی تکراری است");

            var person = new Persons(
                command.Code,
                command.FullName,
                command.NationalCode,
                command.EconomicCode,
                command.RegistrationNumber,
                command.PersonType
            );

            // تنظیمات تکمیلی دامنه
            if (command.IsLegal)
                person.Legal();
            else
                person.IlLegal();

            person.SetBranch(command.BranchId);

            _personRepository.Create(person);
            _personRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditPerson command)
        {
            var operation = new OperationResult();

            var person = _personRepository.Get(command.Id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            if (_personRepository.Exists(x => x.NationalCode == command.NationalCode && x.Id != command.Id))
                return operation.Failed("کد ملی تکراری است");

            person.Edit(
                command.Code,
                command.FullName,
                command.NationalCode,
                command.EconomicCode,
                command.RegistrationNumber,
                command.PersonType
            );

            person.SetBranch(command.BranchId);

            if (command.IsLegal) person.Legal();
            else person.IlLegal();

            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var person = _personRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Remove();
            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var person = _personRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Restore();
            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var person = _personRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Active();
            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var person = _personRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.NotActive();
            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult MakeLegal(long id)
        {
            var operation = new OperationResult();
            var person = _personRepository.Get(id);

            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Legal();
            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult MakeIllegal(long id)
        {
            var operation = new OperationResult();
            var person = _personRepository.Get(id);

            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.IlLegal();
            _personRepository.SaveChanges();
            return operation.Succedded();
        }

        public EditPerson GetDetails(long id)
        {
            return _personRepository.GetDetails(id);
        }

        public PersonFullViewModel GetFullDetails(long id)
        {
            return _personRepository.GetFullDetails(id);
        }

        public List<PersonViewModel> GetPersons()
        {
            return _personRepository.GetAllPersons();
        }

        public List<PersonViewModel> Search(PersonSearchModel searchModel)
        {
            return _personRepository.Search(searchModel);
        }
    }
}
