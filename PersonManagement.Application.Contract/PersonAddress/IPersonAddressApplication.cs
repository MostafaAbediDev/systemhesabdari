using _0_Framework.Application;

namespace PersonManagement.Application.Contract.PersonAddress
{
    public interface IPersonAddressApplication
    {
        OperationResult Create(CreatePersonAddress command);
        OperationResult Edit(EditPersonAddress command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Active(long id);
        OperationResult NotActive(long id);
        OperationResult SetDefault(long id);
        OperationResult UnsetDefault(long id);
        EditPersonAddress GetDetails(long id);
        List<PersonAddressViewModel> Search(PersonAddressSearchModel searchModel);
        List<PersonAddressViewModel> GetByPersonId(long personId);
    }
}
