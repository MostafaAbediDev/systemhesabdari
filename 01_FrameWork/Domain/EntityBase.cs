namespace _0_FrameWork.Domain
{
    public class EntityBase
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime DeletedAt { get; set; }
        public long DeletedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public EntityBase()
        {
            CreationDate = DateTime.Now;
        }
    }
}
