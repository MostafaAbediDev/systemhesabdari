using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.General.CityAgg;

namespace GeneralInfoManagement.Domain.General.ProvinceAgg
{
    public class Provinces : EntityBase
    {
        public string Title { get; private set; }
        public List<Cities> Cities { get; private set; }
        public List<Branches> Branches { get; private set; }


        protected Provinces()
        {
            Cities = new List<Cities>();
            Branches = new List<Branches>();
        }

        public Provinces(string title)
        {
            Title = title;
            IsActive = true;
            IsDeleted = false;
        }

        public void Edit(string title)
        {
            Title = title;
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
