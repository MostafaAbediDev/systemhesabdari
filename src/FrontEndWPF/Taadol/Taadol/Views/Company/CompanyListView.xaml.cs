using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Threading;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Taadol.Controls;
using Taadol.Models;

namespace Taadol.Views
{
    public partial class CompanyListView : UserControl
    {
        public ObservableCollection<CompanyItem> AllCompanies { get; set; }
        public ObservableCollection<CompanyItem> FilteredCompanies { get; set; }
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private CancellationTokenSource _loadCts = new();

        private readonly ICompanyApplication _companyApplication;
        private int _pageSize = 15;
        private string _currentFilter = "all";
        private string _searchText = "";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int filteredListCount = 0;
        private bool _isLoadedOnce = false;
        private bool _sizeWired = false;
        private HashSet<string> _selectedStatuses = new();
        private HashSet<string> _selectedTitles = new();

        public CompanyListView()
        {
            InitializeComponent();

            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            AllCompanies = new ObservableCollection<CompanyItem>();

            CompaniesGrid.NextPageRequested += (s, e) => GoToNextPage();
            CompaniesGrid.PreviousPageRequested += (s, e) => GoToPreviousPage();
            CompaniesGrid.PageRequested += (s, page) => GoToPage(page);
            CompaniesGrid.PageSizeRequested += (s, size) => ChangePageSize(size);
            CompaniesGrid.CheckedItemsChanged += (s, e) => UpdateSummary();

            // انتخاب همه = فقط همین صفحه؛ خاموش‌کردن هدر = پاک کردن انتخاب کل لیست
            CompaniesGrid.SelectAllToggled += (s, select) =>
            {
                if (select || AllCompanies == null) return;
                foreach (var item in AllCompanies.Where(c => !c.IsEmpty))
                    item.IsSelected = false;
            };

            // منوی راست‌کلیک ردیف: ویرایش و حذف
            CompaniesGrid.RowEditRequested += (s, item) =>
            {
                if (item is CompanyItem c && Window.GetWindow(this) is MainWindow mw)
                    mw.NavigateToEditCompany(c.Id);
            };

            CompaniesGrid.RowDeleteRequested += (s, item) =>
            {
                if (item is not CompanyItem c) return;

                foreach (var x in AllCompanies.Where(cc => !cc.IsEmpty))
                    x.IsSelected = ReferenceEquals(x, c);

                BtnDelete_Click(this, new RoutedEventArgs());
            };

            // جستجو با Debounce داخلی SearchBoxControl (پیش‌فرض ۳۰۰ms)
            CompanySearchBox.SearchTextChanged += (s, text) =>
            {
                _searchText = text.Trim();
                _currentPage = 1;
                ApplyFilters();
            };

            FillEmptyRows();

            Loaded += CompanyListView_Loaded;
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
                System.Diagnostics.Debug.WriteLine("[CompanyListView] Refresh was cancelled");
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در بروزرسانی: " + ex.Message);
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
        /// رفرش داده‌های گرید از بیرون (مثلاً بعد از بسته‌شدن فرم «ویرایش شرکت» در مودال).
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
                ToastManager.Error("خطا در بروزرسانی: " + ex.Message);
            }
        }


        private async void CompanyListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;

            _isLoadedOnce = true;

            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در بارگذاری شرکت‌ها: " + ex.Message);
            }
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            CompaniesGrid.IsLoading = true;

            try
            {
                var items = await System.Threading.Tasks.Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();

                    var companyApplication = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();

                    var companies = companyApplication.GetCompanies();

                    return companies.Select((c, index) => new CompanyItem
                    {
                        Id = c.Id,
                        RowNumber = index + 1,
                        RegisterDate = ToPersianDate(c.CreationDate),
                        Title = c.Title,
                        LegalName = c.LegalName,
                        Status = c.IsActive ? "فعال" : "غیرفعال",
                        IsEmpty = false
                    }).ToList();
                });

                AllCompanies = new ObservableCollection<CompanyItem>(items);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در لود شرکت‌ها: " + ex.Message);

                AllCompanies = new ObservableCollection<CompanyItem>();

                ApplyFilters();
            }
            finally
            {
                CompaniesGrid.IsLoading = false;
            }
        }

        private void FillEmptyRows()
        {
            FilteredCompanies = new ObservableCollection<CompanyItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredCompanies.Add(new CompanyItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            CompaniesGrid.ItemsSource = FilteredCompanies;
        }

        private string ToPersianDate(object dateValue)
        {
            if (dateValue == null)
                return "—";

            DateTime date;

            if (dateValue is DateTime dt)
            {
                date = dt;
            }
            else
            {
                if (!DateTime.TryParse(dateValue.ToString(), out date))
                    return "—";
            }

            PersianCalendar pc = new PersianCalendar();

            int year = pc.GetYear(date);
            int month = pc.GetMonth(date);
            int day = pc.GetDayOfMonth(date);

            return $"{year:0000}/{month:00}/{day:00}";
        }

        private void ApplyFilters()
        {
            if (AllCompanies == null) return;

            var selectedIds = AllCompanies.Where(c => !c.IsEmpty && c.IsSelected)
                                          .Select(c => c.Id.ToString()).ToHashSet();

            var query = AllCompanies.AsEnumerable();

            switch (_currentFilter)
            {
                case "active":
                    query = query.Where(c => c.Status == "فعال");
                    break;

                case "inactive":
                    query = query.Where(c => c.Status == "غیرفعال");
                    break;
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query = query.Where(c =>
                    (c.Title != null && c.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (c.LegalName != null && c.LegalName.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (_selectedStatuses.Count > 0)
                query = query.Where(c => _selectedStatuses.Contains(c.Status));

            if (_selectedTitles.Count > 0)
                query = query.Where(c => _selectedTitles.Contains(c.Title));

            var filteredList = query.ToList();
            filteredListCount = filteredList.Count;

            _totalPages = (int)Math.Ceiling(filteredList.Count / (double)_pageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            if (_currentPage < 1)
                _currentPage = 1;

            var pageItems = filteredList
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            foreach (var item in pageItems)
                item.IsSelected = selectedIds.Contains(item.Id.ToString());

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = ((_currentPage - 1) * _pageSize) + i + 1;

            FilteredCompanies = new ObservableCollection<CompanyItem>(pageItems);

            CompaniesGrid.ItemsSource = FilteredCompanies;

            BuildPaginationButtons();
            UpdateSummary();
            UpdateTabCounts();
        }

        private void UpdateTabCounts()
        {
            if (tabAll == null || tabActive == null || tabInactive == null || AllCompanies == null)
                return;

            int total = AllCompanies.Count(c => !c.IsEmpty);
            int active = AllCompanies.Count(c => !c.IsEmpty && c.Status == "فعال");
            int inactive = AllCompanies.Count(c => !c.IsEmpty && c.Status == "غیرفعال");

            tabAll.Tag = $"({ToPersianNumber(total)})";
            tabActive.Tag = $"({ToPersianNumber(active)})";
            tabInactive.Tag = $"({ToPersianNumber(inactive)})";
        }

        private void UpdateSummary()
        {
            if (CompanySummaryBar == null) return;

            int selected = AllCompanies?.Count(c => c.IsSelected && !c.IsEmpty) ?? 0;
            int total = filteredListCount;

            CompanySummaryBar.SelectedSummaryText = $"شرکت انتخاب شده ({ToPersianNumber(selected)})";
            CompanySummaryBar.TotalCountText = $"{ToPersianNumber(total)} مورد";
        }

        private void BuildPaginationButtons()
        {
            var pages = new ObservableCollection<PageItem>();

            if (_totalPages <= 7)
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
                CompaniesGrid.PagesSource = Pages;
                CompaniesGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredCompanies.Count(c => !c.IsEmpty))} از {ToPersianNumber(filteredListCount)} مورد";
                return;
            }

            pages.Add(new PageItem
            {
                PageNumber = 1,
                PageNumberDisplay = ToPersianNumber(1),
                IsCurrent = _currentPage == 1
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

            if (middleStart > 2)
            {
                pages.Add(new PageItem
                {
                    PageNumber = 0,
                    PageNumberDisplay = "...",
                    IsCurrent = false
                });
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

            if (middleEnd < _totalPages - 1)
            {
                pages.Add(new PageItem
                {
                    PageNumber = 0,
                    PageNumberDisplay = "...",
                    IsCurrent = false
                });
            }

            pages.Add(new PageItem
            {
                PageNumber = _totalPages,
                PageNumberDisplay = ToPersianNumber(_totalPages),
                IsCurrent = _currentPage == _totalPages
            });

            Pages = pages;
            CompaniesGrid.PagesSource = Pages;
            CompaniesGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredCompanies.Count(c => !c.IsEmpty))} از {ToPersianNumber(filteredListCount)} مورد";
        }

        public class PageItem
        {
            public int PageNumber { get; set; }
            public string PageNumberDisplay { get; set; }
            public bool IsCurrent { get; set; }
        }

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";

            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];

            return result;
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
            var options = AllCompanies.Where(c => !c.IsEmpty && !string.IsNullOrEmpty(c.Title))
                                      .Select(c => c.Title).Distinct().OrderBy(x => x).ToList();
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر عنوان شرکت",
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

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = AllCompanies
                .Where(c => c.IsSelected && !c.IsEmpty)
                .ToList();

            if (selectedItems.Count == 0)
            {
                ToastManager.Warning("لطفاً یک شرکت انتخاب کنید.");
                return;
            }

            var result = MessageBox.Show(
                $"آیا از حذف {selectedItems.Count} شرکت مطمئن هستید؟",
                "حذف شرکت",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                CompaniesGrid.IsLoading = true;

                await Task.Run(() =>
                {
                    foreach (var item in selectedItems.ToList())
                    {
                        _companyApplication.Remove(item.Id);
                    }
                });
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در حذف: " + ex.Message);
                return;
            }
            finally
            {
                CompaniesGrid.IsLoading = false;
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

    public class CompanyItem : INotifyPropertyChanged, IListRowItem
    {
        private int _rowNumber;
        private bool _isSelected;
        public long Id { get; set; }
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

        public string RegisterDate { get; set; }
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string LegalName { get; set; }
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
