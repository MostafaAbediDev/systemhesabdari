using _0_Framework.Application;

namespace PersonManagement.Application.Contract.PersonTypes
{
    public interface IPersonTypeApplication
    {
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        List<PersonTypeViewModel> GetPersonTypes();

    }
}
