using _0_Framework.Application;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Domain.Person.PersonContactAgg;

namespace PersonManagement.Application
{
    public class PersonContactApplication : IPersonContactApplication
    {
        private readonly IPersonContactRepository _personContactRepository;

        public PersonContactApplication(IPersonContactRepository personContactRepository)
        {
            _personContactRepository = personContactRepository;
        }

        public OperationResult Active(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _personContactRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.Active();
            _personContactRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Create(CreatePersonContact command)
        {
            var operation = new OperationResult();

            if (command == null)
                return operation.Failed("اطلاعات ارسالی نامعتبر است.");

            if (command.PersonId <= 0)
                return operation.Failed("شخص انتخاب نشده است.");

            if (command.ContactTypeId <= 0)
                return operation.Failed("نوع تماس انتخاب نشده است.");

            if (string.IsNullOrWhiteSpace(command.Value))
                return operation.Failed("مقدار تماس الزامی است.");

            if (_personContactRepository.Exists(x => x.Value == command.Value && x.ContactTypeId == command.ContactTypeId && x.PersonId == command.PersonId))
                return operation.Failed("این اطلاعات تماس قبلاً برای این شخص ثبت شده است.");

            var entity = new PersonContacts(
                command.Value,
                command.Description,
                command.PersonId,
                command.ContactTypeId
            );

            if (command.IsDefault)
            {
                var currentDefault = _personContactRepository.GetDefault(command.PersonId, command.ContactTypeId);
                if (currentDefault != null)
                    currentDefault.UnsetDefault();

                entity.SetDefault();
            }

            _personContactRepository.Create(entity);
            _personContactRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditPersonContact command)
        {
            var operation = new OperationResult();

            if (command == null || command.Id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            if (string.IsNullOrWhiteSpace(command.Value))
                return operation.Failed("مقدار تماس الزامی است.");

            if (command.ContactTypeId <= 0)
                return operation.Failed("نوع تماس انتخاب نشده است.");

            var entity = _personContactRepository.Get(command.Id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            if (_personContactRepository.Exists(x => command.Value == x.Value && x.ContactTypeId == command.ContactTypeId && x.PersonId == command.PersonId))
                return operation.Failed("این اطلاعات تماس قبلاً برای این شخص ثبت شده است.");

            var oldContactTypeId = entity.ContactTypeId;

            entity.Edit(command.Value, command.Description, command.ContactTypeId);

            if (command.IsDefault)
            {
                var currentDefault = _personContactRepository.GetDefault(entity.PersonId, command.ContactTypeId);
                if (currentDefault != null && currentDefault.Id != entity.Id)
                    currentDefault.UnsetDefault();

                entity.SetDefault();
            }
            else
            {
                entity.UnsetDefault();
            }

            _personContactRepository.SaveChanges();
            return operation.Succedded();
        }


        public List<PersonContactViewModel> GetByPersonId(long personId)
        {
            return _personContactRepository.GetByPersonId(personId);
        }

        public EditPersonContact GetDetails(long id)
        {
            return _personContactRepository.GetDetails(id);
        }

        public OperationResult NotActive(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _personContactRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.NotActive();
            _personContactRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _personContactRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.Remove();
            _personContactRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _personContactRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.Restore();
            _personContactRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<PersonContactViewModel> Search(PersonContactSearchModel searchModel)
        {
            return _personContactRepository.Search(searchModel);
        }

        public OperationResult SetDefault(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _personContactRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            var currentDefault = _personContactRepository.GetDefault(entity.PersonId, entity.ContactTypeId);
            if (currentDefault != null && currentDefault.Id != entity.Id)
                currentDefault.UnsetDefault();

            entity.SetDefault();

            _personContactRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult UnsetDefault(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _personContactRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.UnsetDefault();

            _personContactRepository.SaveChanges();
            return operation.Succedded();
        }
    }
}
