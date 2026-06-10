using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application.Contract.Persons;
using System.Threading.Tasks;
namespace Taadol.Views
{
    public partial class PersonListView : UserControl
    {
        public ObservableCollection<PersonItem> AllPersons { get; set; }
        public ObservableCollection<PersonItem> FilteredPersons { get; set; }

        private readonly IPersonApplication _personApplication;

        private int _pageSize = 15;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private string _currentFilter = "all";

        public PersonListView()
        {
            InitializeComponent();
            FillEmptyRows();
            SearchBox.TextChanged += (s, e) =>
            {
                _currentPage = 1;
                ApplyFilters();
            };

            Loaded += PersonListView_Loaded;
        }
        private async Task LoadDataAsync()
        {
            ShowLoading(true);

            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var personApplication = scope.ServiceProvider.GetRequiredService<IPersonApplication>();

                    var persons = personApplication.GetPersons();

                    return persons.Select((p, index) => new PersonItem
                    {
                        Id = p.Id,
                        RowNumber = index + 1,
                        Code = p.Code ?? p.Id.ToString(),
                        Category = p.PersonType ?? "—",
                        Status = p.IsActive ? "فعال" : "غیرفعال",
                        Nickname = "—",
                        FullNameText = p.FullName,
                        Company = p.BranchName ?? "—",
                        Province = "—",
                        City = "—",
                        Phone = "—",
                        Mobile = "—",
                        NationalId = p.NationalCode ?? "—",
                        EconomicId = p.EconomicCode ?? "—",
                        AccountStatus = "—",
                        PersonType = p.PersonType,
                        IsEmpty = false
                    }).ToList();
                });

                AllPersons = new ObservableCollection<PersonItem>(items);

                _currentPage = 1;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود اشخاص", MessageBoxButton.OK, MessageBoxImage.Error);
                AllPersons = new ObservableCollection<PersonItem>();
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            PersonsDataGrid.IsHitTestVisible = !show;
        }
        private async void PersonListView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }
        private void LoadData()
        {
            try
            {
                var persons = _personApplication.GetPersons();

                AllPersons = new ObservableCollection<PersonItem>(
                    persons.Select((p, index) => new PersonItem
                    {
                        Id = p.Id,
                        RowNumber = index + 1,
                        Code = p.Code,
                        Category = p.PersonType,
                        Status = p.IsActive ? "فعال" : "غیرفعال",
                        Nickname = "",
                        FullNameText = p.FullName,
                        Company = p.BranchName,
                        Province = "",
                        City = "",
                        Phone = "",
                        Mobile = "",
                        NationalId = p.NationalCode,
                        EconomicId = p.EconomicCode,
                        AccountStatus = "",
                        PersonType = p.PersonType,
                        IsEmpty = false
                    })
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود اشخاص", MessageBoxButton.OK, MessageBoxImage.Error);
                AllPersons = new ObservableCollection<PersonItem>();
            }
        }
        private void FillEmptyRows()
        {
            FilteredPersons = new ObservableCollection<PersonItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredPersons.Add(new PersonItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            PersonsDataGrid.ItemsSource = FilteredPersons;
        }
        private void ApplyFilters()
        {
            if (AllPersons == null) return;

            var query = AllPersons.AsEnumerable();

            switch (_currentFilter)
            {
                case "customer":
                    query = query.Where(p => p.PersonType != null && p.PersonType.Contains("مشتری"));
                    break;

                case "supplier":
                    query = query.Where(p => p.PersonType != null && p.PersonType.Contains("تامین"));
                    break;

                case "personnel":
                    query = query.Where(p => p.PersonType != null && p.PersonType.Contains("پرسنل"));
                    break;
            }

            var searchText = SearchBox?.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    (!string.IsNullOrWhiteSpace(p.Code) && p.Code.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.FullName) && p.FullName.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.NationalId) && p.NationalId.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.EconomicId) && p.EconomicId.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.Company) && p.Company.Contains(searchText))
                );
            }

            var filteredList = query.ToList();

            _totalPages = (int)Math.Ceiling(filteredList.Count / (double)_pageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = filteredList
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = ((_currentPage - 1) * _pageSize) + i + 1;

            FilteredPersons = new ObservableCollection<PersonItem>(pageItems);

            int realCount = FilteredPersons.Count;

            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredPersons.Add(new PersonItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            PersonsDataGrid.ItemsSource = FilteredPersons;

            BuildPaginationButtons();
        }

        private void BuildPaginationButtons()
        {
            if (PageButtonsItemsControl == null) return;

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

        private void FilterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                _currentFilter = "all";

                if (rb == tabCustomers) _currentFilter = "customer";
                else if (rb == tabSuppliers) _currentFilter = "supplier";
                else if (rb == tabPersonnel) _currentFilter = "personnel";

                _currentPage = 1;

                if (AllPersons != null)
                    ApplyFilters();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is not PersonItem item || item.IsEmpty)
            {
                MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"آیا از حذف «{item.FullName}» مطمئن هستید؟",
                "حذف شخص",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            var operation = _personApplication.Remove(item.Id);

            LoadData();
            ApplyFilters();

            MessageBox.Show("عملیات حذف انجام شد.", "حذف شخص", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is PersonItem item && !item.IsEmpty)
                MessageBox.Show($"ویرایش: {item.FullName}", "ویرایش شخص", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فرم شخص جدید", "شخص جدید", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر", "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void PersonsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";

            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];

            return result;
        }
    }

    public class PersonItem : INotifyPropertyChanged
    {
        private int _rowNumber;

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

        public string Code { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Nickname { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullNameText { get; set; }

        public string Company { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string NationalId { get; set; }
        public string EconomicId { get; set; }
        public string AccountStatus { get; set; }
        public string PersonType { get; set; }
        public bool IsEmpty { get; set; }

        public string FullName =>
            IsEmpty ? "" : !string.IsNullOrWhiteSpace(FullNameText)
                ? FullNameText
                : $"{FirstName} {LastName}".Trim();

        public string RowNumberDisplay =>
            RowNumber > 0 && !IsEmpty ? ToPersianNumber(RowNumber) : "";

        public Visibility StatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(Status) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility AccountStatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(AccountStatus) ? Visibility.Collapsed : Visibility.Visible;

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

    public class PageItem
    {
        public int PageNumber { get; set; }
        public string PageNumberDisplay { get; set; }
        public bool IsCurrent { get; set; }
    }
}