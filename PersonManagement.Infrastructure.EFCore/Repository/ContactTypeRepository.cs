using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.ContactTypes;
using PersonManagement.Domain.Person.ContactTypeAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class ContactTypeRepository : RepositoryBase<long, ContactTypes>, IContactTypeRepository
    {
        private readonly PersonFakeDataContext _context;

        public ContactTypeRepository(PersonFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<ContactTypeViewModel> GetActive()
        {
            return _context.ContactTypes
                            .AsNoTracking()
                            .Where(x => !x.IsDeleted && x.IsActive)
                            .OrderBy(x => x.Title)
                            .Select(x => new ContactTypeViewModel
                            {
                                Id = x.Id,
                                Title = x.Title,
                            })
                            .ToList();
        }

        public EditContactType GetDetails(long id)
        {
            return _context.ContactTypes
                            .AsNoTracking()
                            .Where(x => x.Id == id)
                            .Select(x => new EditContactType
                            {
                                Id = x.Id,
                                Title = x.Title
                            })
                            .FirstOrDefault();
        }

        public List<ContactTypeViewModel> Search(ContactTypeSearchModel searchModel)
        {
            var query = _context.ContactTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));


            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new ContactTypeViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                })
                .ToList();
        }
    }
}
