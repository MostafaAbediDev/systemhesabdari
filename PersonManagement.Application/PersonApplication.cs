using _0_Framework.Application;
using CodeManagement.Application.Contracts.Code;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Application
{
    public class PersonApplication : IPersonApplication
    {
        private readonly IPersonRepository _personRepository;
        private readonly ICodeApplication _codeApplication;

        public PersonApplication(IPersonRepository personRepository, ICodeApplication codeApplication)
        {
            _personRepository = personRepository;
            _codeApplication = codeApplication;
        }

        public OperationResult Create(CreatePerson command)
        {
            var result = new OperationResult();

            if (!command.IsLegal && _personRepository.ExistsNationalCode(command.NationalCode))
                return result.Failed("کد ملی تکراری است.");

            if (command.IsLegal && _personRepository.ExistsEconomicCode(command.EconomicCode))
                return result.Failed("کد اقتصادی تکراری است.");

            var person = new Persons(
                command.FullName,
                command.IsLegal,
                command.NationalCode,
                command.EconomicCode,
                command.RegistrationNumber,
                command.PersonTypeId,
                command.BranchId,
                command.CreditLimit);

            _personRepository.Create(person);
            _personRepository.SaveChanges();

            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = person.Id,
                OwnerType = CodeOwnerTypeDTO.Person,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return result.Failed(codeResult.Message);

            return result.Succedded();
        }

        public OperationResult Edit(EditPerson command)
        {
            var result = new OperationResult();
            var person = _personRepository.Get(command.Id);
            if (person == null) return result.Failed(ApplicationMessages.RecordNotFound);

            if (!command.IsLegal && _personRepository.ExistsNationalCode(command.NationalCode, command.Id))
                return result.Failed("کد ملی وارد شده برای شخص دیگری ثبت شده است.");

            person.Edit(command.FullName, command.NationalCode, command.EconomicCode,
                        command.RegistrationNumber, command.PersonTypeId, command.BranchId, command.IsLegal);

            person.UpdateFinancialInfo(command.CreditLimit);

            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = person.Id,
                OwnerType = CodeOwnerTypeDTO.Person,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return result.Failed(codeResult.Message);

            _personRepository.SaveChanges();
            return result.Succedded();
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

        
        public EditPerson GetDetails(long id)
        {
            var details = _personRepository.GetDetails(id);
            if (details == null) return null;

            var code = _codeApplication.GetByOwner(id, CodeOwnerTypeDTO.Person);
            details.CurrentCode = code?.Value;
            details.ManualCode = code?.Value;

            return details;
        }

        public List<PersonViewModel> GetPersons()
        {
            var persons = _personRepository.GetAllPersons();
            if (persons == null || persons.Count == 0) return persons;

            var ids = persons.Select(x => x.Id).ToList();
            var codes = _codeApplication.GetListByOwners(ids, CodeOwnerTypeDTO.Person);

            var dict = codes
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var p in persons)
                p.Code = dict.TryGetValue(p.Id, out var v) ? v : null;

            return persons;
        }

        public List<PersonViewModel> Search(PersonSearchModel searchModel)
        {
            var persons = _personRepository.Search(searchModel);
            if (persons == null || persons.Count == 0) return persons;

            var ids = persons.Select(x => x.Id).ToList();
            var codes = _codeApplication.GetListByOwners(ids, CodeOwnerTypeDTO.Person);

            var dict = codes
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var p in persons)
                p.Code = dict.TryGetValue(p.Id, out var v) ? v : null;

            return persons;
        }
    }
}
