using _0_FrameWork.Domain;
using AccountManagement.Domain.Account.AccountAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;

namespace AccountManagement.Domain.AccountOpeningBalanceAgg
{
    public class AccountOpeningBalance : EntityBase
    {
        public long AccountId { get; private set; }
        public long FinancialPeriodId { get; private set; }
        public decimal Debit { get; private set; }
        public decimal Credit { get; private set; }
        public Accounts Account { get; private set; }
        public FinancialPeriods FinancialPeriod { get; private set; }


        protected AccountOpeningBalance()
        {
        }


        public AccountOpeningBalance(
            long accountId,
            long financialPeriodId,
            decimal debit,
            decimal credit)
        {
            AccountId = accountId;
            FinancialPeriodId = financialPeriodId;
            Debit = debit;
            Credit = credit;
        }
    }
}
