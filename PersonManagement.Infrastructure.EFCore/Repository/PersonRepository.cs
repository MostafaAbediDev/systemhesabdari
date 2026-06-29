using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Domain.Person.PersonAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonRepository : RepositoryBase<long, Persons>, IPersonRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonRepository(PersonFakeDataContext personFakeDataContext) : base(personFakeDataContext)
        {
            _context = personFakeDataContext;
        }

        public EditPerson GetDetails(long id)
        {
            return _context.Persons
                .AsNoTracking()
                .Select(x => new EditPerson
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ContactFirstName = x.ContactFirstName,
                    ContactLastName = x.ContactLastName,
                    IsLegal = x.IsLegal,
                    NationalCode = x.NationalCode,
                    EconomicCode = x.EconomicCode,
                    RegistrationNumber = x.RegistrationNumber,
                    PersonTypeId = x.PersonTypeId,
                    BranchId = x.BranchId,
                    CreditLimit = x.CreditLimit,
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<PersonViewModel> Search(PersonSearchModel searchModel)
        {
            var query = _context.Persons
                .Include(x => x.PersonType)
                .Include(x => x.Branches)
                .AsNoTracking();

            // اگر SoftDelete دارید
            query = query.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchModel.FirstName))
            {
                var firstName = searchModel.FirstName.Trim();
                query = query.Where(x => x.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.LastName))
            {
                var lastName = searchModel.LastName.Trim();
                query = query.Where(x => x.LastName.Contains(lastName));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.ContactFirstName))
            {
                var contactFirstName = searchModel.ContactFirstName.Trim();
                query = query.Where(x => x.ContactFirstName.Contains(contactFirstName));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.ContactLastName))
            {
                var contactLastName = searchModel.ContactLastName.Trim();
                query = query.Where(x => x.ContactLastName.Contains(contactLastName));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.NationalCode))
            {
                var code = searchModel.NationalCode.Trim();
                query = query.Where(x =>
                    (x.NationalCode != null && x.NationalCode.Contains(code)) ||
                    (x.EconomicCode != null && x.EconomicCode.Contains(code)));
            }

            var result = query
                .OrderByDescending(x => x.Id)
                .Select(x => new PersonViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ContactFirstName = x.ContactFirstName,
                    ContactLastName = x.ContactLastName,
                    IsLegal = x.IsLegal,
                    NationalCode = x.NationalCode,
                    EconomicCode = x.EconomicCode,
                    PersonType = x.PersonType.Title,
                    BranchName = x.Branches.Title,
                    CreditLimit = x.CreditLimit,
                    AvailableCredit = x.AvailableCredit,
                    IsActive = x.IsActive,
                })
                .ToList();

            return result;
        }

        public List<PersonViewModel> GetAllPersons()
        {
            return _context.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new PersonViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ContactFirstName = x.ContactFirstName,
                    ContactLastName = x.ContactLastName,
                    IsLegal = x.IsLegal,
                    NationalCode = x.NationalCode,
                    EconomicCode = x.EconomicCode,
                    CreditLimit = x.CreditLimit,
                    AvailableCredit = x.AvailableCredit,
                    IsActive = x.IsActive,

                    PersonType = x.PersonType.Title,
                    BranchName = x.Branches.Title
                })
                .ToList();
        }

        public bool ExistsNationalCode(string code, long id = 0)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            code = code.Trim();

            return _context.Persons.Any(x =>
                !x.IsDeleted &&
                x.Id != id &&
                x.NationalCode != null &&
                x.NationalCode == code);
        }

        public bool ExistsEconomicCode(string code, long id = 0)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            code = code.Trim();

            return _context.Persons.Any(x =>
                !x.IsDeleted &&
                x.Id != id &&
                x.EconomicCode != null &&
                x.EconomicCode == code);
        }
    }
}
