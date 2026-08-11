using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Controls;
using Taadol.ViewModels;

namespace Taadol.Views
{
    public partial class BranchArchiveListView : UserControl
    {
        public BranchArchiveListViewModel ViewModel { get; }
        private bool _isLoadedOnce;

        public BranchArchiveListView()
        {
            InitializeComponent();

            ViewModel = new BranchArchiveListViewModel(App.ServiceProvider);
            DataContext = ViewModel;

            HeaderSearchBox.TextChanged += (s, e) =>
                ViewModel.HandleSearchTextChanged(HeaderSearchBox.Text);

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

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Loaded += BranchArchiveListView_Loaded;

            if (FilePicker != null)
                FilePicker.FileSelected += async (s, e) => await AddPickedFileAsync();
        }

        private async Task AddPickedFileAsync()
        {
            long branchId = 0;
            if (BranchComboBox.SelectedItem is BranchFilterItem b)
                branchId = b.Id;

            if (branchId <= 0)
            {
                MessageBox.Show("برای افزودن فایل، ابتدا یک شعبه انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("لطفاً یک رکورد انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Save action - for now just show a message
            MessageBox.Show("ذخیره شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
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
