using _0_Framework.Application;
using PersonManagement.Application.Contract.PersonCategory;
using PersonManagement.Domain.Person.PersonCategoryAgg;

namespace PersonManagement.Application
{
    public class PersonCategoryApplication : IPersonCategoryApplication
    {

        private readonly IPersonCategoryRepository _personCategoryRepository;

        public PersonCategoryApplication(IPersonCategoryRepository personCategoryRepository)
        {
            _personCategoryRepository = personCategoryRepository;
        }

        public OperationResult Create(CreatePersonCategory command)
        {
            var operation = new OperationResult();

            if (_personCategoryRepository.ExistsByTitle(
                    command.Title,
                    command.PersonTypeId,
                    command.ParentId))
            {
                return operation.Failed("عنوان وارد شده تکراری است.");
            }

            var category = new PersonCategory(
                command.Title,
                command.PersonTypeId,
                command.ParentId);

            _personCategoryRepository.Create(category);
            _personCategoryRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditPersonCategory command)
        {
            var operation = new OperationResult();

            var category = _personCategoryRepository.Get(command.Id);

            if (category == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            if (_personCategoryRepository.ExistsByTitle(command.Title,command.PersonTypeId,command.ParentId))
            {
                return operation.Failed("عنوان وارد شده تکراری است.");
            }

            if (command.ParentId == command.Id)
            {
                return operation.Failed("دسته بندی نمی تواند والد خودش باشد.");
            }

            category.Edit(
                command.Title,
                command.PersonTypeId,
                command.ParentId);

            _personCategoryRepository.SaveChanges();

            return operation.Succedded();
        }

        public EditPersonCategory GetDetails(long id)
        {
            return _personCategoryRepository.GetDetails(id);
        }

        public List<PersonCategoryViewModel> GetParents(long personTypeId)
        {
            return _personCategoryRepository.GetParents(personTypeId);
        }

        public List<PersonCategoryTreeViewModel> GetTree(long personTypeId)
        {
            var categories = _personCategoryRepository.GetAllByType(personTypeId);

            var roots = categories
                .Where(x => x.ParentId == null)
                .ToList();

            return roots
                .Select(x => BuildTree(x, categories))
                .ToList();
        }

        private PersonCategoryTreeViewModel BuildTree(PersonCategory category,List<PersonCategory> categories)
        {
            return new PersonCategoryTreeViewModel
            {
                Id = category.Id,
                Title = category.Title,
                Children = categories
                    .Where(x => x.ParentId == category.Id)
                    .Select(x => BuildTree(x, categories))
                    .ToList()
            };
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var category = _personCategoryRepository.Get(id);

            if (category == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            if (_personCategoryRepository.HasChildren(id))
            {
                return operation.Failed(
                    "این دسته بندی دارای زیر مجموعه است.");
            }

            category.Remove();

            _personCategoryRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var category = _personCategoryRepository.Get(id);

            if (category == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            category.Restore();

            _personCategoryRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Active(long id)
        {
            var operation = new OperationResult();

            var category = _personCategoryRepository.Get(id);

            if (category == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            category.Active();

            _personCategoryRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult NotActive(long id)
        {
            var operation = new OperationResult();

            var category = _personCategoryRepository.Get(id);

            if (category == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            category.NotActive();

            _personCategoryRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<PersonCategoryViewModel> Search(PersonCategorySearchModel searchModel)
        {
            return _personCategoryRepository.Search(searchModel);
        }
    }
}
