using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Taadol_Cal.Views
{
    public partial class PersonListView : UserControl
    {
        public ObservableCollection<PersonItem> AllPersons { get; set; }
        public ObservableCollection<PersonItem> FilteredPersons { get; set; }
        private int _pageSize = 20;

        public PersonListView()
        {
            InitializeComponent();
            LoadTestData();
            ApplyFilter("all");
        }

        private void LoadTestData()
        {
            AllPersons = new ObservableCollection<PersonItem>
    {
        new PersonItem
        {
            RowNumber = 1,
            Code = "۱۰۰۰۰۱",
            Category = "کلا - لوازم یک",
            Status = "فعال",
            Nickname = "لوازم یک",
            FirstName = "علی",
            LastName = "علوی",
            Company = "نوولنگ",
            Province = "فارس",
            City = "جهرم",
            Phone = "۰۷۱۵۴۲۲۳۳۲۳",
            Mobile = "۰۹۱۷۲۸۸۹۱۹۰",
            NationalId = "۲۲۹۵۶۴",
            EconomicId = "۱۲۳۴۴۴۴",
            AccountStatus = "بدهکار",
            PersonType = "Customer",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 2,
            Code = "۱۰۰۰۰۲",
            Category = "تامین کننده",
            Status = "فعال",
            Nickname = "پارت یدک",
            FirstName = "محمد",
            LastName = "احمدی",
            Company = "پارت یدک ایران",
            Province = "تهران",
            City = "تهران",
            Phone = "۰۲۱۸۸۶۶۳۳۲۲",
            Mobile = "۰۹۱۲۳۴۵۶۷۸۹",
            NationalId = "۱۲۳۴۵۶۷",
            EconomicId = "۹۸۷۶۵۴۳",
            AccountStatus = "بستانکار",
            PersonType = "Supplier",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 3,
            Code = "۱۰۰۰۰۳",
            Category = "پرسنل",
            Status = "فعال",
            Nickname = "رضایی",
            FirstName = "حسین",
            LastName = "رضایی",
            Company = "-",
            Province = "اصفهان",
            City = "اصفهان",
            Phone = "۰۳۱۳۶۶۱۲۲۳۳",
            Mobile = "۰۹۱۳۱۱۱۲۲۳۳",
            NationalId = "۴۵۶۷۸۹۰",
            EconomicId = "-",
            AccountStatus = "تسویه",
            PersonType = "Personnel",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 4,
            Code = "۱۰۰۰۰۴",
            Category = "مشتری ویژه",
            Status = "غیرفعال",
            Nickname = "الکترو صنعت",
            FirstName = "سارا",
            LastName = "کریمی",
            Company = "الکترو صنعت جنوب",
            Province = "خوزستان",
            City = "اهواز",
            Phone = "۰۶۱۳۲۲۱۱۴۴۵",
            Mobile = "۰۹۱۶۵۵۵۶۶۷۷",
            NationalId = "۷۸۹۰۱۲۳",
            EconomicId = "۵۵۶۶۷۷۸",
            AccountStatus = "بدهکار",
            PersonType = "Customer",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 5,
            Code = "۱۰۰۰۰۵",
            Category = "تامین کننده",
            Status = "فعال",
            Nickname = "صادقی",
            FirstName = "رضا",
            LastName = "صادقی",
            Company = "پخش مرکزی مشهد",
            Province = "خراسان رضوی",
            City = "مشهد",
            Phone = "۰۵۱۳۸۴۴۵۵۶۶",
            Mobile = "۰۹۱۵۱۰۰۲۰۳۰",
            NationalId = "۰۹۴۱۲۳۴۵۶",
            EconomicId = "۱۱۱۲۲۲۳۳۳",
            AccountStatus = "بستانکار",
            PersonType = "Supplier",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 6,
            Code = "۱۰۰۰۰۶",
            Category = "مشتری",
            Status = "فعال",
            Nickname = "منتظری",
            FirstName = "مریم",
            LastName = "منتظری",
            Company = "-",
            Province = "آذربایجان شرقی",
            City = "تبریز",
            Phone = "۰۴۱۳۳۳۴۴۵۵۶",
            Mobile = "۰۹۱۴۴۴۴۵۵۶۶",
            NationalId = "۱۳۷۸۸۹۹۰۰",
            EconomicId = "-",
            AccountStatus = "بدهکار",
            PersonType = "Customer",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 7,
            Code = "۱۰۰۰۰۷",
            Category = "پرسنل",
            Status = "فعال",
            Nickname = "سهرابی",
            FirstName = "امیر",
            LastName = "سهرابی",
            Company = "-",
            Province = "یزد",
            City = "یزد",
            Phone = "۰۳۵۳۸۲۲۷۷۸۸",
            Mobile = "۰۹۱۳۸۸۸۹۹۰۰",
            NationalId = "۴۴۳۳۲۲۱۱۰",
            EconomicId = "-",
            AccountStatus = "تسویه",
            PersonType = "Personnel",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 8,
            Code = "۱۰۰۰۰۸",
            Category = "تامین کننده",
            Status = "فعال",
            Nickname = "فولاد گستر",
            FirstName = "بهمن",
            LastName = "نوری",
            Company = "صنایع فولاد گیلان",
            Province = "گیلان",
            City = "رشت",
            Phone = "۰۱۳۳۳۵۵۶۶۷۷",
            Mobile = "۰۹۱۱۱۲۲۳۳۴۴",
            NationalId = "۵۵۶۶۴۴۳۳۲",
            EconomicId = "۹۹۸۸۷۷۶۶",
            AccountStatus = "بستانکار",
            PersonType = "Supplier",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 9,
            Code = "۱۰۰۰۰۹",
            Category = "مشتری",
            Status = "غیرفعال",
            Nickname = "خانم ناصری",
            FirstName = "زهرا",
            LastName = "ناصری",
            Company = "-",
            Province = "کرمان",
            City = "کرمان",
            Phone = "۰۳۴۳۲۲۱۱۵۵",
            Mobile = "۰۹۱۳۷۷۷۸۸۹۹",
            NationalId = "۲۹۹۰۰۸۸۷۷",
            EconomicId = "-",
            AccountStatus = "تسویه",
            PersonType = "Customer",
            IsEmpty = false
        },
        new PersonItem
        {
            RowNumber = 10,
            Code = "۱۰۰۰۱۰",
            Category = "مشتری ویژه",
            Status = "فعال",
            Nickname = "مهندس مرادی",
            FirstName = "سعید",
            LastName = "مرادی",
            Company = "سازه گستر فردا",
            Province = "مازندران",
            City = "ساری",
            Phone = "۰۱۱۳۳۲۲۱۱۴۴",
            Mobile = "۰۹۱۱۲۲۳۳۴۴۵",
            NationalId = "۲۱۴۵۵۶۶۷۷",
            EconomicId = "۴۴۵۵۶۶۷۷",
            AccountStatus = "بدهکار",
            PersonType = "Customer",
            IsEmpty = false
        }
    };
        }

        private void ApplyFilter(string filter)
        {
            FilteredPersons = new ObservableCollection<PersonItem>();
            int row = 1;

            foreach (var p in AllPersons)
            {
                bool include = false;

                switch (filter)
                {
                    case "all":
                        include = true;
                        break;
                    case "customer":
                        include = p.PersonType == "Customer";
                        break;
                    case "supplier":
                        include = p.PersonType == "Supplier";
                        break;
                    case "personnel":
                        include = p.PersonType == "Personnel";
                        break;
                }

                if (include)
                {
                    p.RowNumber = row++;
                    FilteredPersons.Add(p);
                }
            }

            // Fill empty rows
            int realCount = FilteredPersons.Count;
            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredPersons.Add(new PersonItem
                {
                    RowNumber = i,
                    IsEmpty = true
                });
            }

            PersonsDataGrid.ItemsSource = FilteredPersons;
        }

        private void FilterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                string filter = "all";
                if (rb == tabCustomers) filter = "customer";
                else if (rb == tabSuppliers) filter = "supplier";
                else if (rb == tabPersonnel) filter = "personnel";

                // Only apply if already loaded
                if (AllPersons != null)
                    ApplyFilter(filter);
            }
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر", "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is PersonItem item && !item.IsEmpty)
            {
                var result = MessageBox.Show(
                    $"آیا از حذف «{item.FullName}» مطمئن هستید؟",
                    "حذف شخص",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    AllPersons.Remove(item);
                    FilteredPersons.Remove(item);

                    // Renumber
                    for (int i = 0; i < FilteredPersons.Count; i++)
                    {
                        FilteredPersons[i].RowNumber = i + 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is PersonItem item && !item.IsEmpty)
            {
                MessageBox.Show($"ویرایش: {item.FullName}", "ویرایش شخص",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فرم شخص جدید", "شخص جدید",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("صفحه بعدی");
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("صفحه قبلی");
        }

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                MessageBox.Show($"صفحه {btn.Tag}");
            }
        }

        private void PersonsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    // ========== DATA MODEL ==========
    public class PersonItem : System.ComponentModel.INotifyPropertyChanged
    {
        private int _rowNumber;

        public int RowNumber
        {
            get => _rowNumber;
            set
            {
                _rowNumber = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(RowNumber)));
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(RowNumberDisplay)));
            }
        }

        public string Code { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string NationalId { get; set; }
        public string EconomicId { get; set; }
        public string AccountStatus { get; set; }
        public string PersonType { get; set; } // Customer, Supplier, Personnel
        public bool IsEmpty { get; set; }

        // === Computed Properties ===

        public string FullName =>
            IsEmpty ? "" : $"{FirstName} {LastName}".Trim();

        public string RowNumberDisplay =>
            RowNumber > 0 ? ToPersianNumber(RowNumber) : "";

        public Visibility StatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(Status) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility AccountStatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(AccountStatus) ? Visibility.Collapsed : Visibility.Visible;

        // === Helper ===

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";
            foreach (char c in number.ToString())
            {
                result += persianDigits[int.Parse(c.ToString())];
            }
            return result;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}