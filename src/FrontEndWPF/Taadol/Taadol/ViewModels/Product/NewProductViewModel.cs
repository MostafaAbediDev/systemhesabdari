using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Taadol.Views;

namespace Taadol.ViewModels
{
    public class NewProductViewModel : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _sku = string.Empty;
        private long? _categoryId;
        private long? _brandId;
        private bool _isActive = true;
        private string _productImage = string.Empty;
        private long? _materialId;
        private string _gender = string.Empty;
        private long? _unitId;
        private string _season = string.Empty;
        private string _description = string.Empty;
        private string _saveButtonText = "ذخیره";
        private bool _isFormInteractive = true;
        private bool _isSaving;
        private long _variantIdCounter;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }
        public string Sku { get => _sku; set { _sku = value; OnPropertyChanged(); } }
        public long? CategoryId { get => _categoryId; set { _categoryId = value; OnPropertyChanged(); } }
        public long? BrandId { get => _brandId; set { _brandId = value; OnPropertyChanged(); } }
        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }
        public string ProductImage { get => _productImage; set { _productImage = value; OnPropertyChanged(); } }
        public long? MaterialId { get => _materialId; set { _materialId = value; OnPropertyChanged(); } }
        public string Gender { get => _gender; set { _gender = value; OnPropertyChanged(); } }
        public long? UnitId { get => _unitId; set { _unitId = value; OnPropertyChanged(); } }
        public string Season { get => _season; set { _season = value; OnPropertyChanged(); } }
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        public string SaveButtonText { get => _saveButtonText; set { _saveButtonText = value; OnPropertyChanged(); } }
        public bool IsFormInteractive { get => _isFormInteractive; set { _isFormInteractive = value; OnPropertyChanged(); } }
        public bool IsSaving { get => _isSaving; set { _isSaving = value; OnPropertyChanged(); } }

        public ObservableCollection<CategoryItem> Categories { get; } = new();
        public ObservableCollection<BrandItem> Brands { get; } = new();
        public ObservableCollection<MaterialItem> Materials { get; } = new();
        public ObservableCollection<string> GenderOptions { get; } = new() { "مردانه", "زنانه", "یونیسکس", "بچه‌گانه" };
        public ObservableCollection<UnitItem> Units { get; } = new();
        public ObservableCollection<string> SeasonOptions { get; } = new() { "بهار", "تابستان", "پاییز", "زمستان", "تمام فصول" };
        public ObservableCollection<ColorItem> Colors { get; } = new();
        public ObservableCollection<SizeItem> Sizes { get; } = new();
        public ObservableCollection<ProductVariantItem> Variants { get; } = new();
        public ObservableCollection<PriceRowItem> PriceRows { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddVariantCommand { get; }
        public ICommand RemoveVariantCommand { get; }
        public ICommand GenerateAllVariantCodesCommand { get; }
        public ICommand AddPriceRowCommand { get; }
        public ICommand RemovePriceRowCommand { get; }

        public NewProductViewModel()
        {
            SaveCommand = new RelayCommand(() => Save());
            EditCommand = new RelayCommand(() => Save());
            CancelCommand = new RelayCommand(() => Cancel());
            AddVariantCommand = new RelayCommand(() => AddVariant());
            RemoveVariantCommand = new RelayCommand<ProductVariantItem>(variant => RemoveVariant(variant));
            GenerateAllVariantCodesCommand = new RelayCommand(() => GenerateAllVariantCodes());
            AddPriceRowCommand = new RelayCommand(() => AddPriceRow());
            RemovePriceRowCommand = new RelayCommand<PriceRowItem>(row => RemovePriceRow(row));

            LoadMockData();
        }

        private void Save() { }
        private void Cancel() { }

        private void LoadMockData()
        {
            Colors.Add(new ColorItem { Id = 1, Name = "قرمز" });
            Colors.Add(new ColorItem { Id = 2, Name = "آبی" });
            Colors.Add(new ColorItem { Id = 3, Name = "سبز" });
            Colors.Add(new ColorItem { Id = 4, Name = "مشکی" });
            Colors.Add(new ColorItem { Id = 5, Name = "سفید" });

            Sizes.Add(new SizeItem { Id = 1, Name = "S" });
            Sizes.Add(new SizeItem { Id = 2, Name = "M" });
            Sizes.Add(new SizeItem { Id = 3, Name = "L" });
            Sizes.Add(new SizeItem { Id = 4, Name = "XL" });

            Materials.Add(new MaterialItem { Id = 1, Name = "پارچه" });
            Materials.Add(new MaterialItem { Id = 2, Name = "چرم" });
            Materials.Add(new MaterialItem { Id = 3, Name = "پلاستیک" });

            Units.Add(new UnitItem { Id = 1, DisplayName = "عدد" });
            Units.Add(new UnitItem { Id = 2, DisplayName = "کیلوگرم" });
            Units.Add(new UnitItem { Id = 3, DisplayName = "بسته" });

            Categories.Add(new CategoryItem { Id = 1, DisplayName = "پوشاک" });
            Categories.Add(new CategoryItem { Id = 2, DisplayName = "لوازم خانگی" });

            Brands.Add(new BrandItem { Id = 1, Name = "برند A" });
            Brands.Add(new BrandItem { Id = 2, Name = "برند B" });
        }

        private void AddVariant()
        {
            _variantIdCounter++;
            var variant = new ProductVariantItem
            {
                Id = _variantIdCounter
            };
            variant.SyncNames(Colors, Sizes);
            Variants.Add(variant);
        }

        private void RemoveVariant(ProductVariantItem? variant)
        {
            if (variant != null) Variants.Remove(variant);
        }

        private void GenerateAllVariantCodes()
        {
            int code = 1;
            foreach (var v in Variants)
            {
                v.Sku = $"VAR-{code:D3}";
                code++;
            }
        }

        private void AddPriceRow()
        {
            PriceRows.Add(new PriceRowItem());
        }

        private void RemovePriceRow(PriceRowItem? row)
        {
            if (row != null) PriceRows.Remove(row);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class CategoryItem { public long Id { get; set; } public string DisplayName { get; set; } = string.Empty; }
    public class BrandItem { public long Id { get; set; } public string Name { get; set; } = string.Empty; }
    public class MaterialItem { public long Id { get; set; } public string Name { get; set; } = string.Empty; }
    public class UnitItem { public long Id { get; set; } public string DisplayName { get; set; } = string.Empty; }
    public class ColorItem { public long Id { get; set; } public string Name { get; set; } = string.Empty; }
    public class SizeItem { public long Id { get; set; } public string Name { get; set; } = string.Empty; }

    public class ProductVariantItem : INotifyPropertyChanged
    {
        private long? _id;
        private long? _colorId;
        private long? _sizeId;
        private string _sku = string.Empty;
        private string _barcode = string.Empty;
        private string _colorName = string.Empty;
        private string _sizeName = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public long? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public long? ColorId { get => _colorId; set { _colorId = value; OnPropertyChanged(); } }
        public long? SizeId { get => _sizeId; set { _sizeId = value; OnPropertyChanged(); } }
        public string Sku { get => _sku; set { _sku = value; OnPropertyChanged(); } }
        public string Barcode { get => _barcode; set { _barcode = value; OnPropertyChanged(); } }
        public string ColorName { get => _colorName; set { _colorName = value; OnPropertyChanged(); } }
        public string SizeName { get => _sizeName; set { _sizeName = value; OnPropertyChanged(); } }

        public string DisplayText => string.IsNullOrEmpty(ColorName) && string.IsNullOrEmpty(SizeName)
            ? "نامشخص"
            : $"{ColorName} - {SizeName}".Trim(' ', '-');

        public void SyncNames(ObservableCollection<ColorItem> colors, ObservableCollection<SizeItem> sizes)
        {
            if (ColorId.HasValue)
                ColorName = colors.FirstOrDefault(c => c.Id == ColorId.Value)?.Name ?? string.Empty;
            if (SizeId.HasValue)
                SizeName = sizes.FirstOrDefault(s => s.Id == SizeId.Value)?.Name ?? string.Empty;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class PriceRowItem : INotifyPropertyChanged
    {
        private long? _variantId;
        private string _salePrice = string.Empty;
        private string _purchasePrice = string.Empty;
        private string _fromDate = string.Empty;
        private string _toDate = string.Empty;
        private ProductVariantItem? _selectedVariant;

        public event PropertyChangedEventHandler? PropertyChanged;

        public long? VariantId
        {
            get => _variantId;
            set
            {
                _variantId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VariantDisplay));
            }
        }
        public string SalePrice { get => _salePrice; set { _salePrice = value; OnPropertyChanged(); } }

        public string PurchasePrice { get => _purchasePrice; set { _purchasePrice = value; OnPropertyChanged(); } }

        public string FromDate { get => _fromDate; set { _fromDate = value; OnPropertyChanged(); } }
        public string ToDate { get => _toDate; set { _toDate = value; OnPropertyChanged(); } }

        public ProductVariantItem? SelectedVariant
        {
            get => _selectedVariant;
            set
            {
                _selectedVariant = value;
                OnPropertyChanged();
                if (value != null)
                {
                    VariantId = value.Id;
                }
                OnPropertyChanged(nameof(VariantDisplay));
            }
        }

        public string VariantDisplay => SelectedVariant?.DisplayText ?? string.Empty;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
