using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.PersonCategory;
using PersonManagement.Domain.Person.PersonCategoryAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonCategoryRepository : RepositoryBase<long, PersonCategory>, IPersonCategoryRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonCategoryRepository(PersonFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public bool ExistsByTitle(string title, long personTypeId, long? parentId)
        {
            return _context.PersonCategories.Any(x =>
                            x.Title == title &&
                            x.PersonTypeId == personTypeId &&
                            x.ParentId == parentId &&
                            !x.IsDeleted);
        }

        public List<PersonCategory> GetAllByType(long personTypeId)
        {
            return _context.PersonCategories
                .Where(x => x.PersonTypeId == personTypeId &&
                            !x.IsDeleted)
                .OrderBy(x => x.Title)
                .AsNoTracking()
                .ToList();
        }

        public EditPersonCategory GetDetails(long id)
        {
            return _context.PersonCategories
                    .Select(x => new EditPersonCategory
                    {
                        Id = x.Id,
                        Title = x.Title,
                        PersonTypeId = x.PersonTypeId,
                        ParentId = x.ParentId
                    })
                    .FirstOrDefault(x => x.Id == id);
        }

        public List<PersonCategoryViewModel> GetParents(long personTypeId)
        {
            return _context.PersonCategories
                    .Where(x =>
                        x.PersonTypeId == personTypeId &&
                        !x.IsDeleted)
                    .Select(x => new PersonCategoryViewModel
                    {
                        Id = x.Id,
                        Title = x.Title
                    })
                    .OrderBy(x => x.Title)
                    .ToList();
        }

        public PersonCategory GetWithChildren(long id)
        {
            return _context.PersonCategories
                            .Include(x => x.Children)
                            .FirstOrDefault(x => x.Id == id);
        }

        public bool HasChildren(long id)
        {
            return _context.PersonCategories.Any(x =>
                            x.ParentId == id &&
                            !x.IsDeleted);
        }

        public List<PersonCategoryViewModel> Search(PersonCategorySearchModel searchModel)
        {
            var query = _context.PersonCategories
                    .Include(x => x.PersonType)
                    .Include(x => x.Parent)
                    .Select(x => new PersonCategoryViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        PersonTypeId = x.PersonTypeId,
                        PersonTypeTitle = x.PersonType.Title,
                        ParentId = x.ParentId,
                        ParentTitle = x.Parent != null
                            ? x.Parent.Title
                            : "-",
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        CreationDate = x.CreationDate.ToString()
                    });

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
            {
                query = query.Where(x =>
                    x.Title.Contains(searchModel.Title));
            }

            if (searchModel.PersonTypeId.HasValue)
            {
                query = query.Where(x =>
                    x.PersonTypeId == searchModel.PersonTypeId);
            }

            if (searchModel.ParentId.HasValue)
            {
                query = query.Where(x =>
                    x.ParentId == searchModel.ParentId);
            }

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}
