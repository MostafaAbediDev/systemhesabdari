using _0_Framework.Application;
using CodeManagement.Application.Contracts.Code;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
using System.Transactions;
using CodeManagement.Domain.CodeAgg;

namespace PersonManagement.Application
{
    public class PersonApplication : IPersonApplication
    {
        private readonly IPersonRepository _personRepository;
        private readonly ICodeApplication _codeApplication;
        private readonly ICodeRepository _codeRepository; 
        private readonly IPersonContactRepository _personContactRepository;
        private readonly IPersonAddressRepository _personAddressRepository;
        private readonly IPersonBankRepository _personBankRepository;

        public PersonApplication(IPersonRepository personRepository, ICodeApplication codeApplication,
            IPersonContactRepository personContactRepository, IPersonAddressRepository personAddressRepository, 
            IPersonBankRepository personBankRepository, ICodeRepository codeRepository)
        {
            _personRepository = personRepository;
            _codeApplication = codeApplication;
            _personContactRepository = personContactRepository;
            _personAddressRepository = personAddressRepository;
            _personBankRepository = personBankRepository;
            _codeRepository = codeRepository;
        }

        public OperationResult Create(CreatePerson command)
        {
            var result = new OperationResult();

            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                TransactionScopeAsyncFlowOption.Enabled);

            // Validation
            if (!command.IsLegal &&
                _personRepository.ExistsNationalCode(command.NationalCode))
            {
                return result.Failed("کد ملی تکراری است.");
            }

            if (command.IsLegal &&
                _personRepository.ExistsEconomicCode(command.EconomicCode))
            {
                return result.Failed("کد اقتصادی تکراری است.");
            }

            // Create Person
            var person = new Persons(
                command.FirstName,
                command.LastName,
                command.ContactFirstName,
                command.ContactLastName,
                command.IsLegal,
                command.NationalCode,
                command.EconomicCode,
                command.RegistrationNumber,
                command.PersonTypeId,
                command.BranchId,
                command.CreditLimit,
                command.PersonCategoryId);

            _personRepository.Create(person);
            _personRepository.SaveChanges();

            // Create Code
            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = person.Id,
                OwnerType = CodeOwnerTypeDTO.Person,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return result.Failed(codeResult.Message);

            // چون DbContext جداست
            _codeRepository.SaveChanges();

            // Commit
            transaction.Complete();

            return result.Succedded();
        }


        public OperationResult Edit(EditPerson command)
        {
            var result = new OperationResult();

            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                TransactionScopeAsyncFlowOption.Enabled);

            var person = _personRepository.Get(command.Id);

            if (person == null)
                return result.Failed(ApplicationMessages.RecordNotFound);

            // بررسی کد ملی
            if (!command.IsLegal &&
                _personRepository.ExistsNationalCode(
                    command.NationalCode,
                    command.Id))
            {
                return result.Failed(
                    "کد ملی وارد شده برای شخص دیگری ثبت شده است.");
            }

            // بررسی کد اقتصادی
            if (command.IsLegal &&
                _personRepository.ExistsEconomicCode(
                    command.EconomicCode,
                    command.Id))
            {
                return result.Failed("کد اقتصادی تکراری است.");
            }

            // ویرایش اطلاعات شخص
            person.Edit(
                command.FirstName,
                command.LastName,
                command.ContactFirstName,
                command.ContactLastName,
                command.NationalCode,
                command.EconomicCode,
                command.RegistrationNumber,
                command.PersonTypeId,
                command.BranchId,
                command.IsLegal,
                command.PersonCategoryId);

            // فقط در صورت ارسال CreditLimit
            if (command.CreditLimit.HasValue)
            {
                person.UpdateFinancialInfo(command.CreditLimit.Value);
            }

            // ایجاد یا ویرایش Code
            var codeResult = _codeApplication.SetCode(new CreateCode
            {
                OwnerId = person.Id,
                OwnerType = CodeOwnerTypeDTO.Person,
                IsAutomatic = command.IsCodeAutomatic,
                Value = command.ManualCode
            });

            if (!codeResult.IsSucceeded)
                return result.Failed(codeResult.Message);

            // چون DbContext مربوط به Person جداست
            _personRepository.SaveChanges();

            // چون DbContext مربوط به Code جداست
            _codeRepository.SaveChanges();

            // نهایی کردن Transaction
            transaction.Complete();

            return result.Succedded();
        }

        //public OperationResult Create(CreatePerson command)
        //{
        //    var result = new OperationResult();

        //    if (!command.IsLegal && _personRepository.ExistsNationalCode(command.NationalCode))
        //        return result.Failed("کد ملی تکراری است.");

        //    if (command.IsLegal && _personRepository.ExistsEconomicCode(command.EconomicCode))
        //        return result.Failed("کد اقتصادی تکراری است.");

        //    var person = new Persons(
        //        command.FirstName,
        //        command.LastName,
        //        command.ContactFirstName,
        //        command.ContactLastName,
        //        command.IsLegal,
        //        command.NationalCode,
        //        command.EconomicCode,
        //        command.RegistrationNumber,
        //        command.PersonTypeId,
        //        command.BranchId,
        //        command.CreditLimit,
        //        command.PersonCategoryId);

        //    _personRepository.Create(person);
        //    _personRepository.SaveChanges();


        //    //Set The code 
        //    var codeResult = _codeApplication.SetCode(new CreateCode
        //    {
        //        OwnerId = person.Id,
        //        OwnerType = CodeOwnerTypeDTO.Person,
        //        IsAutomatic = command.IsCodeAutomatic,
        //        Value = command.ManualCode
        //    });

        //    if (!codeResult.IsSucceeded)
        //        return result.Failed(codeResult.Message);


        //    return result.Succedded();
        //}

        //public OperationResult Edit(EditPerson command)
        //{
        //    var result = new OperationResult();
        //    var person = _personRepository.Get(command.Id);
        //    if (person == null) return result.Failed(ApplicationMessages.RecordNotFound);

        //    if (!command.IsLegal && _personRepository.ExistsNationalCode(command.NationalCode, command.Id))
        //        return result.Failed("کد ملی وارد شده برای شخص دیگری ثبت شده است.");

        //    if (command.IsLegal && _personRepository.ExistsEconomicCode(command.EconomicCode, command.Id))
        //        return result.Failed("کد اقتصادی تکراری است.");

        //    person.Edit(command.FirstName, command.LastName, command.ContactFirstName, command.ContactLastName , command.NationalCode, command.EconomicCode,
        //                command.RegistrationNumber, command.PersonTypeId, command.BranchId, command.IsLegal, command.PersonCategoryId);

        //    if (command.CreditLimit.HasValue)
        //    {
        //        person.UpdateFinancialInfo(command.CreditLimit.Value);
        //    }

        //    //Set The Code
        //    var codeResult = _codeApplication.SetCode(new CreateCode
        //    {
        //        OwnerId = person.Id,
        //        OwnerType = CodeOwnerTypeDTO.Person,
        //        IsAutomatic = command.IsCodeAutomatic,
        //        Value = command.ManualCode
        //    });

        //    if (!codeResult.IsSucceeded)
        //        return result.Failed(codeResult.Message);

        //    _personRepository.SaveChanges();
        //    return result.Succedded();
        //}

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                TransactionScopeAsyncFlowOption.Enabled);

            var person = _personRepository.Get(id);

            if (person == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            // حذف Contact ها
            var contacts = _personContactRepository
                .GetEntitiesByPersonId(id);

            foreach (var contact in contacts)
                contact.Remove();

            // حذف Address ها
            var addresses = _personAddressRepository
                .GetEntitiesByPersonId(id);

            foreach (var address in addresses)
                address.Remove();

            // حذف Bank ها
            var banks = _personBankRepository
                .GetEntitiesByPersonId(id);

            foreach (var bank in banks)
                bank.Remove();

            // حذف Code
            var codeResult = _codeApplication.RemoveByOwner(
                id,
                CodeOwnerTypeDTO.Person);

            if (!codeResult.IsSucceeded)
                return operation.Failed(codeResult.Message);

            // حذف Person
            person.Remove();

            // ذخیره تغییرات Person
            _personRepository.SaveChanges();

            // ذخیره تغییرات Code
            _codeRepository.SaveChanges();

            // نهایی کردن Transaction
            transaction.Complete();

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
            details.IsCodeAutomatic = code?.IsAutomatic ?? true;


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
