using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Models;

namespace Taadol.Views
{
    public partial class FinancialPeriodListView : UserControl
    {
        public ObservableCollection<FinancialPeriodItem> AllPeriods { get; set; }
        public ObservableCollection<FinancialPeriodItem> FilteredPeriods { get; set; }
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private readonly IFinancialPeriodApplication _financialPeriodApplication;
        private int _pageSize = 15;
        private string _currentFilter = "all";
        private string _searchText = "";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _filteredListCount = 0;
        private bool _isLoadedOnce = false;

        public FinancialPeriodListView()
        {
            InitializeComponent();

            _financialPeriodApplication = App.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

            AllPeriods = new ObservableCollection<FinancialPeriodItem>();

            PeriodsGrid.NextPageRequested += (s, e) => GoToNextPage();
            PeriodsGrid.PreviousPageRequested += (s, e) => GoToPreviousPage();
            PeriodsGrid.PageRequested += (s, page) => GoToPage(page);
            PeriodsGrid.PageSizeRequested += (s, size) => ChangePageSize(size);
            PeriodsGrid.CheckedItemsChanged += (s, e) => UpdateSummary();

            PeriodSearchBox.TextChanged += (s, e) =>
            {
                _searchText = PeriodSearchBox.Text.Trim();
                _currentPage = 1;
                ApplyFilters();
            };

            FillEmptyRows();

            Loaded += FinancialPeriodListView_Loaded;
        }

        private async void FinancialPeriodListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;

            _isLoadedOnce = true;

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            PeriodsGrid.IsLoading = true;

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            try
            {
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
                MessageBox.Show(ex.Message, "خطا در لود دوره‌های مالی", MessageBoxButton.OK, MessageBoxImage.Error);

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
                    RowNumber = i,
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

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = skip + i + 1;

            FilteredPeriods = new ObservableCollection<FinancialPeriodItem>(pageItems);

            int realCount = FilteredPeriods.Count;

            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredPeriods.Add(new FinancialPeriodItem
                {
                    RowNumber = i,
                    IsEmpty = true
                });
            }

            PeriodsGrid.ItemsSource = FilteredPeriods;

            BuildPaginationButtons();
            UpdateSummary();
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

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فرم ثبت دوره مالی جدید", "دوره مالی جدید", MessageBoxButton.OK, MessageBoxImage.Information);
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