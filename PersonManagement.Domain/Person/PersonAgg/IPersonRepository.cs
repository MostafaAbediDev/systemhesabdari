using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.Persons;
using System.Linq.Expressions;

namespace PersonManagement.Domain.Person.PersonAgg
{
    public interface IPersonRepository : IRepository<long, Persons>
    {
        EditPerson GetDetails(long id);
        PersonFullViewModel GetFullDetails(long id);
        List<PersonViewModel> Search(PersonSearchModel searchModel);
        List<PersonViewModel> GetAllPersons();
    }
}
