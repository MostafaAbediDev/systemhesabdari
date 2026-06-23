using _0_FrameWork.Infrastructure;
using BankManagement.Application.Contracts.BankType;
using BankManagement.Domain.Bank.BankTypeAgg;

namespace BankManagement.Infrastructure.EFCore.Repository
{
    public class BankTypeRepository : RepositoryBase<long, BankTypes>, IBankTypeRepository
    {
        private readonly BankFakeDataContext _context;

        public BankTypeRepository(BankFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<BankTypeViewModel> GetBankTypes()
        {
            return _context.BankTypes
                            .Where(x => !x.IsDeleted && x.IsActive)
                            .OrderBy(x => x.TitleId)
                            .Select(x => new BankTypeViewModel
                            {
                                Id = x.Id,
                                TitleId = x.TitleId,
                                Title = x.Title
                            })
                            .ToList();
        }
    }
}
