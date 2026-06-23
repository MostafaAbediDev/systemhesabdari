using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;

namespace GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg
{
    public class Companies : EntityBase
    {
        public string Title { get; private set; }
        public string? Logo { get; private set; }
        public string LegalName { get; private set; }
        public DateTime EstablishedDate { get; private set; }
        public List<Branches> Branch { get; private set; }

        protected Companies()
        {
            Branch = new List<Branches>();

        }
        public Companies(string title, string? logo, string legalName, 
            DateTime establishedDate)
        {
            Title = title;
            Logo = logo;
            LegalName = legalName;
            EstablishedDate = establishedDate;
        }

        public void Edit(string title, string? logo, string legalName,
            DateTime establishedDate)
        {
            Title = title;
            Logo = logo;
            LegalName = legalName;
            EstablishedDate = establishedDate;
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
