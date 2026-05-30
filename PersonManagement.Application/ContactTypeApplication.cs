using _0_Framework.Application;
using PersonManagement.Application.Contract.ContactTypes;
using PersonManagement.Domain.Person.ContactTypeAgg;

namespace PersonManagement.Application
{
    public class ContactTypeApplication : IContactTypeApplication
    {
        private readonly IContactTypeRepository _contactTypeRepository;

        public ContactTypeApplication(IContactTypeRepository contactTypeRepository)
        {
            _contactTypeRepository = contactTypeRepository;
        }

        public OperationResult Create(CreateContactType command)
        {
            var operation = new OperationResult();

            if (command == null || string.IsNullOrWhiteSpace(command.Title))
                return operation.Failed("عنوان نوع تماس الزامی است.");

            if (_contactTypeRepository.Exists(x => x.Title == command.Title))
                return operation.Failed("عنوان نوع تماستکراری است");

            var duplicate = _contactTypeRepository
                .Search(new ContactTypeSearchModel { Title = command.Title })
                .Any(x => x.Title.Trim() == command.Title.Trim());

            if (duplicate)
                return operation.Failed("این نوع تماس قبلاً ثبت شده است.");

            var entity = new ContactTypes(command.Title);

            _contactTypeRepository.Create(entity);
            _contactTypeRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditContactType command)
        {
            var operation = new OperationResult();

            if (command == null || command.Id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            if (string.IsNullOrWhiteSpace(command.Title))
                return operation.Failed("عنوان نوع تماس الزامی است.");

            var entity = _contactTypeRepository.Get(command.Id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            var duplicate = _contactTypeRepository
                .Search(new ContactTypeSearchModel { Title = command.Title })
                .Any(x => x.Id != command.Id && x.Title.Trim() == command.Title.Trim());

            if (duplicate)
                return operation.Failed("این عنوان قبلاً برای نوع تماس دیگری ثبت شده است.");

            entity.Edit(command.Title);

            _contactTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _contactTypeRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.Remove();

            _contactTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _contactTypeRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.Restore();

            _contactTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Active(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _contactTypeRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.Active();

            _contactTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult NotActive(long id)
        {
            var operation = new OperationResult();

            if (id <= 0)
                return operation.Failed("شناسه نامعتبر است.");

            var entity = _contactTypeRepository.Get(id);
            if (entity == null)
                return operation.Failed("رکوردی با این شناسه یافت نشد.");

            entity.NotActive();

            _contactTypeRepository.SaveChanges();
            return operation.Succedded();
        }

        public EditContactType GetDetails(long id)
        {
            return _contactTypeRepository.GetDetails(id);
        }

        public List<ContactTypeViewModel> Search(ContactTypeSearchModel searchModel)
        {
            return _contactTypeRepository.Search(searchModel);
        }

        public List<ContactTypeViewModel> GetActive()
        {
            return _contactTypeRepository.GetActive();
        }
    }
}
