using _0_Framework.Application;

namespace GeneralInfoManagement.Application.Contract.Picture
{
    public interface IPictureApplication
    {
        OperationResult Create(CreatePicture command);
        OperationResult Edit(EditPicture command);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult SetAsMain(long id, long ownerId);
        EditPicture GetDetails(long id);
        List<PictureViewModel> GetByOwner(long ownerId, PictureOwnerTypeDTO ownerType);
    }
}
