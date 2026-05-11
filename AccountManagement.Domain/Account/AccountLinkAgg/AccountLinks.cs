using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;

namespace AccountManagement.Domain.Account.AccountLinkAgg
{
    public class AccountLinks : EntityBase
    {
        public int LinkType { get; private set; }
        public long ReferenceId { get; private set; }
        public long AccountId { get; private set; }
        public long CompanyId { get; private set; }
        public Accounts Account { get; private set; }
        public Companies Company { get; private set; }

        public AccountLinks(int linkType, long referenceId)
        {
            LinkType = linkType;
            ReferenceId = referenceId;
        }

        public void Edit(int linkType, long referenceId)
        {
            LinkType = linkType;
            ReferenceId = referenceId;
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
