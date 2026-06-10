using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows.Media;
namespace Taadol.Views
{
    public partial class CompanyListView : UserControl
    {


        public ObservableCollection<CompanyItem> AllCompanies { get; set; }
        public ObservableCollection<CompanyItem> FilteredCompanies { get; set; }

        private readonly ICompanyApplication _companyApplication;
        private int _pageSize = 15;
        private string _currentFilter = "all";

        public CompanyListView()
        {
            InitializeComponent();

            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            LoadData();
            ApplyFilters();
        }
        private void RoundedGridClip_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Grid grid)
            {
                grid.Clip = new RectangleGeometry(
                    new Rect(0, 0, e.NewSize.Width, e.NewSize.Height),
                    12,
                    12
                );
            }
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
                var companies = _companyApplication.GetCompanies();

                AllCompanies = new ObservableCollection<CompanyItem>(
     companies.Select((c, index) => new CompanyItem
     {
         Id = c.Id,
         RowNumber = index + 1,
         RegisterDate = ToPersianDate(c.CreationDate),
         Title = c.Title,
         LegalName = c.LegalName,
         Status = "فعال",
         IsEmpty = false
     })
 );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شرکت‌ها", MessageBoxButton.OK, MessageBoxImage.Error);
                AllCompanies = new ObservableCollection<CompanyItem>();
            }
        }

        private void ApplyFilters()
        {
            if (AllCompanies == null) return;

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

            var filteredList = query.ToList();

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

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = ((_currentPage - 1) * _pageSize) + i + 1;

            FilteredCompanies = new ObservableCollection<CompanyItem>(pageItems);

            int realCount = FilteredCompanies.Count;

            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredCompanies.Add(new CompanyItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            CompaniesDataGrid.ItemsSource = FilteredCompanies;

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
        public class PageItem
        {
            public int PageNumber { get; set; }
            public string PageNumberDisplay { get; set; }
            public bool IsCurrent { get; set; }
        }
        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyFilters();
            }
        }

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";

            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];

            return result;
        }
        private int _currentPage = 1;
        private int _totalPages = 1;
        private void CheckBoxBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;

            if (border?.DataContext is CompanyItem item && !item.IsEmpty)
            {
                item.IsSelected = !item.IsSelected;
                e.Handled = true;
            }
        }

        private void FilterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                if (rb == tabAll) _currentFilter = "all";
                else if (rb == tabActive) _currentFilter = "active";
                else if (rb == tabInactive) _currentFilter = "inactive";

                ApplyFilters();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = AllCompanies
                .Where(c => c.IsSelected && !c.IsEmpty)
                .ToList();

            if (selectedItems.Count == 0)
            {
                if (CompaniesDataGrid.SelectedItem is CompanyItem item && !item.IsEmpty)
                    selectedItems.Add(item);
                else
                {
                    MessageBox.Show("لطفاً یک شرکت انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var result = MessageBox.Show(
                $"آیا از حذف {selectedItems.Count} شرکت مطمئن هستید؟",
                "حذف شرکت",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            foreach (var item in selectedItems)
            {
                _companyApplication.Remove(item.Id);
            }

            _currentPage = 1;
            LoadData();
            ApplyFilters();

            MessageBox.Show("عملیات حذف انجام شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CompaniesDataGrid.SelectedItem is CompanyItem item && !item.IsEmpty)
                MessageBox.Show($"ویرایش شرکت: {item.Title}", "ویرایش شرکت", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("لطفاً یک شرکت انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فرم ثبت شرکت جدید", "شرکت جدید", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر - شرکت‌ها", "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void CompaniesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }

    public class CompanyItem : INotifyPropertyChanged
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