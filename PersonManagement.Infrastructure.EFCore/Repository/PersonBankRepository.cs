using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Domain.Person.PersonBankAgg;

namespace PersonManagement.Infrastructure.EFCore.Repository
{
    public class PersonBankRepository : RepositoryBase<long, PersonBanks>, IPersonBankRepository
    {
        private readonly PersonFakeDataContext _context;

        public PersonBankRepository(PersonFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditPersonBank GetDetails(long id)
        {
            return _context.PersonBanks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id)
                .Select(x => new EditPersonBank
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    CardNumber = x.CardNumber,
                    Shaba = x.Shaba,
                    BankBranchId = x.BankBranchId,
                    IsDefault = x.IsDefault
                })
                .FirstOrDefault();
        }

        public List<PersonBankViewModel> GetByPersonId(long personId)
        {
            return _context.PersonBanks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.PersonId == personId)
                .Select(x => new PersonBankViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    BankBranchId = x.BankBranchId,
                    AccountNumber = x.AccountNumber,
                    CardNumber = x.CardNumber,
                    Shaba = x.Shaba,
                    IsDefault = x.IsDefault,
                    BankBranchName = x.BankBranches.Title,
                    BankName = x.BankBranches.Banks.Title  
                })
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.Id)
                .ToList();
        }

        public PersonBankViewModel GetDefaultByPersonId(long personId)
        {
            return _context.PersonBanks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.PersonId == personId && x.IsDefault)
                .Select(x => new PersonBankViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    BankBranchId = x.BankBranchId,
                    AccountNumber = x.AccountNumber,
                    CardNumber = x.CardNumber,
                    Shaba = x.Shaba,
                    IsDefault = x.IsDefault,
                })
                .FirstOrDefault();
        }

        public List<PersonBankViewModel> Search(PersonBankSearchModel s)
        {
            var query = _context.PersonBanks
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new PersonBankViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    BankBranchId = x.BankBranchId,
                    AccountNumber = x.AccountNumber,
                    CardNumber = x.CardNumber,
                    Shaba = x.Shaba,
                    IsDefault = x.IsDefault,
                    BankBranchName = x.BankBranches.Title,
                    BankName = x.BankBranches.Banks.Title
                });

            if (s.PersonId.HasValue)
                query = query.Where(x => x.PersonId == s.PersonId.Value);

            if (s.BankBranchId.HasValue)
                query = query.Where(x => x.BankBranchId == s.BankBranchId.Value);

            if (!string.IsNullOrWhiteSpace(s.AccountNumber))
                query = query.Where(x => x.AccountNumber.Contains(s.AccountNumber.Trim()));

            if (!string.IsNullOrWhiteSpace(s.CardNumber))
                query = query.Where(x => x.CardNumber.Contains(s.CardNumber.Trim()));

            if (!string.IsNullOrWhiteSpace(s.Shaba))
                query = query.Where(x => x.Shaba.Contains(s.Shaba.Trim()));

            return query
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public void UnsetAllDefaults(long personId)
        {
            var defaults = _context.PersonBanks
                .Where(x => !x.IsDeleted && x.PersonId == personId && x.IsDefault)
                .ToList();

            foreach (var item in defaults)
                item.UnsetDefault();
        }
    }
}
