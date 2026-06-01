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
<<<<<<< HEAD
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
=======
                 .Where(x => x.Id == id)
                 .Select(x => new EditPerson
                 {
                     Id = x.Id,
                     Code = x.Code,
                     FullName = x.FullName,
                     NationalCode = x.NationalCode,
                     EconomicCode = x.EconomicCode,
                     RegistrationNumber = x.RegistrationNumber,
<<<<<<< HEAD
                     PersonType = x.PersonType,
=======
>>>>>>> master
                     BranchId = x.BranchId,
                     IsLegal = x.IsLegal
                 })
                 .FirstOrDefault();
        }

        public PersonFullViewModel GetFullDetails(long id)
        {
            var person = _context.Persons
                .Include(x => x.Branches)
                .Include(x => x.PersonContacts)
                    .ThenInclude(x => x.ContactTypes)
                .Include(x => x.PersonAddresses)
                    .ThenInclude(x => x.Provinces)
                .Include(x => x.PersonAddresses)
                    .ThenInclude(x => x.Cities)
                .Include(x => x.PersonBanks)
                .FirstOrDefault(x => x.Id == id);

            if (person == null)
                return null;

            return new PersonFullViewModel
            {
                Person = new PersonViewModel
                {
                    Id = person.Id,
                    Code = person.Code,
                    FullName = person.FullName,
                    NationalCode = person.NationalCode,
                    EconomicCode = person.EconomicCode,
                    RegistrationNumber = person.RegistrationNumber,
<<<<<<< HEAD
                    PersonType = person.PersonType,
=======
>>>>>>> master
                    BranchName = person.Branches.Title,
                    IsActive = person.IsActive,
                    IsLegal = person.IsLegal
                },

                Contacts = person.PersonContacts
                    .Where(x => !x.IsDeleted)
                    .Select(c => new PersonContactViewModel
                    {
                        Id = c.Id,
                        Value = c.Value,
                        Description = c.Description,
                        IsDefault = c.IsDefault,
                        ContactTypeId = c.ContactTypeId,
                        ContactTypeTitle = c.ContactTypes.Title,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted
                    }).ToList(),

                Addresses = person.PersonAddresses
                    .Where(x => !x.IsDeleted)
                    .Select(a => new PersonAddressViewModel
                    {
                        Id = a.Id,
                        Address = a.Address,
                        PostalCode = a.PostalCode,
                        IsDefault = a.IsDefault,
                        ProvinceId = a.ProvinceId,
                        CityId = a.CityId,
                        ProvinceName = a.Provinces.Title,
                        CityName = a.Cities.Title,
                        IsActive = a.IsActive,
                        IsDeleted = a.IsDeleted
                    }).ToList(),

                Banks = person.PersonBanks
                    .Where(x => !x.IsDeleted)
                    .Select(b => new PersonBankViewModel
                    {
                        Id = b.Id,
                        BankName = b.BankName,
                        AccountNumber = b.AccountNumber,
                        CardNumber = b.CardNumber,
                        Shaba = b.Shaba,
                        IsDefault = b.IsDefault,
                        IsActive = b.IsActive,
                        IsDeleted = b.IsDeleted
                    }).ToList()
            };
>>>>>>> front
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
<<<<<<< HEAD
                    CreditLimit = x.CreditLimit,
                    AvailableCredit = x.AvailableCredit,
=======
<<<<<<< HEAD
                    PersonType = x.PersonType,
=======
>>>>>>> master
>>>>>>> front
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
