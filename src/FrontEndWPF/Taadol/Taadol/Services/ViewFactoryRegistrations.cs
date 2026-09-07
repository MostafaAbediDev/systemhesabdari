using System;
using System.Windows.Controls;
using Taadol.Helpers;
using Taadol.Views;
using Taadol.Views.Fund;

namespace Taadol.Services
{
    /// <summary>
    /// Centralized registration of all navigable views into the ViewFactory.
    /// Keeps MainWindow constructor lean and provides one place to add new views.
    /// </summary>
    public static class ViewFactoryRegistrations
    {
        /// <summary>
        /// Registers all standard navigable views.
        /// Some registrations that require MainWindow-scoped state (e.g., event wiring)
        /// are passed as delegates from MainWindow.
        /// </summary>
        public static void RegisterDefaults(
            ViewFactory factory,
            Func<UserControl> bankListFactory,
            Func<UserControl> newBankFactory)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (bankListFactory is null) throw new ArgumentNullException(nameof(bankListFactory));
            if (newBankFactory is null) throw new ArgumentNullException(nameof(newBankFactory));

            factory.Register(NavKeys.PersonList, () => new PersonListView());
            factory.Register(NavKeys.PersonNew, () => new NewPersonView());

            factory.Register(NavKeys.ProductList, () => new ProductListView());
            factory.Register(NavKeys.ProductNew, () => new NewProductView());

            factory.Register(NavKeys.CompanyInfo, () => new NewCompanyView());
            factory.Register(NavKeys.CompanyList, () => new CompanyListView());

            factory.Register(NavKeys.BranchNew, () => new NewBranchView());
            factory.Register(NavKeys.BranchList, () => new BranchListView());
            factory.Register(NavKeys.BranchArchive, () => new BranchArchiveListView());

            factory.Register(NavKeys.FinancialPeriod, () => new FinancialPeriodListView());
            factory.Register(NavKeys.FinancialPeriodNew, () => new NewFinancialPeriodView());
            factory.Register(NavKeys.BankList, bankListFactory);
            factory.Register(NavKeys.BankNew, newBankFactory);
            factory.Register(NavKeys.FundList, () => new FundListView());
            factory.Register(NavKeys.FundNew, () => new NewFundView());
        }
    }
}