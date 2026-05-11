using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.General.CityAgg;

namespace GeneralInfoManagement.Domain.General.ProvinceAgg
{
    public class Provinces : EntityBase
    {
        public string Title { get; private set; }
        public string Desscription { get; private set; }
        public List<Cities> Cities { get; private set; }

        protected Provinces()
        {
            Cities = new List<Cities>();
        }

        public Provinces(string title, string desscription)
        {
            Title = title;
            Desscription = desscription;
        }

        public void Edit(string title, string desscription)
        {
            Title = title;
            Desscription = desscription;
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
