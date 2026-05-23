using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Picture;

namespace GeneralInfoManagement.Domain.BaseInfo.PictureAgg
{
    public interface IPictureRepository : IRepository<long, Pictures>
    {
        EditPicture GetDetails(long id);
        List<PictureViewModel> GetByOwner(long ownerId, PictureOwnerType ownerType);
        void SetAsMain(long ownerId, long pictureId);
    }
}
