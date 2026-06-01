// Models/Product.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaadolAccounting.Models
{
    public class Product : INotifyPropertyChanged
    {
        private string _accountCode;
        private bool _isAutoCode = true;
        private bool _isActive = true;
        private string _productName;
        private string _barcode1;
        private string _barcode2;
        private string _productCode;
        private string _category;
        private string _brand;
        private string _description;
        private string _imagePath;


        private decimal _purchasePrice;
        private decimal _salePrice;
        private decimal _wholesalePrice;
        private decimal _retailPrice;
        private decimal _workerPrice;

        private bool _isPriceListManaged;

        public string AccountCode
        {
            get => _accountCode;
            set { _accountCode = value; OnPropertyChanged(); }
        }

        public bool IsAutoCode
        {
            get => _isAutoCode;
            set { _isAutoCode = value; OnPropertyChanged(); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); }
        }

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public string Barcode1
        {
            get => _barcode1;
            set { _barcode1 = value; OnPropertyChanged(); }
        }

        public string Barcode2
        {
            get => _barcode2;
            set { _barcode2 = value; OnPropertyChanged(); }
        }

        public string ProductCode
        {
            get => _productCode;
            set { _productCode = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        public string Brand
        {
            get => _brand;
            set { _brand = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set { _purchasePrice = value; OnPropertyChanged(); }
        }

        public decimal SalePrice
        {
            get => _salePrice;
            set { _salePrice = value; OnPropertyChanged(); }
        }

        public decimal WholesalePrice
        {
            get => _wholesalePrice;
            set { _wholesalePrice = value; OnPropertyChanged(); }
        }

        public decimal RetailPrice
        {
            get => _retailPrice;
            set { _retailPrice = value; OnPropertyChanged(); }
        }

        public decimal WorkerPrice
        {
            get => _workerPrice;
            set { _workerPrice = value; OnPropertyChanged(); }
        }

        public bool IsPriceListManaged
        {
            get => _isPriceListManaged;
            set { _isPriceListManaged = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}