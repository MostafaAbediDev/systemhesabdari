using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.PersonTypes;

namespace PersonManagement.Domain.Person.PersonTypeAgg
{
    public interface IPersonTypeRepository : IRepository<long, PersonType>
    {
        List<PersonTypeViewModel> GetPersonTypes();
    }
}
