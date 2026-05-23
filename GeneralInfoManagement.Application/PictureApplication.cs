using _0_Framework.Application;
using _0_FrameWork.Domain;
using GeneralInfoManagement.Application.Contract.Picture;
using GeneralInfoManagement.Domain.BaseInfo.PictureAgg;

namespace GeneralInfoManagement.Application
{
    public class PictureApplication : IPictureApplication
    {
        private readonly IPictureRepository _pictureRepository;

        public PictureApplication(IPictureRepository pictureRepository)
        {
            _pictureRepository = pictureRepository;
        }

        public OperationResult Create(CreatePicture command)
        {
            var operation = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.Url))
                return operation.Failed("آدرس تصویر نمی‌تواند خالی باشد.");

            if (command.OwnerId <= 0)
                return operation.Failed("صاحب تصویر (Owner) به درستی مشخص نشده است.");

            var picture = new Pictures(command.OwnerId, (PictureOwnerType)command.OwnerType, command.Url);

            _pictureRepository.Create(picture);
            _pictureRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Edit(EditPicture command)
        {
            var operation = new OperationResult();

            var picture = _pictureRepository.Get(command.Id);
            if (picture == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            if (string.IsNullOrWhiteSpace(command.Url))
                return operation.Failed("آدرس تصویر نمی‌تواند خالی باشد.");

            picture.Edit(command.OwnerId, (PictureOwnerType)command.OwnerType, command.Url);

            _pictureRepository.SaveChanges();

            return operation.Succedded();
        }

        public List<PictureViewModel> GetByOwner(long ownerId, PictureOwnerTypeDTO ownerType)
        {
            return _pictureRepository.GetByOwner(ownerId, (PictureOwnerType) ownerType);
        }

        public EditPicture GetDetails(long id)
        {
            return _pictureRepository.GetDetails(id);
        }

        public OperationResult Activate(long id)
        {
            var operation = new OperationResult();

            var picture = _pictureRepository.Get(id);
            if (picture == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            picture.Active();
            _pictureRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var operation = new OperationResult();

            var picture = _pictureRepository.Get(id);
            if (picture == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            picture.NotActive();
            _pictureRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var operation = new OperationResult();

            var picture = _pictureRepository.Get(id);
            if (picture == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            picture.Remove();
            _pictureRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var operation = new OperationResult();

            var picture = _pictureRepository.Get(id);
            if (picture == null)
                return operation.Failed("رکورد مورد نظر یافت نشد.");

            picture.Restore();
            _pictureRepository.SaveChanges();

            return operation.Succedded();
        }

        public OperationResult SetAsMain(long id, long ownerId)
        {
            var operation = new OperationResult();

            _pictureRepository.SetAsMain(id, ownerId);

            return operation.Succedded();
        }
    }
}
