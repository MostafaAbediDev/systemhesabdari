using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Taadol_Cal.Views
{
    public partial class ProductListView : UserControl
    {
        public ObservableCollection<ProductItem> Products { get; set; }
        private int _pageSize = 14; // تعداد ردیف قابل نمایش

        public ProductListView()
        {
            InitializeComponent();
            LoadTestData();
            ProductsDataGrid.ItemsSource = Products;
        }

        private void LoadTestData()
        {
            Products = new ObservableCollection<ProductItem>();

            // دیتای واقعی
            Products.Add(new ProductItem
            {
                RowNumber = 1,
                Code = "نمونه۱",
                Category = "لوازم یدکی",
                ProductName = "لوازم یدکی",
                Barcode = "۵۴۴۶",
                ProductCode = "۱۱۰۸۶۵۴۴۶۴",
                Unit = "عدد",
                Quantity = 10,
                Status = "فعال",
                Description = "شرح",
                PurchasePrice = 50000000,
                SalePrice = 65000000,
                Tax = 500,
                IsEmpty = false
            });

            Products.Add(new ProductItem
            {
                RowNumber = 2,
                Code = "نمونه۲",
                Category = "لوازم یدکی",
                ProductName = "لوازم یدکی",
                Barcode = "۵۴۴۶",
                ProductCode = "۱۱۰۸۶۵۴۴۶۴",
                Unit = "عدد",
                Quantity = 12,
                Status = "فعال",
                Description = "شرح",
                PurchasePrice = 20000000,
                SalePrice = 62000000,
                Tax = 2200,
                IsEmpty = false
            });

            // ردیف‌های خالی برای پر کردن جدول
            int realDataCount = Products.Count;
            for (int i = realDataCount + 1; i <= _pageSize; i++)
            {
                Products.Add(new ProductItem
                {
                    RowNumber = i,
                    IsEmpty = true
                });
            }
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("More clicked");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductItem item && !item.IsEmpty)
            {
                Products.Remove(item);
                // شماره‌گذاری مجدد
                for (int i = 0; i < Products.Count; i++)
                {
                    Products[i].RowNumber = i + 1;
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductItem item && !item.IsEmpty)
            {
                MessageBox.Show($"ویرایش: {item.ProductName}");
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("New clicked");
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Next page");
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Previous page");
        }

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                MessageBox.Show($"Page {btn.Tag} clicked");
            }
        }
    }

    public class ProductItem : System.ComponentModel.INotifyPropertyChanged
    {
        private int _rowNumber;

        public int RowNumber
        {
            get => _rowNumber;
            set
            {
                _rowNumber = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(RowNumber)));
            }
        }

        public string Code { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string ProductCode { get; set; }
        public string Unit { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? Tax { get; set; }
        public bool IsEmpty { get; set; }

        // Formatted properties - خالی برای ردیف‌های خالی
        public string PurchasePriceFormatted =>
            IsEmpty || PurchasePrice == null ? "" : $"{PurchasePrice:N0} ریال";

        public string SalePriceFormatted =>
            IsEmpty || SalePrice == null ? "" : $"{SalePrice:N0} ریال";

        public string TaxFormatted =>
            IsEmpty || Tax == null ? "" : $"{Tax:N0} ریال";

        // Visibility برای Badge وضعیت
        public Visibility StatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(Status) ? Visibility.Collapsed : Visibility.Visible;

        // نمایش شماره ردیف فقط
        public string RowNumberDisplay => RowNumber > 0 ? ToPersianNumber(RowNumber) : "";

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