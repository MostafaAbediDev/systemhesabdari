namespace Taadol.Helpers
{
    /// <summary>
    /// کلیدهای ناوبری (Navigation Keys) — جایگزین Magic Strings در MainWindow و صفحات.
    /// مقادیر باید دقیقاً با Tag های تعریف‌شده در SidebarControl.xaml یکسان باشند.
    /// </summary>
    public static class NavKeys
    {
        // اشخاص
        public const string PersonList = "person_list";
        public const string PersonNew = "person_new";

        // محصولات / خدمات
        public const string ProductList = "product_list";
        public const string ProductNew = "product_new";

        // شرکت‌ها
        public const string CompanyInfo = "company_info";
        public const string CompanyList = "company_list";

        // شعب
        public const string BranchNew = "branch_new";
        public const string BranchList = "branch_list";
        public const string BranchArchive = "branch_archive";

        // دوره‌های مالی
        public const string FinancialPeriod = "financial_period";
        public const string FinancialPeriodNew = "financial_period_new";

        // بانک
        public const string BankList = "bank_list";
        public const string BankNew = "bank_new";

        // صندوق
        public const string FundList = "fund_list";
        public const string FundNew = "fund_new";
    }
}
