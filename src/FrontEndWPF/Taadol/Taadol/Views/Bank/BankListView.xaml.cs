using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Taadol.Controls;
using Taadol.Helpers;
using Taadol.Models;
using Taadol.ViewModels;

namespace Taadol.Views.Bank
{
    public partial class BankListView : UserControl
    {
        public BankListViewModel ViewModel { get; }

        private bool _isLoadedOnce;
        private bool _sizeWired;

        public event Action<long>? EditBankRequestedForModal;

        public BankListView()
        {
            InitializeComponent();

            if (BankSummaryBar.Parent is Panel parent)
                parent.Children.Remove(BankSummaryBar);
            BanksGrid.Footer = BankSummaryBar;

            ViewModel = new BankListViewModel(App.ServiceProvider);
            DataContext = ViewModel;

            BanksGrid.NextPageRequested += (_, _) => ViewModel.GoToNextPage();
            BanksGrid.PreviousPageRequested += (_, _) => ViewModel.GoToPreviousPage();
            BanksGrid.PageRequested += (_, page) => ViewModel.GoToPage(page);
            BanksGrid.PageSizeRequested += (_, size) => ViewModel.ChangePageSize(size);
            BanksGrid.CheckedItemsChanged += (_, _) => UpdateSelectedBank();

            BanksGrid.RowEditRequested += BanksGrid_RowEditRequested;
            BanksGrid.RowDeleteRequested += BanksGrid_RowDeleteRequested;
            ViewModel.BankEditRequested += ViewModel_BankEditRequested;
            BanksGrid.SelectAllToggled += BanksGrid_SelectAllToggled;

            BankSearchBox.SearchTextChanged += BankSearchBox_SearchTextChanged;
            Loaded += BankListView_Loaded;
            Unloaded += BankListView_Unloaded;
        }

        private void BankListView_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.BankEditRequested -= ViewModel_BankEditRequested;
            ViewModel.CancelPendingLoads();
            ViewModel.Dispose();
            Unloaded -= BankListView_Unloaded;
        }

        private void BankSearchBox_SearchTextChanged(object sender, string text)
        {
            ViewModel.HandleSearchTextChanged(text);
            BanksGrid.RefreshVisualState();
        }

        private void BanksGrid_SelectAllToggled(object? sender, bool select)
        {
            if (!select)
                ViewModel.ClearSelection();
            else
                ViewModel.SyncSelection();
        }

        private void BanksGrid_RowEditRequested(object? sender, IListRowItem item)
        {
            if (item is BankItem bank)
                ViewModel.EditBankCommand.Execute(bank);
        }

        private void ViewModel_BankEditRequested(BankItem bank)
        {
            if (bank == null || bank.IsEmpty)
                return;

            EditBankRequestedForModal?.Invoke(bank.Id);
        }

        private void BanksGrid_RowDeleteRequested(object? sender, IListRowItem item)
        {
            if (item is not BankItem bank)
                return;

            ViewModel.SelectOnly(bank);
            ViewModel.DeleteBankCommand.Execute(null);
        }

        private void UpdateSelectedBank()
        {
            ViewModel.SyncSelection();
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.CloseCurrentForm();
        }

        private void CountryFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                sender as Button,
                "فیلتر کشور",
                ViewModel.AvailableCountries.ToList(),
                ViewModel.SelectedCountries,
                selected => ViewModel.SetCountryFilter(selected));
        }

        private void BankTypeFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                sender as Button,
                "فیلتر نوع بانک",
                ViewModel.AvailableBankTypes.ToList(),
                ViewModel.SelectedBankTypes,
                selected => ViewModel.SetBankTypeFilter(selected));
        }

        private static void ShowFilterPopup(
            Button? anchor,
            string title,
            List<string> options,
            IEnumerable<string> selected,
            Action<List<string>> onSelectionChanged)
        {
            if (anchor == null)
                return;

            var popup = new FilterPopupControl
            {
                Title = title,
                Options = options,
                SelectedOptions = new HashSet<string>(selected),
                ShowSearch = options.Count > 5,
                ImmediateApply = false
            };

            popup.SelectionChanged += onSelectionChanged;
            popup.ShowAt(anchor);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ToastManager.Warning("چاپ این بخش به‌زودی اضافه می‌شود.");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(FillAvailableSpace), DispatcherPriority.Background);
        }

        private void FillAvailableSpace()
        {
            if (Window.GetWindow(this) is not Window window)
                return;
            if (window.FindName("MainContentBorder") is not Border border)
                return;

            if (!_sizeWired)
            {
                _sizeWired = true;
                border.SizeChanged += (_, _) => FillAvailableSpace();
            }

            var pad = border.Padding;
            var margin = Margin;
            Width = Math.Max(0, border.ActualWidth - pad.Left - pad.Right - margin.Left - margin.Right);
            Height = Math.Max(0, border.ActualHeight - pad.Top - pad.Bottom - margin.Top - margin.Bottom);
        }

        private async void BankListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce)
                return;

            _isLoadedOnce = true;
            try
            {
                await ViewModel.LoadDataAsync();
            }
            catch (OperationCanceledException)
            {
                // لغو بارگذاری رفتار عادی هنگام خروج از صفحه است.
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListView] خطا در بارگذاری بانک‌ها: {exception}");
                ToastManager.Error("خطا در بارگذاری بانک‌ها");
            }
        }

        public async Task RefreshGridAsync()
        {
            _isLoadedOnce = false;
            try
            {
                await ViewModel.RefreshAsync();
                _isLoadedOnce = true;
            }
            catch (OperationCanceledException)
            {
                // لغو رفرش رفتار عادی است.
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListView] خطا در رفرش: {exception}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

    }
}
