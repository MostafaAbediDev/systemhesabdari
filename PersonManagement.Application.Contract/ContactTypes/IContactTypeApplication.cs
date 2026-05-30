using _0_Framework.Application;

namespace PersonManagement.Application.Contract.ContactTypes
{
    public interface IContactTypeApplication
    {
        OperationResult Create(CreateContactType command);
        OperationResult Edit(EditContactType command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Active(long id);
        OperationResult NotActive(long id);
        EditContactType GetDetails(long id);
        List<ContactTypeViewModel> Search(ContactTypeSearchModel searchModel);
        List<ContactTypeViewModel> GetActive();

    }
}
