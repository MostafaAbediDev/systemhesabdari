using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.Persons;

namespace PersonManagement.Domain.Person.PersonAgg
{
    public interface IPersonRepository : IRepository<long, Persons>
    {
        EditPerson GetDetails(long id);
        List<PersonViewModel> Search(PersonSearchModel searchModel);
        List<PersonViewModel> GetAllPersons();
        bool ExistsNationalCode(string code, long id = 0);
        bool ExistsEconomicCode(string code, long id = 0);
    }
}
