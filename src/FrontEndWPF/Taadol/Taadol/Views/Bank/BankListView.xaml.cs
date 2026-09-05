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
using BankManagement.Application.Contracts.Bank;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;
using Taadol.Models;

namespace Taadol.Views.Bank
{
    public partial class BankListView : UserControl
    {
        public ObservableCollection<BankItem> AllBanks { get; set; }
        public ObservableCollection<BankItem> FilteredBanks { get; set; }
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private CancellationTokenSource _loadCts = new();

        private readonly IBankApplication _bankApplication;
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
        private List<BankItem> _cachedFilteredItems;

        public BankListView()
        {
            InitializeComponent();

            // نوار جمع‌بندی به شکاف زیر گرید منتقل می‌شود تا همیشه زیر گرید بچسبد
            if (BankSummaryBar.Parent is Panel parent)
                parent.Children.Remove(BankSummaryBar);
            BanksGrid.Footer = BankSummaryBar;

            _bankApplication = App.ServiceProvider.GetRequiredService<IBankApplication>();

            AllBanks = new ObservableCollection<BankItem>();

            BanksGrid.NextPageRequested += (s, e) => GoToNextPage();
            BanksGrid.PreviousPageRequested += (s, e) => GoToPreviousPage();
            BanksGrid.PageRequested += (s, page) => GoToPage(page);
            BanksGrid.PageSizeRequested += (s, size) => ChangePageSize(size);
            BanksGrid.CheckedItemsChanged += (s, e) => UpdateSummary();

            // منوی راست‌کلیک ردیف: ویرایش (فعلاً در دسترس نیست) و حذف
            BanksGrid.RowEditRequested += (s, item) =>
            {
                if (item is BankItem)
                    ToastManager.Info("ویرایش بانک به‌زودی اضافه می‌شود.");
            };

            BanksGrid.RowDeleteRequested += (s, item) =>
            {
                if (item is not BankItem b) return;

                foreach (var x in AllBanks.Where(bb => !bb.IsEmpty))
                    x.IsSelected = ReferenceEquals(x, b);

                BtnDelete_Click(this, new RoutedEventArgs());
            };

            // انتخاب همه = فقط همین صفحه؛ خاموش‌کردن هدر = پاک کردن انتخاب کل لیست
            BanksGrid.SelectAllToggled += (s, select) =>
            {
                if (select || AllBanks == null) return;
                foreach (var item in AllBanks.Where(b => !b.IsEmpty))
                    item.IsSelected = false;
            };

            // جستجو با Debounce داخلی SearchBoxControl (پیش‌فرض ۳۰۰ms)
            BankSearchBox.SearchTextChanged += (s, text) =>
            {
                _searchText = text.Trim();
                _currentPage = 1;
                ApplyFilters();
            };

            FillEmptyRows();

            Loaded += BankListView_Loaded;
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

        private void BtnNewBank_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.NavigateTo(NavKeys.BankNew);
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
                System.Diagnostics.Debug.WriteLine("[BankListView] Refresh was cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        // فرم لیست باید همیشه کل فضای محتوا رو پر کنه حتی اگه ردیف جدول کم باشه
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

        /// <summary>
        /// رفرش داده‌های گرید از بیرون (مثلاً بعد از بسته‌شدن فرم «بانک جدید»).
        /// </summary>
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
                System.Diagnostics.Debug.WriteLine("[BankListView] RefreshGridAsync was cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        private async void BankListView_Loaded(object sender, RoutedEventArgs e)
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
                System.Diagnostics.Debug.WriteLine($"[BankListView] Load banks error: {ex}");
                ToastManager.Error("خطا در بارگذاری بانک‌ها");
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
            BanksGrid.IsLoading = true;
            BanksGrid.LoadErrorText = null;

            try
            {
                await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();

                    var app = scope.ServiceProvider.GetRequiredService<IBankApplication>();

                    var banks = app.Search(new BankSearchModel());

                    return banks.Select((b, index) => new BankItem
                    {
                        Id = b.Id,
                        UniqueId = b.Id.ToString(),
                        RowNumber = index + 1,
                        Title = b.Title,
                        Country = string.IsNullOrWhiteSpace(b.Country) ? "—" : b.Country,
                        BankType = b.BankType ?? "",
                        RegisterDate = ToPersianDate(b.CreationDate),
                        IsEmpty = false
                    }).ToList();
                }, cancellationToken);

                if (requestVersion != Volatile.Read(ref _loadRequestVersion))
                    return;

                AllBanks = new ObservableCollection<BankItem>(items);

                ApplyFilters();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListView] Load banks error: {ex}");
                if (requestVersion != Volatile.Read(ref _loadRequestVersion))
                    return;

                BanksGrid.LoadErrorText = "خطا در بارگذاری بانک‌ها";
                ToastManager.Error("خطا در لود بانک‌ها");

                AllBanks = new ObservableCollection<BankItem>();

                ApplyFilters();
            }
            finally
            {
                if (requestVersion == Volatile.Read(ref _loadRequestVersion))
                    BanksGrid.IsLoading = false;
                Interlocked.CompareExchange(ref _loadCts, null, loadCts);
                loadCts.Dispose();
            }
        }

        private void FillEmptyRows()
        {
            FilteredBanks = new ObservableCollection<BankItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredBanks.Add(new BankItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            BanksGrid.ItemsSource = FilteredBanks;
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
            if (AllBanks == null)
                return;

            var filterKey = BuildFilterKey();
            if (filterKey == _lastAppliedFilterKey)
                return;

            var selectedIds = AllBanks.Where(b => !b.IsEmpty && b.IsSelected)
                                      .Select(b => b.UniqueId).ToHashSet();

            var filterOnlyKey = BuildFilterOnlyKey();
            List<BankItem> filteredList;
            if (filterOnlyKey == _cachedFilterKey && _cachedFilteredItems != null)
            {
                filteredList = _cachedFilteredItems;
            }
            else
            {
                var query = AllBanks.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    query = query.Where(b =>
                        (b.Title != null && b.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (b.Country != null && b.Country.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (b.BankType != null && b.BankType.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
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

            FilteredBanks = new ObservableCollection<BankItem>(pageItems);

            BanksGrid.ItemsSource = FilteredBanks;

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
            if (BankSummaryBar == null) return;

            int selected = AllBanks?.Count(b => b.IsSelected && !b.IsEmpty) ?? 0;
            int total = _filteredListCount;

            BankSummaryBar.SelectedSummaryText = $"بانک انتخاب شده ({ToPersianNumber(selected)})";
            BankSummaryBar.TotalCountText = $"{ToPersianNumber(total)} مورد";
        }

        private void BuildPaginationButtons()
        {
            var pages = new ObservableCollection<PageItem>();

            // آستانه ۵ صفحه: با ۵ صفحه یا کمتر همه‌ی شماره‌ها بدون نقطه‌چین
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
                BanksGrid.PagesSource = Pages;
                BanksGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredBanks.Count(b => !b.IsEmpty))} از {ToPersianNumber(_filteredListCount)} مورد";
                return;
            }

            pages.Add(new PageItem
            {
                PageNumber = 1,
                PageNumberDisplay = ToPersianNumber(1),
                IsCurrent = _currentPage == 1
            });

            // نقطه‌چین اول — همیشه ثابت در هر دو طرف (مطابق لیست اشخاص)
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

            // نقطه‌چین دوم — همیشه ثابت
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
            BanksGrid.PagesSource = Pages;
            BanksGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredBanks.Count(b => !b.IsEmpty))} از {ToPersianNumber(_filteredListCount)} مورد";
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
            var selectedItems = AllBanks
                .Where(b => b.IsSelected && !b.IsEmpty)
                .ToList();

            if (selectedItems.Count == 0)
            {
                ToastManager.Warning("لطفاً یک بانک انتخاب کنید.");
                return;
            }

            var result = MessageBox.Show(
                $"آیا از حذف {selectedItems.Count} بانک مطمئن هستید؟",
                "حذف بانک",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                BanksGrid.IsLoading = true;

                await Task.Run(() =>
                {
                    foreach (var item in selectedItems.ToList())
                    {
                        _bankApplication.Remove(item.Id);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListView] Delete error: {ex}");
                ToastManager.Error("خطا در حذف");
                return;
            }
            finally
            {
                BanksGrid.IsLoading = false;
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

    public class BankItem : INotifyPropertyChanged, IListRowItem
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
        public string Country { get; set; }
        public string BankType { get; set; }
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
