using _0_Framework.Application;
using PersonManagement.Application.Contract.PersonTypes;
using PersonManagement.Domain.Person.PersonTypeAgg;

namespace PersonManagement.Application
{
    public class PersonTypeApplication : IPersonTypeApplication
    {
        private readonly IPersonTypeRepository _personTypeRepository;

        public PersonTypeApplication(IPersonTypeRepository personTypeRepository)
        {
            _personTypeRepository = personTypeRepository;
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var person = _personTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Remove();
            _personTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var person = _personTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Restore();
            _personTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();
            var person = _personTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.Active();
            _personTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();
            var person = _personTypeRepository.Get(id);
            if (person == null)
                return operation.Failed("رکورد یافت نشد");

            person.NotActive();
            _personTypeRepository.SaveChanges();
            return operation.Succedded();
        }
        public List<PersonTypeViewModel> GetPersonTypes()
        {
            return _personTypeRepository.GetPersonTypes();
        }
    }
}
