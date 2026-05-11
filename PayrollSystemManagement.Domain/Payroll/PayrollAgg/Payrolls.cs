using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;

namespace PayrollSystemManagement.Domain.Payroll.PayrollAgg
{
    public class Payrolls : EntityBase
    {
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Status { get; private set; }
        public decimal TotalBenefits { get; private set; }
        public decimal TotalDeduction { get; private set; }
        public decimal NetPay { get; private set; }
        public long BranchId { get; private set; }
        public long EmployeeId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public long AccountingDocumentId { get; private set; }
        public Branches Branches { get; private set; }
        public Employees Employees { get; private set; }
        public FinancialPeriods FinancialPeriods { get; private set; }
        public AccountingDocuments AccountingDocuments { get; private set; }
        public List<PayrollDetails> PayrollDetails { get; private set; }

        protected Payrolls()
        {
            PayrollDetails = new List<PayrollDetails>();
        }
        public Payrolls(int year, int month, int status, decimal totalBenefits, decimal totalDeduction, decimal netPay)
        {
            Year = year;
            Month = month;
            Status = status;
            TotalBenefits = totalBenefits;
            TotalDeduction = totalDeduction;
            NetPay = netPay;
        }

        public void Edit(int year, int month, int status, decimal totalBenefits, decimal totalDeduction, decimal netPay)
        {
            Year = year;
            Month = month;
            Status = status;
            TotalBenefits = totalBenefits;
            TotalDeduction = totalDeduction;
            NetPay = netPay;
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
