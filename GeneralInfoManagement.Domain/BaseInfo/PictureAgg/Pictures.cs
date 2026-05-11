using _0_FrameWork.Domain;

namespace GeneralInfoManagement.Domain.BaseInfo.PictureAgg
{
    public class Pictures : EntityBase
    {
        public string EntityType { get; private set; }
        public long EntityId { get; private set; }
        public string ImageUrl { get; private set; }
        public bool IsMain { get; private set; }
      
        public Pictures(string entityType, long entityId, string imageUrl)
        {
            EntityType = entityType;
            EntityId = entityId;
            ImageUrl = imageUrl;
            IsMain = false;
        }

        public void Edit(string entityType, long entityId, string imageUrl)
        {
            EntityType = entityType;
            EntityId = entityId;
            ImageUrl = imageUrl;
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
