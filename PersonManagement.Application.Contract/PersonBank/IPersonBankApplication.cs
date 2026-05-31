using _0_Framework.Application;

namespace PersonManagement.Application.Contract.PersonBank
{
    public interface IPersonBankApplication
    {
        OperationResult Create(CreatePersonBank command);
        OperationResult Edit(EditPersonBank command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Active(long id);
        OperationResult NotActive(long id);
        OperationResult SetDefault(long id);
        EditPersonBank GetDetails(long id);
        List<PersonBankViewModel> Search(PersonBankSearchModel searchModel);
        List<PersonBankViewModel> GetByPersonId(long personId);
        PersonBankViewModel GetDefaultByPersonId(long personId);
    }
}
