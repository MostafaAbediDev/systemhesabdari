using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.PersonContact;

namespace PersonManagement.Domain.Person.PersonContactAgg
{
    public interface IPersonContactRepository : IRepository<long, PersonContacts>
    {
        EditPersonContact GetDetails(long id);
        List<PersonContactViewModel> Search(PersonContactSearchModel searchModel);
        List<PersonContactViewModel> GetByPersonId(long personId);
        PersonContacts GetDefault(long personId, long contactTypeId);

    }
}
