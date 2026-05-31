using _0_Framework.Application;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Domain.Person.PersonBankAgg;

namespace PersonManagement.Application
{
    public class PersonBankApplication : IPersonBankApplication
    {
        private readonly IPersonBankRepository _personBankRepository;

        public PersonBankApplication(IPersonBankRepository personBankRepository)
        {
            _personBankRepository = personBankRepository;
        }

        public OperationResult Create(CreatePersonBank command)
        {
            var result = new OperationResult();

            if (_personBankRepository.Exists(x => x.PersonId == command.PersonId && x.Shaba == command.Shaba && !x.IsDeleted))
                return result.Failed("این شماره شبا قبلا ثبت شده است.");

            var entity = new PersonBanks(
                command.AccountNumber,
                command.CardNumber,
                command.Shaba,
                command.PersonId,
                command.BankBranchId);

            if (command.IsDefault)
            {
                _personBankRepository.UnsetAllDefaults(command.PersonId);
                entity.SetDefault();
            }

            _personBankRepository.Create(entity);
            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult Edit(EditPersonBank command)
        {
            var result = new OperationResult();

            var entity = _personBankRepository.Get(command.Id);
            if (entity == null || entity.IsDeleted)
                return result.Failed(ApplicationMessages.RecordNotFound);

            entity.Edit(
                command.AccountNumber,
                command.CardNumber,
                command.Shaba,
                command.BankBranchId);

            if (command.IsDefault)
            {
                _personBankRepository.UnsetAllDefaults(entity.PersonId);
                entity.SetDefault();
            }
            else
            {
                entity.UnsetDefault();
            }

            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public List<PersonBankViewModel> GetByPersonId(long personId)
        {
            return _personBankRepository.GetByPersonId(personId);
        }

        public PersonBankViewModel GetDefaultByPersonId(long personId)
        {
            return _personBankRepository.GetDefaultByPersonId(personId);
        }

        public EditPersonBank GetDetails(long id)
        {
            return _personBankRepository.GetDetails(id);
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();
            var entity = _personBankRepository.Get(id);
            if (entity == null) return result.Failed(ApplicationMessages.RecordNotFound);

            entity.Remove();
            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();
            var entity = _personBankRepository.Get(id);
            if (entity == null) return result.Failed(ApplicationMessages.RecordNotFound);

            entity.Restore();
            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult Active(long id)
        {
            var result = new OperationResult();
            var entity = _personBankRepository.Get(id);
            if (entity == null || entity.IsDeleted)
                return result.Failed(ApplicationMessages.RecordNotFound);

            entity.Active();
            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult NotActive(long id)
        {
            var result = new OperationResult();
            var entity = _personBankRepository.Get(id);
            if (entity == null || entity.IsDeleted)
                return result.Failed(ApplicationMessages.RecordNotFound);

            entity.NotActive();
            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult SetDefault(long id)
        {
            var result = new OperationResult();
            var entity = _personBankRepository.Get(id);
            if (entity == null || entity.IsDeleted)
                return result.Failed(ApplicationMessages.RecordNotFound);

            _personBankRepository.UnsetAllDefaults(entity.PersonId);
            entity.SetDefault();

            _personBankRepository.SaveChanges();
            return result.Succedded();
        }

        public List<PersonBankViewModel> Search(PersonBankSearchModel searchModel)
        {
            return _personBankRepository.Search(searchModel);
        }

    }
}
