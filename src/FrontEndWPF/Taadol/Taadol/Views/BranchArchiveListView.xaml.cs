using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Taadol.ViewModels;

namespace Taadol.Views
{
    public partial class BranchArchiveListView : UserControl
    {
        public BranchArchiveListViewModel ViewModel { get; }
        private bool _isLoadedOnce;
        private bool _scrollWired;

        public BranchArchiveListView()
        {
            InitializeComponent();

            ViewModel = new BranchArchiveListViewModel(App.ServiceProvider);
            DataContext = ViewModel;

            HeaderSearchBox.TextChanged += (s, e) =>
                ViewModel.HandleSearchTextChanged(HeaderSearchBox.Text);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Loaded += BranchArchiveListView_Loaded;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BranchArchiveListViewModel.FilteredArchives))
            {
                if (ArchivesDataGrid != null)
                    ArchivesDataGrid.ItemsSource = ViewModel.FilteredArchives;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateRowBorders();
                }), DispatcherPriority.Loaded);
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.Pages))
            {
                if (PageButtonsItemsControl != null)
                    PageButtonsItemsControl.ItemsSource = ViewModel.Pages;
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.IsLoading))
            {
                ShowLoading(ViewModel.IsLoading);
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.PageInfoText))
            {
                if (PageInfoText != null)
                    PageInfoText.Text = ViewModel.PageInfoText;
            }
            else if (e.PropertyName == nameof(BranchArchiveListViewModel.SelectedSummaryText))
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
            await ViewModel.LoadDataAsync();
            WireScrollChanged();
        }

        private void ShowLoading(bool show)
        {
            if (LoadingOverlay != null)
                LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (ArchivesDataGrid != null)
                ArchivesDataGrid.IsHitTestVisible = !show;
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
            await ViewModel.DeleteSelectedAsync();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.RefreshAsync();
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

        // ─── Pagination ───
        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
            => ViewModel.GoToPreviousPage();

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
            => ViewModel.GoToNextPage();

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int page)
                ViewModel.GoToPage(page);
        }

        private void PageSizeSelector_SelectionChanged(object sender, int newSize)
            => ViewModel.ChangePageSize(newSize);

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

        // ─── DataGrid Visual ───
        private void ArchivesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArchivesDataGrid.SelectedItem != null)
                ArchivesDataGrid.SelectedItem = null;
        }

        private void ArchivesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Double-click edit placeholder
        }

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsInsideCheckBox(e.OriginalSource as DependencyObject))
                e.Handled = true;
        }

        private void DataGridRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsInsideCheckBox(e.OriginalSource as DependencyObject))
                e.Handled = true;
        }

        private bool IsInsideCheckBox(DependencyObject element)
        {
            while (element != null)
            {
                if (element is FrameworkElement fe && fe.Name == "CheckBoxBorder")
                    return true;
                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }

        private void CheckBoxBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is BranchArchiveItem item)
            {
                item.IsSelected = !item.IsSelected;
                ViewModel.UpdateSelectedCount();
            }
        }

        private void SelectAllBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool allSelected = ViewModel.FilteredArchives != null &&
                !ViewModel.FilteredArchives.Any(p => !p.IsEmpty && !p.IsSelected);

            if (allSelected)
                ViewModel.DeselectAll();
            else
                ViewModel.SelectAll();

            UpdateHeaderSelectAllState();
            e.Handled = true;
        }

        private void UpdateHeaderSelectAllState()
        {
            if (ArchivesDataGrid == null || ViewModel.FilteredArchives == null) return;

            var headerBorder = FindDescendantByName(ArchivesDataGrid, "SelectAllBorder") as Border;
            if (headerBorder == null) return;

            var checkMark = FindDescendantByName(headerBorder, "SelectAllCheckMark");
            bool allSelected = true;
            foreach (var item in ViewModel.FilteredArchives)
            {
                if (!item.IsEmpty && !item.IsSelected)
                {
                    allSelected = false;
                    break;
                }
            }

            if (checkMark is FrameworkElement fe)
                fe.Visibility = allSelected ? Visibility.Visible : Visibility.Collapsed;

            headerBorder.Background = allSelected
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"))
                : new SolidColorBrush(Colors.White);
            headerBorder.BorderBrush = allSelected
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
        }

        private void UpdateRowBorders()
        {
            if (ArchivesDataGrid == null) return;

            var items = ArchivesDataGrid.Items;
            var gray = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#D1D5DB"));
            var transparent = System.Windows.Media.Brushes.Transparent;

            int lastRealIndex = -1;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is BranchArchiveItem p && !p.IsEmpty)
                {
                    lastRealIndex = i;
                    break;
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                var row = ArchivesDataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (row == null) continue;

                var item = items[i] as BranchArchiveItem;
                if (item == null || item.IsEmpty)
                {
                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                    continue;
                }

                bool isLast = (i == lastRealIndex);

                if (isLast)
                {
                    row.BorderBrush = gray;
                    row.BorderThickness = new Thickness(0, 0, 0, 1);
                }
                else
                {
                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                }
            }
        }

        private void WireScrollChanged()
        {
            if (ArchivesDataGrid == null || _scrollWired) return;
            if (FindDescendantByName(ArchivesDataGrid, "DG_ScrollViewer") is ScrollViewer sv)
            {
                _scrollWired = true;
                sv.ScrollChanged += (s, _) =>
                    Dispatcher.BeginInvoke(new Action(UpdateRowBorders), DispatcherPriority.Background);
            }
        }

        private static DependencyObject FindDescendantByName(DependencyObject root, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is FrameworkElement fe && fe.Name == name)
                    return child;
                var result = FindDescendantByName(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
