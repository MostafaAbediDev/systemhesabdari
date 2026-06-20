using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Domain.Person.PersonAddressAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonAddressRepository : RepositoryBase<long, PersonAddresses>, IPersonAddressRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonAddressRepository(PersonFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<PersonAddressViewModel> GetByPersonId(long personId)
        {
            return _context.PersonAddresses
                            .AsNoTracking()
                            .Where(x => x.PersonId == personId && !x.IsDeleted)
                            .OrderByDescending(x => x.IsDefault)
                            .ThenByDescending(x => x.Id)
                            .Select(x => new PersonAddressViewModel
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Address = x.Address,
                                PostalCode = x.PostalCode,
                                PersonId = x.PersonId,
                                PersonFirstName = x.Persons.FirstName,
                                PersonLastName = x.Persons.LastName,
                                ProvinceId = x.ProvinceId,
                                ProvinceName = x.Provinces.Title,
                                CityId = x.CityId,
                                CityName = x.Cities.Title
                            })
                            .ToList();
        }

        public PersonAddresses GetDefault(long personId)
        {
            return _context.PersonAddresses
                            .FirstOrDefault(x =>
                                x.PersonId == personId &&
                                x.IsDefault &&
                                !x.IsDeleted);
        }

        public EditPersonAddress GetDetails(long id)
        {
            return _context.PersonAddresses
                            .AsNoTracking()
                            .Where(x => x.Id == id)
                            .Select(x => new EditPersonAddress
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Address = x.Address,
                                PostalCode = x.PostalCode,
                                PersonId = x.PersonId,
                                ProvinceId = x.ProvinceId,
                                CityId = x.CityId,
                                IsDefault = x.IsDefault
                            })
                            .FirstOrDefault();
        }

        public List<PersonAddressViewModel> Search(PersonAddressSearchModel searchModel)
        {
            var query = _context.PersonAddresses
                            .AsNoTracking()
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
                query = query.Where(x => x.Title.Contains(searchModel.Title));

            if (searchModel.PersonId > 0)
                query = query.Where(x => x.PersonId == searchModel.PersonId);

            if (searchModel.ProvinceId > 0)
                query = query.Where(x => x.ProvinceId == searchModel.ProvinceId);

            if (searchModel.CityId > 0)
                query = query.Where(x => x.CityId == searchModel.CityId);

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new PersonAddressViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Address = x.Address,
                    PostalCode = x.PostalCode,
                    PersonId = x.PersonId,
                    PersonFirstName = x.Persons.FirstName,
                    PersonLastName = x.Persons.LastName,
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.Provinces.Title,
                    CityId = x.CityId,
                    CityName = x.Cities.Title
                })
                .ToList();
        }
    }
}
