using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Domain.Person.PersonContactAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonContactRepository : RepositoryBase<long, PersonContacts>, IPersonContactRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonContactRepository(PersonFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<PersonContactViewModel> GetByPersonId(long personId)
        {
            return _context.PersonContacts
                            .AsNoTracking()
                            .Include(x => x.ContactTypes)
                            .Include(x => x.Persons)
                            .Where(x => x.PersonId == personId && !x.IsDeleted)
                            .OrderByDescending(x => x.IsDefault)
                            .ThenBy(x => x.ContactTypes.Title)
                            .Select(x => new PersonContactViewModel
                            {
                                Id = x.Id,
                                Value = x.Value,
                                Description = x.Description,
                                PersonId = x.PersonId,
                                ContactTypeId = x.ContactTypeId,
                                ContactTypeTitle = x.ContactTypes.Title,
                                PersonFirstName = x.Persons.FirstName,
                                PersonLastName = x.Persons.LastName
                            })
                            .ToList();
        }

        public PersonContacts GetDefault(long personId, long contactTypeId)
        {
            return _context.PersonContacts
                            .FirstOrDefault(x =>
                                x.PersonId == personId &&
                                x.ContactTypeId == contactTypeId &&
                                x.IsDefault &&
                                !x.IsDeleted);
        }

        public EditPersonContact GetDetails(long id)
        {
            return _context.PersonContacts
                            .AsNoTracking()
                            .Where(x => x.Id == id)
                            .Select(x => new EditPersonContact
                            {
                                Id = x.Id,
                                Value = x.Value,
                                Description = x.Description,
                                PersonId = x.PersonId,
                                ContactTypeId = x.ContactTypeId,
                                IsDefault = x.IsDefault
                            })
                            .FirstOrDefault();
        }

        public List<PersonContactViewModel> Search(PersonContactSearchModel searchModel)
        {
            var query = _context.PersonContacts
                            .AsNoTracking()
                            .Include(x => x.Persons)
                            .Include(x => x.ContactTypes)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.Value))
                query = query.Where(x => x.Value.Contains(searchModel.Value));

            if (searchModel.PersonId > 0)
                query = query.Where(x => x.PersonId == searchModel.PersonId);

            if (searchModel.ContactTypeId > 0)
                query = query.Where(x => x.ContactTypeId == searchModel.ContactTypeId);

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new PersonContactViewModel
                {
                    Id = x.Id,
                    Value = x.Value,
                    Description = x.Description,
                    PersonId = x.PersonId,
                    ContactTypeId = x.ContactTypeId,
                    ContactTypeTitle = x.ContactTypes.Title,
                    PersonFirstName = x.Persons.FirstName,
                    PersonLastName = x.Persons.LastName,
                })
                .ToList();
        }
    }
}
