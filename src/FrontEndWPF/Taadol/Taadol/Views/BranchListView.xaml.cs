using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Threading;
namespace Taadol.Views
{
    public partial class BranchListView : UserControl
    {

        public ObservableCollection<BranchItem> AllBranches { get; set; }
        public ObservableCollection<BranchItem> FilteredBranches { get; set; }

        private readonly IBranchApplication _branchApplication;
        private int _pageSize = 15;
        private string _currentFilter = "all";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _isLoadedOnce = false;

        public BranchListView()
        {
            InitializeComponent();

            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();

            AllBranches = new ObservableCollection<BranchItem>();

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
            ShowLoading(true);

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
                        CompanyName = b.CompanyId.ToString(),
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
                ShowLoading(false);
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

            BranchesDataGrid.ItemsSource = FilteredBranches;
        }

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            BranchesDataGrid.IsHitTestVisible = !show;
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
        private void LoadData()
        {
            try
            {
                var branches = _branchApplication.GetBranches();


                AllBranches = new ObservableCollection<BranchItem>(
                    branches.Select((b, index) => new BranchItem
                    {
                        RowNumber = index + 1,
                        RegisterDate = ToPersianDate(b.CreatedAt),
                        UniqueId = b.Id.ToString(),
                        BranchType = "—",
                        CompanyName = b.CompanyId.ToString(),
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
                    })
                )
                {

                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شعبه‌ها");
            }
        }

        private void ApplyFilters()
        {
            if (AllBranches == null)
                return;

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

            var filteredList = query.ToList();

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

            BranchesDataGrid.ItemsSource = FilteredBranches;

            BuildPaginationButtons();
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

                PageButtonsItemsControl.ItemsSource = pages;
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

            PageButtonsItemsControl.ItemsSource = pages;
        }
        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            string result = "";

            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];

            return result;
        }
        private void CheckBoxBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is BranchItem item && !item.IsEmpty)
            {
                item.IsSelected = !item.IsSelected;
                e.Handled = true;
            }
        }
        public class PageItem
        {
            public int PageNumber { get; set; }
            public string PageNumberDisplay { get; set; }
            public bool IsCurrent { get; set; }
        }
        private void FilterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                if (rb == tabAll) _currentFilter = "all";
                else if (rb == tabActive) _currentFilter = "active";
                else if (rb == tabInactive) _currentFilter = "inactive";

                _currentPage = 1;
                ApplyFilters();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = AllBranches.Where(b => b.IsSelected && !b.IsEmpty).ToList();

            if (selectedItems.Count == 0)
            {
                if (BranchesDataGrid.SelectedItem is BranchItem item && !item.IsEmpty)
                    selectedItems.Add(item);
                else
                {
                    MessageBox.Show("لطفاً یک شعبه انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var result = MessageBox.Show($"آیا از حذف {selectedItems.Count} شعبه مطمئن هستید؟", "حذف",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var item in selectedItems.ToList())
                    AllBranches.Remove(item);
                ApplyFilters();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (BranchesDataGrid.SelectedItem is BranchItem item && !item.IsEmpty)
                MessageBox.Show($"ویرایش شعبه: {item.BranchName}", "ویرایش شعبه", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("لطفاً یک شعبه انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فرم ثبت شعبه جدید", "شعبه جدید", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر - شعبه‌ها", "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyFilters();
            }
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFilters();
            }
        }

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int pageNumber = Convert.ToInt32(btn.Tag);

                if (pageNumber <= 0)
                    return;

                _currentPage = pageNumber;
                ApplyFilters();
            }
        }
        private void BranchesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void RowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.DataContext is BranchItem item)
                item.IsSelected = true;
        }

        private void RowCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.DataContext is BranchItem item)
                item.IsSelected = false;
        }
    }

    public class BranchItem : INotifyPropertyChanged
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

    public class InverseBooleanConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue) return !boolValue;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue) return !boolValue;
            return false;
        }
    }
}