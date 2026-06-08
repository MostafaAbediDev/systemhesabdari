using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountingDocumentAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;

namespace PayrollSystemManagement.Domain.Payroll.PayrollAgg
{
    public class Payrolls : EntityBase
    {
        
        public long BranchId { get; private set; }
        public long EmployeeId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public long? AccountingDocumentId { get; private set; }
        public PayrollStatus Status { get; private set; }
        public decimal TotalBenefits {  get; private set; }
        public decimal TotalDeduction {  get; private set; }
        public decimal NetPay => TotalBenefits - TotalDeduction;
        public Branches Branches { get; private set; }
        public Employees Employees { get; private set; }
        public FinancialPeriods FinancialPeriods { get; private set; }
        public AccountingDocuments AccountingDocuments { get; private set; }
        public List<PayrollDetails> PayrollDetails { get; private set; }

        protected Payrolls()
        {
            PayrollDetails = new List<PayrollDetails>();
        }
        public Payrolls(long employeeId, long branchId, long financialPeriodId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId is invalid");

            if (branchId <= 0)
                throw new ArgumentException("BranchId is invalid");

            if (financialPeriodId <= 0)
                throw new ArgumentException("FinancialPeriodId is invalid");

            EmployeeId = employeeId;
            BranchId = branchId;
            FinancialPeriodId = financialPeriodId;

            Status = PayrollStatus.Draft;
        }

        public void Edit(long employeeId, long branchId, long financialPeriodId)
        {
            if (Status != PayrollStatus.Draft)
                throw new InvalidOperationException("Only Draft payroll can be edited");

            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId is invalid");

            if (branchId <= 0)
                throw new ArgumentException("BranchId is invalid");

            if (financialPeriodId <= 0)
                throw new ArgumentException("FinancialPeriodId is invalid");

            EmployeeId = employeeId;
            BranchId = branchId;
            FinancialPeriodId = financialPeriodId;
        }

        public void Approve()
        {
            if (Status != PayrollStatus.Draft)
                throw new InvalidOperationException("Only Draft payroll can be approved");

            Status = PayrollStatus.Approved;
        }

        public void Pay(long accountingDocumentId)
        {
            if (Status != PayrollStatus.Approved)
                throw new InvalidOperationException("Payroll must be approved first");

            if (accountingDocumentId <= 0)
                throw new ArgumentException("AccountingDocumentId is invalid");

            AccountingDocumentId = accountingDocumentId;
            Status = PayrollStatus.Paid;
        }

        public void Cancel()
        {
            if (Status is PayrollStatus.Paid or PayrollStatus.Cancelled)
                throw new InvalidOperationException("Paid payroll cannot be cancelled");

            Status = PayrollStatus.Cancelled;
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
