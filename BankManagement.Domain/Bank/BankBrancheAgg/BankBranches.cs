using _0_FrameWork.Domain;
using BankManagement.Domain.Bank.BankAgg;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;

namespace BankManagement.Domain.Bank.BankBrancheAgg
{
    public class BankBranches : EntityBase
    {
        public string BranchName { get; private set; }
        public string BranchCode { get; private set; }
        public string Address { get; private set; }
        public string Telephone { get; private set; }
        public long BankId { get; private set; }
        public long ProvinceId { get; private set; }
        public long CityId { get; private set; }
        public Banks Banks { get; private set; }
        public Provinces Provinces { get; private set; }
        public Cities Cities { get; private set; }

        public BankBranches(string branchName, string branchCode, string address, string telephone, long bankId, long provinceId, long cityId)
        {
            BranchName = branchName;
            BranchCode = branchCode;
            Address = address;
            Telephone = telephone;
            BankId = bankId;
            ProvinceId = provinceId;
            CityId = cityId;
        }

        public void Edit(string branchName, string branchCode, string address, string telephone, long bankId, long provinceId, long cityId)
        {
            BranchName = branchName;
            BranchCode = branchCode;
            Address = address;
            Telephone = telephone;
            BankId = bankId;
            ProvinceId = provinceId;
            CityId = cityId;
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
