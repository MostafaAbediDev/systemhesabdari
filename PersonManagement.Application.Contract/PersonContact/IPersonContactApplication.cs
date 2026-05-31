using _0_Framework.Application;

namespace PersonManagement.Application.Contract.PersonContact
{
    public interface IPersonContactApplication
    {
        OperationResult Create(CreatePersonContact command);
        OperationResult Edit(EditPersonContact command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Active(long id);
        OperationResult NotActive(long id);
        OperationResult SetDefault(long id);
        OperationResult UnsetDefault(long id);
        EditPersonContact GetDetails(long id);
        List<PersonContactViewModel> Search(PersonContactSearchModel searchModel);
        List<PersonContactViewModel> GetByPersonId(long personId);
    }
}
