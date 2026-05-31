using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.PersonAddress;

namespace PersonManagement.Domain.Person.PersonAddressAgg
{
    public interface IPersonAddressRepository : IRepository<long, PersonAddresses>
    {
        EditPersonAddress GetDetails(long id);
        List<PersonAddressViewModel> Search(PersonAddressSearchModel searchModel);
        List<PersonAddressViewModel> GetByPersonId(long personId);
        PersonAddresses GetDefault(long personId);
    }
}
