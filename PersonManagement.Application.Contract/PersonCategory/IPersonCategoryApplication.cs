using _0_Framework.Application;

namespace PersonManagement.Application.Contract.PersonCategory
{
    public interface IPersonCategoryApplication
    {
        OperationResult Create(CreatePersonCategory command);
        OperationResult Edit(EditPersonCategory command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Active(long id);
        OperationResult NotActive(long id);
        EditPersonCategory GetDetails(long id);
        List<PersonCategoryViewModel> Search(PersonCategorySearchModel searchModel);
        List<PersonCategoryTreeViewModel> GetTree(long personTypeId);
        List<PersonCategoryViewModel> GetParents(long personTypeId);
    }
}
