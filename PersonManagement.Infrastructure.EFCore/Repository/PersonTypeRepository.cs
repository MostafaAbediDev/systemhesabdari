using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.PersonTypes;
using PersonManagement.Domain.Person.PersonTypeAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonTypeRepository : RepositoryBase<long, PersonType>, IPersonTypeRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonTypeRepository(PersonFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<PersonTypeViewModel> GetPersonTypes()
        {
            return _context.PersonTypes
                            .AsNoTracking()
                            .Where(x => !x.IsDeleted && x.IsActive)
                            .OrderBy(x => x.TitleId)
                            .Select(x => new PersonTypeViewModel
                            {
                                Id = x.Id,
                                TitleId = x.TitleId,
                                Title = x.Title
                            })
                            .ToList();
        }
    }
}
