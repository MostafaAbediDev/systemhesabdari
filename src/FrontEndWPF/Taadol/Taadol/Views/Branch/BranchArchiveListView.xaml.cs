using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Taadol.Controls;
using Taadol.ViewModels;

namespace Taadol.Views
{
    public partial class BranchArchiveListView : UserControl
    {
        public BranchArchiveListViewModel ViewModel { get; }
        private readonly DispatcherTimer _searchDebounceTimer;
        private CancellationTokenSource _loadCts = new();
        private bool _isLoadedOnce;

        public BranchArchiveListView()
        {
            InitializeComponent();

            // ظاهر گرید آرشیو را دقیقاً شبیه جدول حساب‌های بانکی (BankAccountsTableControl) کن
            ApplyBankTableStyle();

            ViewModel = new BranchArchiveListViewModel(App.ServiceProvider);
            DataContext = ViewModel;

            // جستجو با Debounce: هر ضربه کلید مستقیم فیلتر سنگین را روی UI Thread اجرا نکند
            _searchDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                ViewModel.HandleSearchTextChanged(HeaderSearchBox.Text);
            };

            HeaderSearchBox.TextChanged += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Start();
            };

            ArchivesGrid.NextPageRequested += (s, e) => ViewModel.GoToNextPage();
            ArchivesGrid.PreviousPageRequested += (s, e) => ViewModel.GoToPreviousPage();
            ArchivesGrid.PageRequested += (s, page) => ViewModel.GoToPage(page);
            ArchivesGrid.PageSizeRequested += (s, size) => ViewModel.ChangePageSize(size);

            ArchivesGrid.CheckedItemsChanged += (s, e) =>
                ViewModel.UpdateSelectedCount();

            // انتخاب همه = فقط همین صفحه؛ خاموش‌کردن هدر = پاک کردن انتخاب کل لیست
            ArchivesGrid.SelectAllToggled += (s, select) =>
            {
                if (select || ViewModel.AllArchives == null) return;
                foreach (var item in ViewModel.AllArchives.Where(p => !p.IsEmpty))
                    item.IsSelected = false;
            };

            // منوی راست‌کلیک ردیف: فقط حذف (ویرایش در این فرم وجود ندارد)
            ArchivesGrid.RowDeleteRequested += (s, item) =>
            {
                if (item is not BranchArchiveItem a) return;

                foreach (var x in ViewModel.AllArchives.Where(aa => !aa.IsEmpty))
                    x.IsSelected = ReferenceEquals(x, a);

                BtnDelete_Click(this, new RoutedEventArgs());
            };

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Loaded += BranchArchiveListView_Loaded;
            this.Unloaded += OnViewUnloaded;

            if (FilePicker != null)
                FilePicker.FileSelected += async (s, e) => await AddPickedFileAsync();
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = null;

            _searchDebounceTimer?.Stop();

            this.Unloaded -= OnViewUnloaded;
        }

        // ─── ظاهر گرید شبیه جدول حساب‌های بانکی ───
        // ردیف‌ها همه‌سفید (بدون یک‌درمیان) و هدر ستون‌های داخلی (چک‌باکس و شماره ردیف)
        // هم خط زیرین نازک خاکستری بگیرند مثل بقیه ستون‌ها.
        private void ApplyBankTableStyle()
        {
            // ردیف‌های یک‌درمیان خاکستری (#F8F8F8) خاموش می‌شود → همه ردیف‌ها سفید
            ArchivesGrid.Grid.AlternationCount = 1;

            // هدر ستون‌های داخلی: خط آبی ۲px → خاکستری ۱px (مثل جدول حساب‌های بانکی)
            var gray = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"));
            var builtInThicknesses = new[] { new Thickness(0, 0, 1, 1), new Thickness(0, 0, 1, 1) };
            var cellStyle = (Style)FindResource("BankTableCellStyle");
            for (int i = 0; i < 2 && i < ArchivesGrid.Grid.Columns.Count; i++)
            {
                var baseStyle = ArchivesGrid.Grid.Columns[i].HeaderStyle;
                if (baseStyle == null) continue;
                var s = new Style(typeof(DataGridColumnHeader), baseStyle);
                s.Setters.Add(new Setter(Control.BorderBrushProperty, gray));
                s.Setters.Add(new Setter(Control.BorderThicknessProperty, builtInThicknesses[i]));
                ArchivesGrid.Grid.Columns[i].HeaderStyle = s;

                // سلول‌های ستون‌های داخلی هم خط افقی خاکستری زیر ردیف بگیرند
                ArchivesGrid.Grid.Columns[i].CellStyle = cellStyle;
            }

            // ستون شماره ردیف مثل بقیه فرم‌های گرید (۵۰px)
            if (ArchivesGrid.Grid.Columns.Count > 1)
            {
                var rowNumberCol = ArchivesGrid.Grid.Columns[1];
                rowNumberCol.Width = new DataGridLength(50);
                rowNumberCol.MinWidth = 45;
            }
        }


        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.CloseCurrentForm();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ToastManager.Warning("چاپ این بخش به‌زودی اضافه می‌شود.");
        }


        private async Task AddPickedFileAsync()
        {
            long branchId = 0;
            if (BranchComboBox.SelectedItem is BranchFilterItem b)
                branchId = b.Id;

            if (branchId <= 0)
            {
                ToastManager.Warning("برای افزودن فایل، ابتدا یک شعبه انتخاب کنید.");
                return;
            }

            await ViewModel.AddNewArchiveAsync(
                SearchBox.Text,
                DescriptionSearchBox.Text,
                FilePicker.FilePath,
                branchId);

            SearchBox.Text = "";
            DescriptionSearchBox.Text = "";
            FilePicker.FilePath = null;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BranchArchiveListViewModel.SelectedSummaryText))
            {
                if (SelectedSummaryText != null)
                    SelectedSummaryText.Text = ViewModel.SelectedSummaryText;
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.TotalCountText))
            {
                if (TotalCountText != null)
                    TotalCountText.Text = ViewModel.TotalCountText;
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.CompanyItems))
            {
                if (CompanyComboBox != null)
                    CompanyComboBox.ItemsSource = ViewModel.CompanyItems;
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.BranchItems))
            {
                if (BranchComboBox != null)
                    BranchComboBox.ItemsSource = ViewModel.BranchItems;
            }
        }

        private async void BranchArchiveListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;
            _isLoadedOnce = true;
            try
            {
                await ViewModel.LoadDataAsync();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در بارگذاری بایگانی: " + ex.Message);
            }
        }

        // ─── CRUD ───
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = ViewModel.GetSelectedItems();
            if (selectedItems.Count == 0)
            {
                ToastManager.Warning("لطفاً یک رکورد انتخاب کنید.");
                return;
            }

            var result = MessageBox.Show(
                $"آیا از حذف {selectedItems.Count} مورد انتخاب شده اطمینان دارید؟",
                "تایید حذف", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;
            try
            {
                await ViewModel.DeleteSelectedAsync();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در حذف: " + ex.Message);
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ViewModel.RefreshAsync();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در بروزرسانی: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Cancel action - clear filters
            HeaderSearchBox.Text = "";
            SearchBox.Text = "";
            DescriptionSearchBox.Text = "";
            CompanyComboBox.SelectedItem = null;
            BranchComboBox.SelectedItem = null;
            ViewModel.HandleSearchTextChanged("");
        }

        // ─── Filter ───
        private void CompanyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;
            ViewModel.OnCompanyChanged();
        }

        private void BranchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;
            ViewModel.OnBranchChanged();
        }
    }
}
