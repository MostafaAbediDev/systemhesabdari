using _0_FrameWork.Domain;

namespace GeneralInfoManagement.Domain.BaseInfo.PictureAgg
{
    public class Pictures : EntityBase
    {
        public long OwnerId { get; private set; }
        public PictureOwnerType OwnerType { get; private set; }
        public string Url { get; private set; }
        public bool IsMain { get; private set; }

        protected Pictures() { }

        public Pictures(long ownerId, PictureOwnerType ownerType, string url)
        {
            OwnerId = ownerId;
            OwnerType = ownerType;
            Url = url;
            IsMain = false;
        }

        public void SetAsMain()
        {
            IsMain = true;
        }

        public void RemoveMain()
        {
            IsMain = false;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
