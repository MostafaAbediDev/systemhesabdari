using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.PersonBank;

namespace PersonManagement.Domain.Person.PersonBankAgg
{
    public interface IPersonBankRepository : IRepository<long, PersonBanks>
    {
        EditPersonBank GetDetails(long id);
        List<PersonBankViewModel> Search(PersonBankSearchModel searchModel);
        List<PersonBankViewModel> GetByPersonId(long personId);
        PersonBankViewModel GetDefaultByPersonId(long personId);
        void UnsetAllDefaults(long personId);
    }
}
