using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FinancialManagement.Application.Contracts.Fund;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;
using Taadol.Models;

namespace Taadol.Views.Fund
{
    public partial class FundListView : UserControl
    {
        public ObservableCollection<FundItem> AllFunds { get; set; }
        public ObservableCollection<FundItem> FilteredFunds { get; set; }
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private CancellationTokenSource _loadCts = new();

        private readonly IFundApplication _fundApplication;
        private int _pageSize = 15;
        private string _searchText = "";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _filteredListCount = 0;
        private bool _isLoadedOnce = false;
        private bool _sizeWired = false;
        private int _loadRequestVersion;
        private string _lastAppliedFilterKey;
        private string _cachedFilterKey;
        private List<FundItem> _cachedFilteredItems;

        public FundListView()
        {
            InitializeComponent();

            if (FundSummaryBar.Parent is Panel parent)
                parent.Children.Remove(FundSummaryBar);
            FundsGrid.Footer = FundSummaryBar;

            _fundApplication = App.ServiceProvider.GetRequiredService<IFundApplication>();

            AllFunds = new ObservableCollection<FundItem>();

            FundsGrid.NextPageRequested += (s, e) => GoToNextPage();
            FundsGrid.PreviousPageRequested += (s, e) => GoToPreviousPage();
            FundsGrid.PageRequested += (s, page) => GoToPage(page);
            FundsGrid.PageSizeRequested += (s, size) => ChangePageSize(size);
            FundsGrid.CheckedItemsChanged += (s, e) => UpdateSummary();

            FundsGrid.RowEditRequested += (s, item) =>
            {
                if (item is FundItem)
                    ToastManager.Info("ویرایش صندوق به‌زودی اضافه می‌شود.");
            };

            FundsGrid.RowDeleteRequested += (s, item) =>
            {
                if (item is not FundItem f) return;

                foreach (var x in AllFunds.Where(ff => !ff.IsEmpty))
                    x.IsSelected = ReferenceEquals(x, f);

                BtnDelete_Click(this, new RoutedEventArgs());
            };

            FundsGrid.SelectAllToggled += (s, select) =>
            {
                if (select || AllFunds == null) return;
                foreach (var item in AllFunds.Where(f => !f.IsEmpty))
                    item.IsSelected = false;
            };

            FundSearchBox.SearchTextChanged += (s, text) =>
            {
                _searchText = text.Trim();
                _currentPage = 1;
                ApplyFilters();
            };

            FillEmptyRows();

            Loaded += FundListView_Loaded;
            this.Unloaded += (s, e) =>
            {
                var current = Interlocked.Exchange(ref _loadCts, null);
                if (current != null)
                {
                    try { current.Cancel(); } catch (ObjectDisposedException) { }
                    current.Dispose();
                }
            };
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.CloseCurrentForm();
        }

        private void BtnNewFund_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.NavigateTo(NavKeys.FundNew);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ToastManager.Warning("چاپ این بخش به‌زودی اضافه می‌شود.");
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            var current = Interlocked.Exchange(ref _loadCts, null);
            if (current != null)
            {
                try { current.Cancel(); } catch (ObjectDisposedException) { }
                current.Dispose();
            }
            _isLoadedOnce = false;
            try
            {
                await LoadDataAsync();
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[FundListView] Refresh was cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FundListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FillAvailableSpace();
            }), DispatcherPriority.Background);
        }

        private void FillAvailableSpace()
        {
            if (Window.GetWindow(this) is not Window window) return;
            if (window.FindName("MainContentBorder") is not Border border) return;

            if (!_sizeWired)
            {
                _sizeWired = true;
                border.SizeChanged += (s, _) => FillAvailableSpace();
            }

            var pad = border.Padding;
            var margin = Margin;
            var w = border.ActualWidth - pad.Left - pad.Right - margin.Left - margin.Right;
            var h = border.ActualHeight - pad.Top - pad.Bottom - margin.Top - margin.Bottom;
            Width = w > 0 ? w : 0;
            Height = h > 0 ? h : 0;
        }

        public async Task RefreshGridAsync()
        {
            var current = Interlocked.Exchange(ref _loadCts, null);
            if (current != null)
            {
                try { current.Cancel(); } catch (ObjectDisposedException) { }
                current.Dispose();
            }
            _isLoadedOnce = false;
            try
            {
                await LoadDataAsync();
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[FundListView] RefreshGridAsync was cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FundListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        private async void FundListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;

            _isLoadedOnce = true;

            try
            {
                await LoadDataAsync();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FundListView] Load funds error: {ex}");
                ToastManager.Error("خطا در بارگذاری صندوق‌ها");
            }
        }

        private async Task LoadDataAsync()
        {
            var requestVersion = Interlocked.Increment(ref _loadRequestVersion);
            var loadCts = new CancellationTokenSource();
            var previousCts = Interlocked.Exchange(ref _loadCts, loadCts);
            previousCts?.Cancel();
            var cancellationToken = loadCts.Token;
            _lastAppliedFilterKey = null;
            _cachedFilterKey = null;
            _cachedFilteredItems = null;
            FundsGrid.IsLoading = true;
            FundsGrid.LoadErrorText = null;

            try
            {
                await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();

                    var app = scope.ServiceProvider.GetRequiredService<IFundApplication>();

                    var funds = app.GetFunds();

                    return funds.Select((f, index) => new FundItem
                    {
                        Id = f.Id,
                        UniqueId = f.Id.ToString(),
                        RowNumber = index + 1,
                        Title = f.Title,
                        BranchTitle = f.BranchTitle ?? "—",
                        RegisterDate = ToPersianDate(f.CreationDate),
                        IsEmpty = false
                    }).ToList();
                }, cancellationToken);

                if (requestVersion != Volatile.Read(ref _loadRequestVersion))
                    return;

                AllFunds = new ObservableCollection<FundItem>(items);

                ApplyFilters();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FundListView] Load funds error: {ex}");
                if (requestVersion != Volatile.Read(ref _loadRequestVersion))
                    return;

                FundsGrid.LoadErrorText = "خطا در بارگذاری صندوق‌ها";
                ToastManager.Error("خطا در لود صندوق‌ها");

                AllFunds = new ObservableCollection<FundItem>();

                ApplyFilters();
            }
            finally
            {
                if (requestVersion == Volatile.Read(ref _loadRequestVersion))
                    FundsGrid.IsLoading = false;
                Interlocked.CompareExchange(ref _loadCts, null, loadCts);
                loadCts.Dispose();
            }
        }

        private void FillEmptyRows()
        {
            FilteredFunds = new ObservableCollection<FundItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredFunds.Add(new FundItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            FundsGrid.ItemsSource = FilteredFunds;
        }

        private string ToPersianDate(string dateValue)
        {
            if (string.IsNullOrWhiteSpace(dateValue))
                return "—";

            if (!DateTime.TryParse(dateValue, out var date))
                return "—";

            PersianCalendar pc = new PersianCalendar();

            int year = pc.GetYear(date);
            int month = pc.GetMonth(date);
            int day = pc.GetDayOfMonth(date);

            return $"{year:0000}/{month:00}/{day:00}";
        }

        private void ApplyFilters()
        {
            if (AllFunds == null)
                return;

            var filterKey = BuildFilterKey();
            if (filterKey == _lastAppliedFilterKey)
                return;

            var selectedIds = AllFunds.Where(f => !f.IsEmpty && f.IsSelected)
                                      .Select(f => f.UniqueId).ToHashSet();

            var filterOnlyKey = BuildFilterOnlyKey();
            List<FundItem> filteredList;
            if (filterOnlyKey == _cachedFilterKey && _cachedFilteredItems != null)
            {
                filteredList = _cachedFilteredItems;
            }
            else
            {
                var query = AllFunds.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    query = query.Where(f =>
                        (f.Title != null && f.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (f.BranchTitle != null && f.BranchTitle.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
                }

                filteredList = query.ToList();
                _cachedFilterKey = filterOnlyKey;
                _cachedFilteredItems = filteredList;
            }

            _filteredListCount = filteredList.Count;

            _totalPages = (int)Math.Ceiling(filteredList.Count / (double)_pageSize);

            if (_totalPages < 1)
                _totalPages = 1;

            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            if (_currentPage < 1)
                _currentPage = 1;

            int skip = (_currentPage - 1) * _pageSize;

            var pageItems = filteredList
                .Skip(skip)
                .Take(_pageSize)
                .ToList();

            foreach (var item in pageItems)
                item.IsSelected = selectedIds.Contains(item.UniqueId);

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = skip + i + 1;

            FilteredFunds = new ObservableCollection<FundItem>(pageItems);

            FundsGrid.ItemsSource = FilteredFunds;

            BuildPaginationButtons();
            UpdateSummary();
            _lastAppliedFilterKey = filterKey;
        }

        private string BuildFilterOnlyKey()
        {
            return string.Join("|", _searchText);
        }

        private string BuildFilterKey()
        {
            return string.Join("|", _currentPage, _pageSize, _searchText);
        }

        private void UpdateSummary()
        {
            if (FundSummaryBar == null) return;

            int selected = AllFunds?.Count(f => f.IsSelected && !f.IsEmpty) ?? 0;
            int total = _filteredListCount;

            FundSummaryBar.SelectedSummaryText = $"صندوق انتخاب شده ({ToPersianNumber(selected)})";
            FundSummaryBar.TotalCountText = $"{ToPersianNumber(total)} مورد";
        }

        private void BuildPaginationButtons()
        {
            var pages = new ObservableCollection<PageItem>();

            if (_totalPages <= 5)
            {
                for (int i = 1; i <= _totalPages; i++)
                {
                    pages.Add(new PageItem
                    {
                        PageNumber = i,
                        PageNumberDisplay = ToPersianNumber(i),
                        IsCurrent = i == _currentPage
                    });
                }

                Pages = pages;
                FundsGrid.PagesSource = Pages;
                FundsGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredFunds.Count(f => !f.IsEmpty))} از {ToPersianNumber(_filteredListCount)} مورد";
                return;
            }

            pages.Add(new PageItem
            {
                PageNumber = 1,
                PageNumberDisplay = ToPersianNumber(1),
                IsCurrent = _currentPage == 1
            });

            pages.Add(new PageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });

            int middleStart = _currentPage - 1;
            int middleEnd = _currentPage + 1;

            if (_currentPage <= 3)
            {
                middleStart = 2;
                middleEnd = 4;
            }
            else if (_currentPage >= _totalPages - 2)
            {
                middleStart = _totalPages - 3;
                middleEnd = _totalPages - 1;
            }

            for (int i = middleStart; i <= middleEnd; i++)
            {
                if (i > 1 && i < _totalPages)
                {
                    pages.Add(new PageItem
                    {
                        PageNumber = i,
                        PageNumberDisplay = ToPersianNumber(i),
                        IsCurrent = i == _currentPage
                    });
                }
            }

            pages.Add(new PageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });

            pages.Add(new PageItem
            {
                PageNumber = _totalPages,
                PageNumberDisplay = ToPersianNumber(_totalPages),
                IsCurrent = _currentPage == _totalPages
            });

            Pages = pages;
            FundsGrid.PagesSource = Pages;
            FundsGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredFunds.Count(f => !f.IsEmpty))} از {ToPersianNumber(_filteredListCount)} مورد";
        }

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            string result = "";

            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];

            return result;
        }

        public class PageItem
        {
            public int PageNumber { get; set; }
            public string PageNumberDisplay { get; set; }
            public bool IsCurrent { get; set; }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = AllFunds
                .Where(f => f.IsSelected && !f.IsEmpty)
                .ToList();

            if (selectedItems.Count == 0)
            {
                ToastManager.Warning("لطفاً یک صندوق انتخاب کنید.");
                return;
            }

            var result = MessageBox.Show(
                $"آیا از حذف {selectedItems.Count} صندوق مطمئن هستید؟",
                "حذف صندوق",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                FundsGrid.IsLoading = true;

                await Task.Run(() =>
                {
                    foreach (var item in selectedItems.ToList())
                    {
                        _fundApplication.Remove(item.Id);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FundListView] Delete error: {ex}");
                ToastManager.Error("خطا در حذف");
                return;
            }
            finally
            {
                FundsGrid.IsLoading = false;
            }

            _currentPage = 1;
            await LoadDataAsync();

            ToastManager.Success("عملیات حذف انجام شد.");
        }

        private void GoToNextPage()
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyFilters();
            }
        }

        private void GoToPreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFilters();
            }
        }

        private void GoToPage(int pageNumber)
        {
            if (pageNumber <= 0) return;
            _currentPage = pageNumber;
            ApplyFilters();
        }

        private void ChangePageSize(int newSize)
        {
            _pageSize = newSize;
            _currentPage = 1;
            ApplyFilters();
        }
    }

    public class FundItem : INotifyPropertyChanged, IListRowItem
    {
        private int _rowNumber;
        private bool _isSelected;

        public long Id { get; set; }
        public string UniqueId { get; set; }

        public int RowNumber
        {
            get => _rowNumber;
            set
            {
                _rowNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumber)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumberDisplay)));
            }
        }

        public string Title { get; set; }
        public string BranchTitle { get; set; }
        public string RegisterDate { get; set; }
        public bool IsEmpty { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public string RowNumberDisplay => RowNumber > 0 && !IsEmpty ? ToPersianNumber(RowNumber) : "";

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";
            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
