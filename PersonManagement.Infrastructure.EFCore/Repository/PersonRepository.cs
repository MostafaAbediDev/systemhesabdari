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
                    FullName = x.FullName,
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

            if (!string.IsNullOrWhiteSpace(searchModel.FullName))
            {
                var fullName = searchModel.FullName.Trim();
                query = query.Where(x => x.FullName.Contains(fullName));
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
                    FullName = x.FullName,
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
                    FullName = x.FullName,
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
