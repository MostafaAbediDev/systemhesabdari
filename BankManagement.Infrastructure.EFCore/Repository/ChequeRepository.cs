using _0_FrameWork.Infrastructure;
using BankManagement.Application.Contracts.Cheque;
using BankManagement.Domain.Bank.ChequeAgg;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Infrastructure.EFCore.Repository
{
    internal class ChequeRepository : RepositoryBase<long, Cheques>, IChequeRepository
    {
        private readonly BankFakeDataContext _context;

        public ChequeRepository(BankFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<ChequeViewModel> GetCheques()
        {
            return _context.Cheques
                .Include(x => x.ChequeBooks)
                .Include(x => x.Branches)
                .OrderByDescending(x => x.Id)
                .Select(x => new ChequeViewModel
                {
                    Id = x.Id,
                    ChequeNumber = x.ChequeNumber,
                    ChequeType = x.ChequeType.ToString(),
                    Amount = x.Amount,
                    DueDate = x.DueDate.ToString("yyyy/MM/dd"),
                    Status = x.Status.ToString(),
                    ReferenceType = x.ReferenceType.ToString(),
                    ChequeBookSerial = x.ChequeBooks.SerialNumber,
                    BranchName = x.Branches.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd"),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                })
                .ToList();
        }

        public EditCheque GetDetails(long id)
        {
            return _context.Cheques
                .Select(x => new EditCheque
            {
                Id = x.Id,
                ChequeType = (ChequeTypeDTO)x.ChequeType,
                ChequeNumber = x.ChequeNumber,
                Amount = x.Amount,
                DueDate = x.DueDate,
                ReferenceType = (ChequeReferenceTypeDTO)x.ReferenceType,
                ReferenceId = x.ReferenceId,
                ChequeBookId = x.ChequeBookId,
                BranchId = x.BranchId
            })
            .FirstOrDefault(x => x.Id == id);
        }

        public List<ChequeViewModel> Search(ChequeSearchModel searchModel)
        {
            var query = _context.Cheques
                .Include(x => x.ChequeBooks)
                .Include(x => x.Branches)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.ChequeNumber))
                query = query.Where(x => x.ChequeNumber.Contains(searchModel.ChequeNumber));

            if (searchModel.ChequeType.HasValue)
                query = query.Where(x => x.ChequeType == (ChequeType)searchModel.ChequeType.Value);

            if (searchModel.Status.HasValue)
                query = query.Where(x => x.Status == (ChequeStatus)searchModel.Status.Value);

            if (searchModel.ChequeBookId.HasValue)
                query = query.Where(x => x.ChequeBookId == searchModel.ChequeBookId.Value);

            if (searchModel.BranchId.HasValue)
                query = query.Where(x => x.BranchId == searchModel.BranchId.Value);

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new ChequeViewModel
                {
                    Id = x.Id,
                    ChequeNumber = x.ChequeNumber,
                    ChequeType = x.ChequeType.ToString(),
                    Amount = x.Amount,
                    DueDate = x.DueDate.ToString("yyyy/MM/dd"),
                    Status = x.Status.ToString(),
                    ReferenceType = x.ReferenceType.ToString(),
                    ChequeBookSerial = x.ChequeBooks.SerialNumber,
                    BranchName = x.Branches.Title,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd"),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                })
                .ToList();
        }
    }
}
