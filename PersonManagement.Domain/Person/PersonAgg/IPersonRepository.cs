using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.Persons;

namespace PersonManagement.Domain.Person.PersonAgg
{
    public interface IPersonRepository : IRepository<long, Persons>
    {
        bool Exists(Func<Persons, bool> predicate);
        EditPerson GetDetails(long id);
        PersonFullViewModel GetFullDetails(long id);
        List<PersonViewModel> Search(PersonSearchModel searchModel);
        List<PersonViewModel> GetAllPersons();
    }
}
