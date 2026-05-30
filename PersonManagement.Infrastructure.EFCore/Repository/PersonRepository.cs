using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Domain.Person.PersonAgg;
using System.Linq.Expressions;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonRepository : RepositoryBase<long, Persons>, IPersonRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonRepository(PersonFakeDataContext personFakeDataContext) : base(personFakeDataContext)
        {
            _context = personFakeDataContext;
        }

        public List<PersonViewModel> GetAllPersons()
        {
            return _context.Persons
                .AsNoTracking()
                .Include(x => x.Branches)
                .Select(x => new PersonViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    BranchName = x.Branches != null ? x.Branches.Title : string.Empty,
                    IsActive = x.IsActive,
                    IsLegal = x.IsLegal
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public EditPerson GetDetails(long id)
        {
            return _context.Persons
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new EditPerson
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    NationalCode = x.NationalCode,
                    EconomicCode = x.EconomicCode,
                    RegistrationNumber = x.RegistrationNumber,
                    PersonTypeId = x.PersonTypeId,
                    BranchId = x.BranchId,
                    IsLegal = x.IsLegal,
                    IsActive = x.IsActive
                })
                .FirstOrDefault();
        }

        // ✅ گرفتن جزئیات کامل با Include دقیق
        public PersonFullViewModel GetFullDetails(long id)
        {
            var person = _context.Persons
                .AsNoTracking()
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
                    FullName = person.FullName,
                    NationalCode = person.NationalCode,
                    EconomicCode = person.EconomicCode,
                    RegistrationNumber = person.RegistrationNumber,
                    BranchName = person.Branches?.Title ?? string.Empty,
                    IsActive = person.IsActive,
                    IsLegal = person.IsLegal
                },

                Contacts = person.PersonContacts?
                    .Where(x => !x.IsDeleted)
                    .Select(c => new PersonContactViewModel
                    {
                        Id = c.Id,
                        Value = c.Value,
                        Description = c.Description,
                        IsDefault = c.IsDefault,
                        ContactTypeId = c.ContactTypeId,
                        ContactTypeTitle = c.ContactTypes != null ? c.ContactTypes.Title : string.Empty,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted
                    }).ToList() ?? new List<PersonContactViewModel>(),

                Addresses = person.PersonAddresses?
                    .Where(x => !x.IsDeleted)
                    .Select(a => new PersonAddressViewModel
                    {
                        Id = a.Id,
                        Address = a.Address,
                        PostalCode = a.PostalCode,
                        IsDefault = a.IsDefault,
                        ProvinceId = a.ProvinceId,
                        CityId = a.CityId,
                        ProvinceName = a.Provinces != null ? a.Provinces.Title : string.Empty,
                        CityName = a.Cities != null ? a.Cities.Title : string.Empty,
                        IsActive = a.IsActive,
                        IsDeleted = a.IsDeleted
                    }).ToList() ?? new List<PersonAddressViewModel>(),

                Banks = person.PersonBanks?
                    .Where(x => !x.IsDeleted)
                    .Select(b => new PersonBankViewModel
                    {
                        Id = b.Id,
                        AccountNumber = b.AccountNumber,
                        CardNumber = b.CardNumber,
                        Shaba = b.Shaba,
                        IsDefault = b.IsDefault,
                        IsActive = b.IsActive,
                        IsDeleted = b.IsDeleted
                    }).ToList() ?? new List<PersonBankViewModel>()
            };
        }

        // ✅ Search با فیلتر پویا و AsNoTracking
        public List<PersonViewModel> Search(PersonSearchModel searchModel)
        {
            var query = _context.Persons
                .AsNoTracking()
                .Include(x => x.Branches)
                .Select(x => new PersonViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    NationalCode = x.NationalCode,
                    EconomicCode = x.EconomicCode,
                    RegistrationNumber = x.RegistrationNumber,
                    BranchName = x.Branches != null ? x.Branches.Title : string.Empty,
                    IsActive = x.IsActive,
                    IsLegal = x.IsLegal
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.FullName))
                query = query.Where(x => x.FullName.Contains(searchModel.FullName));

            if (!string.IsNullOrWhiteSpace(searchModel.NationalCode))
                query = query.Where(x => x.NationalCode == searchModel.NationalCode);

            if (!string.IsNullOrWhiteSpace(searchModel.EconomicCode))
                query = query.Where(x => x.EconomicCode == searchModel.EconomicCode);

            if (!string.IsNullOrWhiteSpace(searchModel.RegistrationNumber))
                query = query.Where(x => x.RegistrationNumber == searchModel.RegistrationNumber);

            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}
