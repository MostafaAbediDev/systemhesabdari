namespace _0_FrameWork.Domain
{
    public class EntityBase
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
<<<<<<< HEAD
        public DateTime? DeletedAt { get; set; }
        public long? DeletedBy { get; set; }
=======
<<<<<<< HEAD
        public DateTime DeletedAt { get; set; }
        public long DeletedBy { get; set; }
=======
        public DateTime? DeletedAt { get; set; }
        public long? DeletedBy { get; set; }
>>>>>>> master
>>>>>>> front
        public DateTime CreationDate { get; set; }
        public EntityBase()
        {   
            CreationDate = DateTime.Now;
        }
    }
}
    