using _0_FrameWork.Infrastructure;
using GeneralInfoManagement.Application.Contract.Picture;
using GeneralInfoManagement.Domain.BaseInfo.PictureAgg;

namespace GeneralInfoManagement.Infrastructure.EFCore.Repository
{
    public class PictureRepository : RepositoryBase<long, Pictures>, IPictureRepository
    {
        private readonly GeneralInfoFakeDataContext _context;

        public PictureRepository(GeneralInfoFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditPicture GetDetails(long id)
        {
            return _context.Pictures
                .Select(x => new EditPicture
                {
                    Id = x.Id,
                    OwnerId = x.OwnerId,
                    OwnerType = (PictureOwnerTypeDTO)x.OwnerType,
                    Url = x.Url
                })
                .FirstOrDefault(x => x.Id == id);
        }

        public List<PictureViewModel> GetByOwner(long ownerId, PictureOwnerType ownerType)
        {
            return _context.Pictures
                .Where(x => x.OwnerId == ownerId && x.OwnerType == ownerType)
                .Select(x => new PictureViewModel
                {
                    Id = x.Id,
                    Url = x.Url,
                    IsMain = x.IsMain,
                    CreationDate = x.CreationDate.ToString()
                })
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public void SetAsMain(long ownerId, long pictureId)
        {
            var currentMains = _context.Pictures
                .Where(x => x.OwnerId == ownerId && x.IsMain)
                .ToList();

            foreach (var picture in currentMains)
            {
                picture.RemoveMain();
            }

            var newMain = _context.Pictures.FirstOrDefault(x => x.Id == pictureId);
            if (newMain != null)
            {
                newMain.SetAsMain();
            }

            _context.SaveChanges();
        }
    }
}
