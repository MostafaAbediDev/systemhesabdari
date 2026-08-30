using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Domain.Person.PersonContactAgg;

namespace PersonManagement.Domain.Person.PersonAddressAgg
{
    public interface IPersonAddressRepository : IRepository<long, PersonAddresses>
    {
        EditPersonAddress GetDetails(long id);
        List<PersonAddressViewModel> Search(PersonAddressSearchModel searchModel);
        List<PersonAddressViewModel> GetByPersonId(long personId);
        PersonAddresses GetDefault(long personId);
        List<PersonAddresses> GetEntitiesByPersonId(long personId);
    }
}
