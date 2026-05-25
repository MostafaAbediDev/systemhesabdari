using _0_Framework.Application;

namespace PersonManagement.Application.Contract.Persons
{
    public interface IPersonApplication
    {
        OperationResult Create(CreatePerson command);
        OperationResult Edit(EditPerson command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        //OperationResult MakeLegal(long id);
        //OperationResult MakeIllegal(long id);
        EditPerson GetDetails(long id);
        List<PersonViewModel> Search(PersonSearchModel searchModel);
        List<PersonViewModel> GetPersons();
        PersonFullViewModel GetFullDetails(long id);

    }
}
