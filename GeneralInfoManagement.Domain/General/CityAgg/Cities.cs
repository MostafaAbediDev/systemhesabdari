using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace GeneralInfoManagement.Domain.General.CityAgg
{
    public class Cities : EntityBase
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public long ProvinceId { get; private set; }
        public Provinces Provinces { get; private set; }

        public Cities(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public void Edit(string title, string description)
        {
            Title = title;
            Description = description;
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
