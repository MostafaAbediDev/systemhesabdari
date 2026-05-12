using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace GeneralInfoManagement.Domain.General.CityAgg
{
    public class Cities : EntityBase
    {
        public string Title { get; private set; }
        public long ProvinceId { get; private set; }
        public Provinces Provinces { get; private set; }

        protected Cities()
        {
        }

        public Cities(string title, long provinceId)
        {
            Title = title;
            ProvinceId = provinceId;
            IsActive = true;
            IsDeleted = false;
        }

        public void Edit(string title, long provinceId)
        {
            Title = title;
            ProvinceId = provinceId;
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
