using _0_Framework.Application;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Domain.Person.PersonAddressAgg;

namespace PersonManagement.Application
{
    public class PersonAddressApplication : IPersonAddressApplication
    {
        private readonly IPersonAddressRepository _personAddressRepository;

        public PersonAddressApplication(IPersonAddressRepository personAddressRepository)
        {
            _personAddressRepository = personAddressRepository;
        }

        public OperationResult Create(CreatePersonAddress command)
        {
            var operation = new OperationResult();

            var title = command.Title;
            if (_personAddressRepository.Exists(x =>
                    !x.IsDeleted &&
                    x.PersonId == command.PersonId &&
                    x.Title == title))
                return operation.Failed("عنوان آدرس برای این شخص تکراری است.");

            var address = new PersonAddresses(
                title,
                command.PersonId,
                command.ProvinceId,
                command.CityId,
                command.Address,
                command.PostalCode
            );

            _personAddressRepository.Create(address);
            _personAddressRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditPersonAddress command)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(command.Id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            var title = command.Title;

            if (_personAddressRepository.Exists(x =>
                    x.Id != command.Id &&
                    !x.IsDeleted &&
                    x.PersonId == address.PersonId && 
                    x.Title == title))
                return operation.Failed("عنوان آدرس برای این شخص تکراری است.");

            address.Edit(
                title,
                command.ProvinceId,
                command.CityId,
                command.Address,
                command.PostalCode
            );

            _personAddressRepository.SaveChanges();
            return operation.Succedded();
        }

        public List<PersonAddressViewModel> GetByPersonId(long personId)
        {
            return _personAddressRepository.GetByPersonId(personId);
        }

        public EditPersonAddress GetDetails(long id)
        {
            return _personAddressRepository.GetDetails(id);
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            address.Remove();
            _personAddressRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            address.Restore();
            _personAddressRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Active(long id)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            address.Active();
            _personAddressRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult NotActive(long id)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            address.NotActive();
            _personAddressRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<PersonAddressViewModel> Search(PersonAddressSearchModel searchModel)
        {
            throw new NotImplementedException();
        }

        public OperationResult SetDefault(long id)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            if (address.IsDeleted)
                return operation.Failed("امکان پیش‌فرض کردن آدرس حذف شده وجود ندارد.");

            if (address.IsDefault)
                return operation.Succedded();

            var currentDefault = _personAddressRepository.GetDefault(address.PersonId);
            if (currentDefault != null && currentDefault.Id != address.Id)
                currentDefault.UnsetDefault();

            address.SetDefault();

            _personAddressRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult UnsetDefault(long id)
        {
            var operation = new OperationResult();

            var address = _personAddressRepository.Get(id);
            if (address == null)
                return operation.Failed("آدرس مورد نظر یافت نشد.");

            if (!address.IsDefault)
                return operation.Succedded();

            address.UnsetDefault();
            _personAddressRepository.SaveChanges();

            return operation.Succedded();
        }
    }
}
