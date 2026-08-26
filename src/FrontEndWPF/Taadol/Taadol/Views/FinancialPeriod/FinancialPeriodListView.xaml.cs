using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Models;

namespace Taadol.Views
{
    public partial class FinancialPeriodListView : UserControl
    {
        public ObservableCollection<FinancialPeriodItem> AllPeriods { get; set; }
        public ObservableCollection<FinancialPeriodItem> FilteredPeriods { get; set; }
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private CancellationTokenSource _loadCts = new();

        private readonly IFinancialPeriodApplication _financialPeriodApplication;
        private int _pageSize = 15;
        private string _currentFilter = "all";
        private string _searchText = "";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _filteredListCount = 0;
        private bool _isLoadedOnce = false;
        private bool _sizeWired = false;
        private HashSet<string> _selectedStatuses = new();
        private HashSet<string> _selectedTitles = new();

        public FinancialPeriodListView()
        {
            InitializeComponent();

            // نوار جمع‌بندی به شکاف زیر گرید منتقل می‌شود تا همیشه زیر گرید بچسبد
            // (اول از والد فعلی جدا می‌شود تا خطای «Must disconnect child» رخ ندهد)
            if (PeriodSummaryBar.Parent is Panel parent)
                parent.Children.Remove(PeriodSummaryBar);
            PeriodsGrid.Footer = PeriodSummaryBar;

            _financialPeriodApplication = App.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

            AllPeriods = new ObservableCollection<FinancialPeriodItem>();

            PeriodsGrid.NextPageRequested += (s, e) => GoToNextPage();
            PeriodsGrid.PreviousPageRequested += (s, e) => GoToPreviousPage();
            PeriodsGrid.PageRequested += (s, page) => GoToPage(page);
            PeriodsGrid.PageSizeRequested += (s, size) => ChangePageSize(size);
            PeriodsGrid.CheckedItemsChanged += (s, e) => UpdateSummary();

            // منوی راست‌کلیک ردیف: ویرایش
            PeriodsGrid.RowEditRequested += (s, item) =>
            {
                if (item is FinancialPeriodItem p && Window.GetWindow(this) is MainWindow mw)
                    mw.NavigateToEditFinancialPeriod(long.Parse(p.UniqueId));
            };

            // انتخاب همه = فقط همین صفحه؛ خاموش‌کردن هدر = پاک کردن انتخاب کل لیست
            PeriodsGrid.SelectAllToggled += (s, select) =>
            {
                if (select || AllPeriods == null) return;
                foreach (var item in AllPeriods.Where(p => !p.IsEmpty))
                    item.IsSelected = false;
            };

            // جستجو با Debounce داخلی SearchBoxControl (پیش‌فرض ۳۰۰ms)
            PeriodSearchBox.SearchTextChanged += (s, text) =>
            {
                _searchText = text.Trim();
                _currentPage = 1;
                ApplyFilters();
            };

            FillEmptyRows();

            Loaded += FinancialPeriodListView_Loaded;
            this.Unloaded += (s, e) => { _loadCts?.Cancel(); _loadCts?.Dispose(); _loadCts = null; };
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.CloseCurrentForm();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ToastManager.Warning("چاپ این بخش به‌زودی اضافه می‌شود.");
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = new CancellationTokenSource();
            _isLoadedOnce = false;
            try
            {
                await LoadDataAsync();
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[FinancialPeriodListView] Refresh was cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FinancialPeriodListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        // فرم لیست باید همیشه کل فضای محتوا رو پر کنه حتی اگه ردیف جدول کم باشه
        // (چون MainContent با Top/Left فقط اندازه محتوا رو می‌گیره) — مثل PersonListView
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
        /// رفرش داده‌های گرید از بیرون (مثلاً بعد از بسته‌شدن فرم «ویرایش دوره مالی» در مودال).
        /// </summary>
        public async Task RefreshGridAsync()
        {
            _isLoadedOnce = false;
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FinancialPeriodListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }


        private async void FinancialPeriodListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;

            _isLoadedOnce = true;

            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FinancialPeriodListView] Load periods error: {ex}");
                ToastManager.Error("خطا در بارگذاری دوره‌های مالی");
            }
        }

        private async Task LoadDataAsync()
        {
            PeriodsGrid.IsLoading = true;
            PeriodsGrid.LoadErrorText = null;

            try
            {
                await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();

                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

                    var periods = app.GetFinancialPeriods();

                    return periods.Select((p, index) => new FinancialPeriodItem
                    {
                        RowNumber = index + 1,
                        UniqueId = p.Id.ToString(),
                        Year = GetPersianYear(p.StartDate),
                        Title = p.Title,
                        BranchTitle = p.BranchTitle ?? "—",
                        StartDate = ToPersianDate(p.StartDate),
                        EndDate = ToPersianDate(p.EndDate),
                        Status = p.IsActive && !p.IsDeleted ? "فعال" : "غیرفعال",
                        IsEmpty = false
                    }).ToList();
                });

                AllPeriods = new ObservableCollection<FinancialPeriodItem>(items);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FinancialPeriodListView] Load periods error: {ex}");
                PeriodsGrid.LoadErrorText = "خطا در بارگذاری دوره‌های مالی";
                ToastManager.Error("خطا در لود دوره‌های مالی");

                AllPeriods = new ObservableCollection<FinancialPeriodItem>();

                ApplyFilters();
            }
            finally
            {
                PeriodsGrid.IsLoading = false;
            }
        }

        private void FillEmptyRows()
        {
            FilteredPeriods = new ObservableCollection<FinancialPeriodItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredPeriods.Add(new FinancialPeriodItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            PeriodsGrid.ItemsSource = FilteredPeriods;
        }

        private string GetPersianYear(string dateValue)
        {
            if (DateTime.TryParse(dateValue, out var date))
                return ToPersianNumber(new PersianCalendar().GetYear(date));

            return "—";
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
            if (AllPeriods == null)
                return;

            var selectedIds = AllPeriods.Where(p => !p.IsEmpty && p.IsSelected)
                                        .Select(p => p.UniqueId).ToHashSet();

            var query = AllPeriods.AsEnumerable();

            switch (_currentFilter)
            {
                case "active":
                    query = query.Where(p => p.Status == "فعال");
                    break;

                case "inactive":
                    query = query.Where(p => p.Status == "غیرفعال");
                    break;
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query = query.Where(p =>
                    (p.Title != null && p.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (p.BranchTitle != null && p.BranchTitle.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (_selectedStatuses.Count > 0)
                query = query.Where(p => _selectedStatuses.Contains(p.Status));

            if (_selectedTitles.Count > 0)
                query = query.Where(p => _selectedTitles.Contains(p.Title));

            var filteredList = query.ToList();
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

            FilteredPeriods = new ObservableCollection<FinancialPeriodItem>(pageItems);

            PeriodsGrid.ItemsSource = FilteredPeriods;

            BuildPaginationButtons();
            UpdateSummary();
            UpdateTabCounts();
        }

        private void UpdateTabCounts()
        {
            if (tabAll == null || tabActive == null || tabInactive == null || AllPeriods == null)
                return;

            int total = AllPeriods.Count(p => !p.IsEmpty);
            int active = AllPeriods.Count(p => !p.IsEmpty && p.Status == "فعال");
            int inactive = AllPeriods.Count(p => !p.IsEmpty && p.Status == "غیرفعال");

            tabAll.Tag = $"({ToPersianNumber(total)})";
            tabActive.Tag = $"({ToPersianNumber(active)})";
            tabInactive.Tag = $"({ToPersianNumber(inactive)})";
        }

        private void UpdateSummary()
        {
            if (PeriodSummaryBar == null) return;

            int selected = AllPeriods?.Count(p => p.IsSelected && !p.IsEmpty) ?? 0;
            int total = _filteredListCount;

            PeriodSummaryBar.SelectedSummaryText = $"دوره مالی انتخاب شده ({ToPersianNumber(selected)})";
            PeriodSummaryBar.TotalCountText = $"{ToPersianNumber(total)} مورد";
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
                PeriodsGrid.PagesSource = Pages;
                PeriodsGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredPeriods.Count(p => !p.IsEmpty))} از {ToPersianNumber(_filteredListCount)} مورد";
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
            PeriodsGrid.PagesSource = Pages;
            PeriodsGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredPeriods.Count(p => !p.IsEmpty))} از {ToPersianNumber(_filteredListCount)} مورد";
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

        private void FilterTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton tb) return;

            if (tb == tabAll) _currentFilter = "all";
            else if (tb == tabActive) _currentFilter = "active";
            else if (tb == tabInactive) _currentFilter = "inactive";

            tabAll.IsChecked = tb == tabAll;
            tabActive.IsChecked = tb == tabActive;
            tabInactive.IsChecked = tb == tabInactive;

            _currentPage = 1;
            ApplyFilters();
        }

        private void StatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر وضعیت",
                options: new List<string> { "فعال", "غیرفعال" },
                selected: _selectedStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    _selectedStatuses = new HashSet<string>(result);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        private void TitleFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllPeriods.Where(p => !p.IsEmpty && !string.IsNullOrEmpty(p.Title))
                                    .Select(p => p.Title).Distinct().OrderBy(x => x).ToList();
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر عنوان دوره",
                options: options,
                selected: _selectedTitles,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    _selectedTitles = new HashSet<string>(result);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        private void ShowFilterPopup(
            Button anchor,
            string title,
            List<string> options,
            HashSet<string> selected,
            bool showSearch,
            bool immediateApply,
            Action<List<string>> onSelectionChanged)
        {
            if (anchor == null) return;

            var popup = new Taadol.Controls.FilterPopupControl
            {
                Title = title,
                Options = options,
                SelectedOptions = new HashSet<string>(selected),
                ShowSearch = showSearch,
                ImmediateApply = immediateApply
            };

            popup.SelectionChanged += (selectedList) =>
            {
                onSelectionChanged(selectedList);
            };

            popup.ShowAt(anchor);
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

    public class FinancialPeriodItem : INotifyPropertyChanged, IListRowItem
    {
        private int _rowNumber;
        private bool _isSelected;

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

        public string UniqueId { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
        public string BranchTitle { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
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