using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Threading;
using Taadol.Models;

namespace Taadol.Views
{
    public partial class BranchListView : UserControl
    {
        public ObservableCollection<BranchItem> AllBranches { get; set; }
        public ObservableCollection<BranchItem> FilteredBranches { get; set; }
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private readonly IBranchApplication _branchApplication;
        private int _pageSize = 15;
        private string _currentFilter = "all";
        private string _searchText = "";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _isLoadedOnce = false;
        private HashSet<string> _selectedStatuses = new();
        private HashSet<string> _selectedCompanyNames = new();
        private HashSet<string> _selectedProvinces = new();
        private HashSet<string> _selectedCities = new();

        public BranchListView()
        {
            InitializeComponent();

            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();

            AllBranches = new ObservableCollection<BranchItem>();

            BranchesGrid.NextPageRequested += (s, e) => GoToNextPage();
            BranchesGrid.PreviousPageRequested += (s, e) => GoToPreviousPage();
            BranchesGrid.PageRequested += (s, page) => GoToPage(page);
            BranchesGrid.PageSizeRequested += (s, size) => ChangePageSize(size);
            BranchesGrid.CheckedItemsChanged += (s, e) => UpdateSummary();

            // انتخاب همه = فقط همین صفحه؛ خاموش‌کردن هدر = پاک کردن انتخاب کل لیست
            BranchesGrid.SelectAllToggled += (s, select) =>
            {
                if (select || AllBranches == null) return;
                foreach (var item in AllBranches.Where(b => !b.IsEmpty))
                    item.IsSelected = false;
            };

            BranchSearchBox.TextChanged += (s, e) =>
            {
                _searchText = BranchSearchBox.Text.Trim();
                _currentPage = 1;
                ApplyFilters();
            };

            FillEmptyRows();

            Loaded += BranchListView_Loaded;
        }

        private async void BranchListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;

            _isLoadedOnce = true;

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            BranchesGrid.IsLoading = true;

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();

                    var branchApplication = scope.ServiceProvider.GetRequiredService<IBranchApplication>();

                    var branches = branchApplication.GetBranches();

                    return branches.Select((b, index) => new BranchItem
                    {
                        RowNumber = index + 1,
                        RegisterDate = ToPersianDate(b.CreatedAt),
                        UniqueId = b.Id.ToString(),
                        BranchType = "—",
                        CompanyName = b.CompanyName ?? "—",
                        BranchName = b.Title,
                        RegistrationNumber = b.RegisterNumber,
                        BranchCode = b.Code,
                        Province = b.ProvinceName,
                        City = b.CityName,
                        Phone = b.TelePhone,
                        Mobile = b.MobilePhone,
                        Address = b.Address,
                        Status = b.IsActive ? "فعال" : "غیرفعال",
                        IsEmpty = false
                    }).ToList();
                });

                AllBranches = new ObservableCollection<BranchItem>(items);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شعبه‌ها", MessageBoxButton.OK, MessageBoxImage.Error);

                AllBranches = new ObservableCollection<BranchItem>();

                ApplyFilters();
            }
            finally
            {
                BranchesGrid.IsLoading = false;
            }
        }

        private void FillEmptyRows()
        {
            FilteredBranches = new ObservableCollection<BranchItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredBranches.Add(new BranchItem
                {
                    RowNumber = i,
                    IsEmpty = true
                });
            }

            BranchesGrid.ItemsSource = FilteredBranches;
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
            if (AllBranches == null)
                return;

            var selectedIds = AllBranches.Where(b => !b.IsEmpty && b.IsSelected)
                                         .Select(b => b.UniqueId).ToHashSet();

            var query = AllBranches.AsEnumerable();

            switch (_currentFilter)
            {
                case "active":
                    query = query.Where(b => b.Status == "فعال");
                    break;

                case "inactive":
                    query = query.Where(b => b.Status == "غیرفعال");
                    break;
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query = query.Where(b =>
                    (b.BranchName != null && b.BranchName.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (b.BranchCode != null && b.BranchCode.Contains(_searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (b.RegistrationNumber != null && b.RegistrationNumber.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (_selectedStatuses.Count > 0)
                query = query.Where(b => _selectedStatuses.Contains(b.Status));

            if (_selectedCompanyNames.Count > 0)
                query = query.Where(b => _selectedCompanyNames.Contains(b.CompanyName));

            if (_selectedProvinces.Count > 0)
                query = query.Where(b => _selectedProvinces.Contains(b.Province));

            if (_selectedCities.Count > 0)
                query = query.Where(b => _selectedCities.Contains(b.City));

            var filteredList = query.ToList();
            filteredListCount = filteredList.Count;

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

            FilteredBranches = new ObservableCollection<BranchItem>(pageItems);

            int realCount = FilteredBranches.Count;

            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredBranches.Add(new BranchItem
                {
                    RowNumber = i,
                    IsEmpty = true
                });
            }

            BranchesGrid.ItemsSource = FilteredBranches;

            BuildPaginationButtons();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            if (BranchSummaryBar == null) return;

            int selected = AllBranches?.Count(b => b.IsSelected && !b.IsEmpty) ?? 0;
            int total = filteredListCount;

            BranchSummaryBar.SelectedSummaryText = $"شعبه انتخاب شده ({ToPersianNumber(selected)})";
            BranchSummaryBar.TotalCountText = $"{ToPersianNumber(total)} مورد";
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
                BranchesGrid.PagesSource = Pages;
                BranchesGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredBranches.Count(b => !b.IsEmpty))} از {ToPersianNumber(filteredListCount)} مورد";
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
            BranchesGrid.PagesSource = Pages;
            BranchesGrid.PageInfoContent = $"نمایش {ToPersianNumber(FilteredBranches.Count(b => !b.IsEmpty))} از {ToPersianNumber(filteredListCount)} مورد";
        }

        private int filteredListCount = 0;

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

        private void CompanyNameFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllBranches.Where(b => !b.IsEmpty && !string.IsNullOrEmpty(b.CompanyName))
                                     .Select(b => b.CompanyName).Distinct().OrderBy(x => x).ToList();
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر نام شرکت",
                options: options,
                selected: _selectedCompanyNames,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    _selectedCompanyNames = new HashSet<string>(result);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        private void ProvinceFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllBranches.Where(b => !b.IsEmpty && !string.IsNullOrEmpty(b.Province))
                                     .Select(b => b.Province).Distinct().OrderBy(x => x).ToList();
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر استان",
                options: options,
                selected: _selectedProvinces,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    _selectedProvinces = new HashSet<string>(result);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        private void CityFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllBranches.Where(b => !b.IsEmpty && !string.IsNullOrEmpty(b.City))
                                     .Select(b => b.City).Distinct().OrderBy(x => x).ToList();
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر شهر",
                options: options,
                selected: _selectedCities,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    _selectedCities = new HashSet<string>(result);
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
            var selectedItems = AllBranches.Where(b => b.IsSelected && !b.IsEmpty).ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("لطفاً یک شعبه انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"آیا از حذف {selectedItems.Count} شعبه مطمئن هستید؟", "حذف",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                foreach (var item in selectedItems.ToList())
                {
                    if (long.TryParse(item.UniqueId, out var branchId))
                        _branchApplication.Remove(branchId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف: " + ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _currentPage = 1;
            await LoadDataAsync();

            MessageBox.Show("شعبه‌های انتخاب‌شده حذف شدند.", "حذف", MessageBoxButton.OK, MessageBoxImage.Information);
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

    public class BranchItem : INotifyPropertyChanged, IListRowItem
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

        public string RegisterDate { get; set; }
        public string UniqueId { get; set; }
        public string BranchType { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string RegistrationNumber { get; set; }
        public string BranchCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
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
