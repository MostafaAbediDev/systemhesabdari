using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Taadol.Views
{
    public partial class BranchListView : UserControl
    {
        public ObservableCollection<BranchItem> AllBranches { get; set; }
        public ObservableCollection<BranchItem> FilteredBranches { get; set; }
        private int _pageSize = 20;
        private string _currentFilter = "all";
        private string _searchText = "";

        public BranchListView()
        {
            InitializeComponent();
            LoadTestData();
            ApplyFilters();
        }

                private void CheckBoxBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is BranchItem item && !item.IsEmpty)
            {
                item.IsSelected = !item.IsSelected;
                e.Handled = true;
            }
        }

        private void LoadTestData()
        {
            AllBranches = new ObservableCollection<BranchItem>
            {
                new BranchItem { RowNumber = 1, RegisterDate = "۱۴۰۳/۰۶/۱۰", UniqueId = "BR-1001-001", BranchType = "اصلی", CompanyName = "تعادل سامانه پارس", BranchName = "دفتر مرکزی", RegistrationNumber = "۴۸۵۲۱", BranchCode = "۱۰۰۱", Province = "تهران", City = "تهران", Phone = "۰۲۱۸۸۶۵۴۳۲۱", Mobile = "۰۹۱۲۱۲۳۴۵۶۷", Address = "تهران، خیابان ولیعصر، بالاتر از میدان ولیعصر، پلاک ۱۲۳", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 2, RegisterDate = "۱۴۰۳/۰۷/۱۵", UniqueId = "BR-1002-002", BranchType = "فرعی", CompanyName = "تعادل سامانه پارس", BranchName = "شعبه شمال", RegistrationNumber = "۴۸۵۲۲", BranchCode = "۱۰۰۲", Province = "مازندران", City = "ساری", Phone = "۰۱۱۳۳۲۲۱۱۴۴", Mobile = "۰۹۱۱۱۲۲۳۳۴۴", Address = "ساری، خیابان انقلاب، نبش کوچه ۱۲، طبقه ۲", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 3, RegisterDate = "۱۴۰۳/۰۸/۰۱", UniqueId = "BR-1003-003", BranchType = "فرعی", CompanyName = "تعادل سامانه پارس", BranchName = "شعبه جنوب", RegistrationNumber = "۴۸۵۲۳", BranchCode = "۱۰۰۳", Province = "فارس", City = "شیراز", Phone = "۰۷۱۳۲۲۳۳۴۴۵", Mobile = "۰۹۱۳۳۴۴۵۵۶۶", Address = "شیراز، خیابان زند، روبروی پارک شاعران، پلاک ۴۵", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 4, RegisterDate = "۱۴۰۳/۰۸/۲۰", UniqueId = "BR-1004-004", BranchType = "اصلی", CompanyName = "تعادل سامانه پارس", BranchName = "انبار مرکزی", RegistrationNumber = "۴۸۵۲۴", BranchCode = "۱۰۰۴", Province = "اصفهان", City = "اصفهان", Phone = "۰۳۱۳۶۶۵۵۴۴۳", Mobile = "۰۹۱۴۴۵۵۶۶۷۷", Address = "اصفهان، شهرک صنعتی جی، بلوار اصلی، پلاک ۷۸", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 5, RegisterDate = "۱۴۰۳/۰۹/۰۵", UniqueId = "BR-1005-005", BranchType = "فرعی", CompanyName = "تعادل سامانه پارس", BranchName = "شعبه غرب", RegistrationNumber = "۴۸۵۲۵", BranchCode = "۱۰۰۵", Province = "کرمانشاه", City = "کرمانشاه", Phone = "۰۸۳۳۸۳۲۲۱۱", Mobile = "۰۹۱۵۵۶۶۷۷۸۸", Address = "کرمانشاه، بلوار طاقبستان، پلاک ۳۲", Status = "غیرفعال", IsEmpty = false },

                new BranchItem { RowNumber = 6, RegisterDate = "۱۴۰۳/۰۹/۱۸", UniqueId = "BR-1006-006", BranchType = "فرعی", CompanyName = "تعادل سامانه پارس", BranchName = "شعبه شرق", RegistrationNumber = "۴۸۵۲۶", BranchCode = "۱۰۰۶", Province = "خراسان رضوی", City = "مشهد", Phone = "۰۵۱۳۸۴۵۶۷۸۹", Mobile = "۰۹۱۶۶۷۷۸۸۹۹", Address = "مشهد، خیابان احمدآباد، بین احمدآباد ۲۰ و ۲۲، پلاک ۱۵", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 7, RegisterDate = "۱۴۰۳/۱۰/۰۲", UniqueId = "BR-1007-007", BranchType = "اصلی", CompanyName = "تعادل سامانه پارس", BranchName = "واحد تحقیق و توسعه", RegistrationNumber = "۴۸۵۲۷", BranchCode = "۱۰۰۷", Province = "تهران", City = "تهران", Phone = "۰۲۱۸۸۵۵۴۴۳۳", Mobile = "۰۹۱۸۸۹۹۰۰۱۱", Address = "تهران، پارک فناوری پردیس، خیابان دانش، پلاک ۷", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 8, RegisterDate = "۱۴۰۳/۱۱/۰۱", UniqueId = "BR-1008-008", BranchType = "فرعی", CompanyName = "تعادل سامانه پارس", BranchName = "شعبه جنوب غرب", RegistrationNumber = "۴۸۵۲۸", BranchCode = "۱۰۰۸", Province = "خوزستان", City = "اهواز", Phone = "۰۶۱۳۲۲۵۵۴۴۳", Mobile = "۰۹۱۹۹۰۰۱۱۲۲", Address = "اهواز، کیانپارس، خیابان ۳۵، پلاک ۱۲", Status = "فعال", IsEmpty = false },

                new BranchItem { RowNumber = 9, RegisterDate = "۱۴۰۳/۱۱/۲۰", UniqueId = "BR-1009-009", BranchType = "فرعی", CompanyName = "تعادل سامانه پارس", BranchName = "شعبه مرکز", RegistrationNumber = "۴۸۵۲۹", BranchCode = "۱۰۰۹", Province = "قم", City = "قم", Phone = "۰۲۵۳۶۶۵۵۴۴۳", Mobile = "۰۹۱۰۱۲۳۴۵۶۷", Address = "قم، بلوار امین، کوچه ۸، پلاک ۲۳", Status = "غیرفعال", IsEmpty = false },

                new BranchItem { RowNumber = 10, RegisterDate = "۱۴۰۳/۱۲/۰۵", UniqueId = "BR-1010-010", BranchType = "اصلی", CompanyName = "تعادل سامانه پارس", BranchName = "واحد پشتیبانی", RegistrationNumber = "۴۸۵۳۰", BranchCode = "۱۰۱۰", Province = "البرز", City = "کرج", Phone = "۰۲۶۳۴۴۵۵۶۶۷", Mobile = "۰۹۲۲۳۳۴۴۵۵", Address = "کرج، مهرشهر، بلوار ارم، پلاک ۵۶", Status = "فعال", IsEmpty = false }
            };
        }

        private void ApplyFilters()
        {
            if (AllBranches == null) return;

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
                    b.CompanyName.Contains(_searchText) ||
                    b.BranchName.Contains(_searchText) ||
                    b.BranchCode.Contains(_searchText) ||
                    b.City.Contains(_searchText));
            }

            var filteredList = query.ToList();
            for (int i = 0; i < filteredList.Count; i++)
                filteredList[i].RowNumber = i + 1;

            FilteredBranches = new ObservableCollection<BranchItem>(filteredList);

            int realCount = FilteredBranches.Count;
            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredBranches.Add(new BranchItem { RowNumber = i, IsEmpty = true });
            }

            BranchesDataGrid.ItemsSource = FilteredBranches;
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

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر - شعبه‌ها", "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void BtnNextPage_Click(object sender, RoutedEventArgs e) => MessageBox.Show("صفحه بعدی");
        private void BtnPrevPage_Click(object sender, RoutedEventArgs e) => MessageBox.Show("صفحه قبلی");
        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
                MessageBox.Show($"صفحه {btn.Tag}");
        }

        private void BranchesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

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
            set { _rowNumber = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumber))); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumberDisplay))); }
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
            set { _isSelected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected))); }
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

        public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
    }
}