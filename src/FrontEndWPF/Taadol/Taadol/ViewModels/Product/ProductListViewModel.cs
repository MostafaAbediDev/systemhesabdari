using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Taadol.Models;

namespace Taadol.ViewModels.Product
{
    public partial class ProductListViewModel : ObservableObject
    {
        private List<ProductListItem> _allProductsList = new();
        private int _totalPages = 1;

        public ProductListViewModel()
        {
            AllProducts = new ObservableCollection<ProductListItem>();
            FilteredProducts = new ObservableCollection<ProductListItem>();
            Pages = new ObservableCollection<PageItem>();

            LoadMockData();
            ApplyFilters();
        }

        [ObservableProperty]
        private ObservableCollection<ProductListItem> _allProducts;

        [ObservableProperty]
        private ObservableCollection<ProductListItem> _filteredProducts;

        [ObservableProperty]
        private ObservableCollection<PageItem> _pages;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 15;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _loadErrorText = string.Empty;

        [ObservableProperty]
        private string _pageInfoText = string.Empty;

        [ObservableProperty]
        private string _selectedTab = "all";

        [ObservableProperty]
        private string _selectedStatus = "all";

        [RelayCommand]
        private void Search()
        {
            CurrentPage = 1;
            ApplyFilters();
        }

        [RelayCommand]
        private void FilterByTab(string tab)
        {
            SelectedTab = tab;
            CurrentPage = 1;
            ApplyFilters();
        }

        [RelayCommand]
        private void FilterByStatus(string status)
        {
            SelectedStatus = status;
            CurrentPage = 1;
            ApplyFilters();
        }

        [RelayCommand]
        private void GoToPage(int page)
        {
            if (page < 1 || page > _totalPages) return;
            CurrentPage = page;
            ApplyFilters();
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            if (CurrentPage < _totalPages)
            {
                CurrentPage++;
                ApplyFilters();
            }
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                ApplyFilters();
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadProductsAsync();
        }

        public void ChangePageSize(int newSize)
        {
            if (newSize < 5 || newSize > 100) return;
            PageSize = newSize;
            CurrentPage = 1;
            ApplyFilters();
        }

        private async Task LoadProductsAsync()
        {
            IsLoading = true;
            LoadErrorText = string.Empty;

            try
            {

                await Task.Delay(500);
                LoadMockData();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                LoadErrorText = $"خطا در بارگذاری: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var query = _allProductsList.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.Trim();
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Category) && p.Category.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Brand) && p.Brand.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.Contains(search)));
            }

            if (SelectedTab == "products")
                query = query.Where(p => p.ItemType == "کالا");
            else if (SelectedTab == "services")
                query = query.Where(p => p.ItemType == "خدمات");

            if (SelectedStatus == "active")
                query = query.Where(p => p.IsActive);
            else if (SelectedStatus == "inactive")
                query = query.Where(p => !p.IsActive);

            var filtered = query.ToList();

            _totalPages = Math.Max(1, (int)Math.Ceiling((double)filtered.Count / (double)PageSize));
            if (CurrentPage > _totalPages) CurrentPage = _totalPages;

            var paged = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            FilteredProducts.Clear();
            foreach (var item in paged)
                FilteredProducts.Add(item);

            BuildPages();
            UpdatePageInfo(filtered.Count);
        }

        private void BuildPages()
        {
            Pages.Clear();
            for (int i = 1; i <= _totalPages; i++)
            {
                Pages.Add(new PageItem
                {
                    PageNumber = i,
                    PageNumberDisplay = ToPersianDigits(i),
                    IsCurrent = i == CurrentPage
                });
            }
        }

        private void UpdatePageInfo(int totalItems)
        {
            var currentPageCount = Math.Min(PageSize, totalItems - (CurrentPage - 1) * PageSize);
            if (currentPageCount < 0) currentPageCount = 0;
            PageInfoText = $"نمایش {ToPersianDigits(currentPageCount)} از {ToPersianDigits(totalItems)} مورد";
        }

        private void LoadMockData()
        {
            _allProductsList = new List<ProductListItem>
            {

                new() { Id = 1, ProductCode = "PRD-001", ProductName = "کفش نایک ایرمکس ۲۰۲۴", Category = "کفش", Brand = "نایک", Barcode = "6281100123456", SalePrice = 2500000, IsActive = true, ItemType = "کالا" },
                new() { Id = 2, ProductCode = "PRD-002", ProductName = "کفش اداری مردانه چرم", Category = "کفش", Brand = "辩驳", Barcode = "6281100234567", SalePrice = 1200000, IsActive = true, ItemType = "کالا" },
                new() { Id = 3, ProductCode = "PRD-003", ProductName = "کفش ورزشی زنانه آدیداس", Category = "کفش", Brand = "آدیداس", Barcode = "6281100345678", SalePrice = 2800000, IsActive = true, ItemType = "کالا" },
                new() { Id = 4, ProductCode = "PRD-004", ProductName = "کفش پیاده‌روی مردانه", Category = "کفش", Brand = "asics", Barcode = "6281100456789", SalePrice = 1950000, IsActive = false, ItemType = "کالا" },
                new() { Id = 5, ProductCode = "PRD-005", ProductName = "کفش مجلسی زنانه", Category = "کفش", Brand = "manato", Barcode = "6281100567890", SalePrice = 3200000, IsActive = true, ItemType = "کالا" },

                new() { Id = 6, ProductCode = "PRD-006", ProductName = "کیف چرم طبیعی دستی", Category = "کیف", Brand = "چرم مشهد", Barcode = "6281100678901", SalePrice = 1800000, IsActive = true, ItemType = "کالا" },
                new() { Id = 7, ProductCode = "PRD-007", ProductName = "کیف دستی زنانه مانگو", Category = "کیف", Brand = "مانگو", Barcode = "6281100789012", SalePrice = 2100000, IsActive = false, ItemType = "کالا" },
                new() { Id = 8, ProductCode = "PRD-008", ProductName = "کیف پول چرم مردانه", Category = "کیف", Brand = "تارا", Barcode = "6281100890123", SalePrice = 620000, IsActive = true, ItemType = "کالا" },
                new() { Id = 9, ProductCode = "PRD-009", ProductName = "کیف کمری ورزشی", Category = "کیف", Brand = "نایک", Barcode = "6281100901234", SalePrice = 450000, IsActive = true, ItemType = "کالا" },

                new() { Id = 10, ProductCode = "PRD-010", ProductName = "کمربند مردانه چرم", Category = "کمربند", Brand = "کالج", Barcode = "6281101012345", SalePrice = 450000, IsActive = true, ItemType = "کالا" },
                new() { Id = 11, ProductCode = "PRD-011", ProductName = "جوراب مردانه ۳ بسته", Category = "پوشاک", Brand = "جوراب مشهد", Barcode = "6281101123456", SalePrice = 85000, IsActive = true, ItemType = "کالا" },
                new() { Id = 12, ProductCode = "PRD-012", ProductName = "دستکش چرم زمستانه", Category = "پوشاک", Brand = "چرم کhoria", Barcode = "6281101234567", SalePrice = 380000, IsActive = true, ItemType = "کالا" },
                new() { Id = 13, ProductCode = "PRD-013", ProductName = "کلاه لبه‌دار مردانه", Category = "کلاه", Brand = "نیو ارا", Barcode = "6281101345678", SalePrice = 280000, IsActive = true, ItemType = "کالا" },

                new() { Id = 14, ProductCode = "PRD-014", ProductName = "عینک آفتابی ری بن", Category = "عینک", Brand = "ری بن", Barcode = "6281101456789", SalePrice = 3200000, IsActive = false, ItemType = "کالا" },
                new() { Id = 15, ProductCode = "PRD-015", ProductName = "ساعت مچی مردانه سیتیزن", Category = "ساعت", Brand = "سیتیزن", Barcode = "6281101567890", SalePrice = 5500000, IsActive = true, ItemType = "کالا" },
                new() { Id = 16, ProductCode = "PRD-016", ProductName = "زنجیر گردن استیل", Category = "زیورآلات", Brand = "silver", Barcode = "6281101678901", SalePrice = 720000, IsActive = true, ItemType = "کالا" },

                new() { Id = 17, ProductCode = "SVC-001", ProductName = "خدمات خیاطی و تعمیر", Category = "خدمات", Brand = "-", Barcode = "-", SalePrice = 150000, IsActive = true, ItemType = "خدمات" },
                new() { Id = 18, ProductCode = "SVC-002", ProductName = "خدمات تعمیر کفش", Category = "خدمات", Brand = "-", Barcode = "-", SalePrice = 80000, IsActive = true, ItemType = "خدمات" },
                new() { Id = 19, ProductCode = "SVC-003", ProductName = "خدمات واکس کفش", Category = "خدمات", Brand = "-", Barcode = "-", SalePrice = 35000, IsActive = true, ItemType = "خدمات" },
                new() { Id = 20, ProductCode = "SVC-004", ProductName = "خدمات بسته‌بندی هدیه", Category = "خدمات", Brand = "-", Barcode = "-", SalePrice = 25000, IsActive = false, ItemType = "خدمات" },
            };

            for (int i = 0; i < _allProductsList.Count; i++)
                _allProductsList[i].RowNumber = i + 1;
        }

        private static string ToPersianDigits(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            return string.Concat(number.ToString().Select(c => persianDigits[c - '0']));
        }
    }

    public partial class ProductListItem : ObservableObject, IListRowItem
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
                OnPropertyChanged(nameof(RowNumber));
                OnPropertyChanged(nameof(RowNumberDisplay));
            }
        }

        [ObservableProperty] private string _productCode = string.Empty;
        [ObservableProperty] private string _productName = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _brand = string.Empty;
        [ObservableProperty] private string _barcode = string.Empty;
        [ObservableProperty] private decimal _salePrice;
        [ObservableProperty] private bool _isActive;
        [ObservableProperty] private string _itemType = "کالا";

        public string RowNumberDisplay =>
            RowNumber > 0 ? ToPersianDigits(RowNumber) : string.Empty;

        public string SalePriceDisplay =>
            SalePrice == 0 ? string.Empty : $"{SalePrice:N0} ریال";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsEmpty => false;

        public string Status =>
            IsActive ? "فعال" : "غیرفعال";

        public System.Windows.Visibility StatusVisibility =>
            RowNumber > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        private static string ToPersianDigits(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            return string.Concat(number.ToString().Select(c => persianDigits[c - '0']));
        }
    }

    public class PageItem
    {
        public int PageNumber { get; set; }
        public string PageNumberDisplay { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
    }
}
