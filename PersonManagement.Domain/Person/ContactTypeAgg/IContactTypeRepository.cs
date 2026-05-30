using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.ContactTypes;

namespace PersonManagement.Domain.Person.ContactTypeAgg
{
    public interface IContactTypeRepository : IRepository<long, ContactTypes>
    {
        EditContactType GetDetails(long id);
        List<ContactTypeViewModel> Search(ContactTypeSearchModel searchModel);
        List<ContactTypeViewModel> GetActive();
    }
}
