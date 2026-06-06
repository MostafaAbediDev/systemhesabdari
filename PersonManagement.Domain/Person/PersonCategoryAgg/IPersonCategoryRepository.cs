using _0_FrameWork.Domain;
using PersonManagement.Application.Contract.PersonCategory;

namespace PersonManagement.Domain.Person.PersonCategoryAgg
{
    public interface IPersonCategoryRepository : IRepository<long, PersonCategory>
    {
        PersonCategory GetWithChildren(long id);
        bool ExistsByTitle(string title,long personTypeId,long? parentId);
        bool HasChildren(long id);
        List<PersonCategory> GetAllByType(long personTypeId);
        EditPersonCategory GetDetails(long id);
        List<PersonCategoryViewModel> Search(PersonCategorySearchModel searchModel);
        List<PersonCategoryViewModel> GetParents(long personTypeId);
    }
}
