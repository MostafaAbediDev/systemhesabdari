using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Taadol.Views
{
    public partial class ProductListView : UserControl
    {
        public ObservableCollection<ItemBase> AllProducts { get; set; }
        public ObservableCollection<ItemBase> AllServices { get; set; }
        public ObservableCollection<ItemBase> FilteredItems { get; set; }

        private string _currentType = "all"; private string _currentStatus = "all"; private int _pageSize = 20;

        public ProductListView()
        {
            InitializeComponent();

            LoadTestData();

            this.Dispatcher.BeginInvoke(new Action(() => ApplyFilter()));
        }

        private void LoadTestData()
        {
            AllProducts = new ObservableCollection<ItemBase>
            {
                new ItemBase
                {
                    RowNumber = 1,
                    Code = "۱۰۰۰۱",
                    Category = "لوازم یدکی",
                    Name = "پولی V بلت",
                    Barcode = "۵۴۴۶",
                    Unit = "عدد",
                    Quantity = 10,
                    Status = "فعال",
                    Description = "-",
                    PurchasePrice = 50000000,
                    SalePrice = 65000000,
                    Tax = 500000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 2,
                    Code = "۱۰۰۰۲",
                    Category = "لوازم الکتریکی",
                    Name = "کابل برق",
                    Barcode = "۶۵۵۷",
                    Unit = "متر",
                    Quantity = 12,
                    Status = "فعال",
                    Description = "-",
                    PurchasePrice = 20000000,
                    SalePrice = 62000000,
                    Tax = 2200000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 3,
                    Code = "۱۰۰۰۳",
                    Category = "قطعات صنعتی",
                    Name = "فلنج فولادی",
                    Barcode = "۷۶۶۸",
                    Unit = "عدد",
                    Quantity = 25,
                    Status = "فعال",
                    Description = "-",
                    PurchasePrice = 35000000,
                    SalePrice = 48000000,
                    Tax = 1500000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 4,
                    Code = "۱۰۰۰۴",
                    Category = "لوازم یدکی",
                    Name = "فیلتر روغن",
                    Barcode = "۸۷۷۹",
                    Unit = "عدد",
                    Quantity = 8,
                    Status = "غیرفعال",
                    Description = "-",
                    PurchasePrice = 15000000,
                    SalePrice = 22000000,
                    Tax = 800000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 5,
                    Code = "۱۰۰۰۵",
                    Category = "لوازم الکتریکی",
                    Name = "سوئیچ 220V",
                    Barcode = "۹۸۸۰",
                    Unit = "عدد",
                    Quantity = 45,
                    Status = "فعال",
                    Description = "-",
                    PurchasePrice = 12000000,
                    SalePrice = 18000000,
                    Tax = 600000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 6,
                    Code = "۱۰۰۰۶",
                    Category = "قطعات صنعتی",
                    Name = "بلبرینگ 6205",
                    Barcode = "۲۳۴۵",
                    Unit = "عدد",
                    Quantity = 18,
                    Status = "غیرفعال",
                    Description = "-",
                    PurchasePrice = 28000000,
                    SalePrice = 42000000,
                    Tax = 1200000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 7,
                    Code = "۱۰۰۰۷",
                    Category = "لوازم یدکی",
                    Name = "باتری 12V",
                    Barcode = "۳۴۵۶",
                    Unit = "عدد",
                    Quantity = 30,
                    Status = "فعال",
                    Description = "-",
                    PurchasePrice = 85000000,
                    SalePrice = 125000000,
                    Tax = 4000000,
                    ItemType = "product",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 8,
                    Code = "۱۰۰۰۸",
                    Category = "لوازم الکتریکی",
                    Name = "ترانس 1KVA",
                    Barcode = "۴۵۶۷",
                    Unit = "عدد",
                    Quantity = 5,
                    Status = "فعال",
                    Description = "-",
                    PurchasePrice = 220000000,
                    SalePrice = 320000000,
                    Tax = 12000000,
                    ItemType = "product",
                    IsEmpty = false
                }
            };

            AllServices = new ObservableCollection<ItemBase>
            {
                new ItemBase
                {
                    RowNumber = 1,
                    Code = "۲۰۰۰۱",
                    Category = "خدمات تعمیر",
                    Name = "تعمیر موتور الکتریکی",
                    Barcode = "۱۲۳۴",
                    Unit = "ساعت",
                    Quantity = null,
                    Status = "فعال",
                    Description = "تعمیر و سرویس موتورهای الکتریکی",
                    PurchasePrice = 0,
                    SalePrice = 150000000,
                    Tax = 5000000,
                    ItemType = "service",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 2,
                    Code = "۲۰۰۰۲",
                    Category = "خدمات نصب",
                    Name = "نصب و راه اندازی تجهیزات",
                    Barcode = "۲۳۴۵",
                    Unit = "پروژه",
                    Quantity = null,
                    Status = "فعال",
                    Description = "نصب و راه اندازی تجهیزات صنعتی",
                    PurchasePrice = 0,
                    SalePrice = 200000000,
                    Tax = 8000000,
                    ItemType = "service",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 3,
                    Code = "۲۰۰۰۳",
                    Category = "خدمات مشاوره",
                    Name = "مشاوره فنی",
                    Barcode = "۳۴۵۶",
                    Unit = "ساعت",
                    Quantity = null,
                    Status = "فعال",
                    Description = "ارائه مشاوره فنی برای حل مشکلات",
                    PurchasePrice = 0,
                    SalePrice = 100000000,
                    Tax = 3000000,
                    ItemType = "service",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 4,
                    Code = "۲۰۰۰۴",
                    Category = "خدمات تعمیر",
                    Name = "سرویس انجین",
                    Barcode = "۴۵۶۷",
                    Unit = "بار",
                    Quantity = null,
                    Status = "غیرفعال",
                    Description = "سرویس دوری انجین خودرو",
                    PurchasePrice = 0,
                    SalePrice = 50000000,
                    Tax = 1500000,
                    ItemType = "service",
                    IsEmpty = false
                },
                new ItemBase
                {
                    RowNumber = 5,
                    Code = "۲۰۰۰۵",
                    Category = "خدمات حمل و نقل",
                    Name = "حمل و نقل محصولات",
                    Barcode = "۵۶۷۸",
                    Unit = "تن",
                    Quantity = null,
                    Status = "فعال",
                    Description = "حمل و نقل محصولات در سراسر کشور",
                    PurchasePrice = 0,
                    SalePrice = 75000000,
                    Tax = 2500000,
                    ItemType = "service",
                    IsEmpty = false
                }
            };
        }

        private void ApplyFilter()
        {
            if (AllProducts == null || AllServices == null)
            {
                return;
            }

            FilteredItems = new ObservableCollection<ItemBase>();
            int row = 1;

            ObservableCollection<ItemBase> sourceCollection = null;

            switch (_currentType)
            {
                case "all":
                    sourceCollection = new ObservableCollection<ItemBase>();
                    foreach (var item in AllProducts)
                    {
                        sourceCollection.Add(item);
                    }
                    foreach (var item in AllServices)
                    {
                        sourceCollection.Add(item);
                    }
                    break;
                case "products":
                    sourceCollection = AllProducts;
                    break;
                case "services":
                    sourceCollection = AllServices;
                    break;
                default:
                    sourceCollection = AllProducts;
                    break;
            }

            if (sourceCollection != null)
            {
                foreach (var item in sourceCollection)
                {
                    bool include = false;

                    switch (_currentStatus)
                    {
                        case "all":
                            include = true;
                            break;
                        case "active":
                            include = item.Status == "فعال";
                            break;
                        case "inactive":
                            include = item.Status == "غیرفعال";
                            break;
                    }

                    if (include)
                    {
                        item.RowNumber = row++;
                        FilteredItems.Add(item);
                    }
                }
            }

            int realCount = FilteredItems.Count;
            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredItems.Add(new ItemBase
                {
                    RowNumber = i,
                    IsEmpty = true
                });
            }

            if (ProductsDataGrid != null)
            {
                ProductsDataGrid.ItemsSource = FilteredItems;
            }

            UpdateHeaderTitle();
        }

        private void UpdateHeaderTitle()
        {
            if (HeaderTitle != null)
            {
                string typeText = _currentType == "products" ? "کالاها" :
                                 _currentType == "services" ? "خدمات" : "کالاها و خدمات";
            }
        }

        private void TypeTab_Checked(object sender, RoutedEventArgs e)
        {
            if (AllProducts == null || AllServices == null)
                return;

            if (sender is RadioButton rb)
            {
                if (rb.Name == "tabAllTypes")
                {
                    _currentType = "all";
                }
                else if (rb.Name == "tabProducts")
                {
                    _currentType = "products";
                }
                else if (rb.Name == "tabServices")
                {
                    _currentType = "services";
                }

                ApplyFilter();
            }
        }

        private void FilterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (AllProducts == null || AllServices == null)
                return;

            if (sender is RadioButton rb)
            {
                if (rb.Name == "tabAll")
                    _currentStatus = "all";
                else if (rb.Name == "tabActive")
                    _currentStatus = "active";
                else if (rb.Name == "tabInactive")
                    _currentStatus = "inactive";

                ApplyFilter();
            }
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر", "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ItemBase item && !item.IsEmpty)
            {
                var result = MessageBox.Show(
                    $"آیا از حذف «{item.Name}» مطمئن هستید؟",
                    "حذف مورد",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    var sourceCollection = _currentType == "products" ? AllProducts : AllServices;
                    sourceCollection.Remove(item);
                    FilteredItems.Remove(item);

                    for (int i = 0; i < FilteredItems.Count; i++)
                    {
                        if (!FilteredItems[i].IsEmpty)
                            FilteredItems[i].RowNumber = i + 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک مورد انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ItemBase item && !item.IsEmpty)
            {
                MessageBox.Show($"ویرایش: {item.Name}", "ویرایش مورد",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("لطفاً یک مورد انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            string typeText = _currentType == "products" ? "کالا" : "خدمت";
            MessageBox.Show($"فرم {typeText} جدید", $"{typeText} جدید",
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

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }

    public class ItemBase : System.ComponentModel.INotifyPropertyChanged
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
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Unit { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Tax { get; set; }
        public string ItemType { get; set; }
        public bool IsEmpty { get; set; }


        public string RowNumberDisplay =>
            RowNumber > 0 ? ToPersianNumber(RowNumber) : "";

        public string PurchasePriceFormatted =>
            IsEmpty || PurchasePrice == 0 ? "" : $"{PurchasePrice:N0} ریال";

        public string SalePriceFormatted =>
            IsEmpty || SalePrice == 0 ? "" : $"{SalePrice:N0} ریال";

        public string TaxFormatted =>
            IsEmpty || Tax == 0 ? "" : $"{Tax:N0} ریال";

        public Visibility StatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(Status) ? Visibility.Collapsed : Visibility.Visible;


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
