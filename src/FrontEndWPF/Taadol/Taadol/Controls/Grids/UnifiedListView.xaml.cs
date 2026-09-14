using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Taadol.Models;

namespace Taadol.Controls
{

    public partial class UnifiedListView : UserControl
    {
        private bool _scrollWired = false;
        private bool _rowBordersUpdatePending = false;
        private bool _builtColumnsHandled = false;

        private bool _itemsSourceEverSet = false;
        private ScrollViewer _innerScrollViewer;

        private const double GridCornerRadius = 3;

        public UnifiedListView()
        {
            InitializeComponent();
            Loaded += UnifiedListView_Loaded;

            DataGridView.SizeChanged += DataGridView_SizeChanged;

            DataGridView.PreviewMouseLeftButtonDown += DataGridRow_PreviewMouseLeftButtonDown;
            DataGridView.MouseLeftButtonDown += DataGridRow_MouseLeftButtonDown;
            DataGridView.PreviewMouseRightButtonDown += DataGridView_PreviewMouseRightButtonDown;

            PaginationBar.NextPageRequested += (s, e) => NextPageRequested?.Invoke(this, e);
            PaginationBar.PreviousPageRequested += (s, e) => PreviousPageRequested?.Invoke(this, e);
            PaginationBar.PageRequested += (s, page) => PageRequested?.Invoke(this, page);
            PaginationBar.PageSizeRequested += (s, size) => PageSizeRequested?.Invoke(this, size);

            RowContextMenu.EditRequested += (s, e) =>
            {
                var item = _contextRowItem;
                _contextRowItem = null;
                if (item != null)
                    RowEditRequested?.Invoke(this, item);
            };

            RowContextMenu.DeleteRequested += (s, e) =>
            {
                var item = _contextRowItem;
                _contextRowItem = null;
                if (item != null)
                    RowDeleteRequested?.Invoke(this, item);
            };
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(UnifiedListView),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    if (control.DataGridView != null)
                    {

                        if (!ReferenceEquals(control.DataGridView.ItemsSource, control.ItemsSource))
                        {
                            control.DataGridView.ItemsSource = null;
                            control.DataGridView.ItemsSource = control.ItemsSource;

                        control.ScheduleRowBordersUpdate();

                            control.Dispatcher.BeginInvoke(new Action(control.ScrollGridToTop),
                                DispatcherPriority.Background);
                        }

                        control._itemsSourceEverSet = true;
                        control.UpdateEmptyState();
                    }
                }));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register(nameof(Footer), typeof(object), typeof(UnifiedListView),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    if (control.FooterSlot == null) return;
                    control.FooterSlot.Content = control.Footer;
                    control.FooterSlot.Visibility = control.Footer == null
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }));

        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty PagesSourceProperty =
            DependencyProperty.Register(nameof(PagesSource), typeof(IEnumerable), typeof(UnifiedListView),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    if (control.PaginationBar != null)
                        control.PaginationBar.PagesSource = control.PagesSource;
                }));

        public IEnumerable PagesSource
        {
            get => (IEnumerable)GetValue(PagesSourceProperty);
            set => SetValue(PagesSourceProperty, value);
        }

        public static readonly DependencyProperty PageInfoContentProperty =
            DependencyProperty.Register(nameof(PageInfoContent), typeof(string), typeof(UnifiedListView),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    if (control.PaginationBar != null)
                        control.PaginationBar.PageInfoContent = control.PageInfoContent;
                }));

        public string PageInfoContent
        {
            get => (string)GetValue(PageInfoContentProperty);
            set => SetValue(PageInfoContentProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(UnifiedListView),
                new PropertyMetadata(false, (d, e) =>
                {
                    var control = (UnifiedListView)d;
                    control.ShowLoading(e.NewValue as bool? == true);
                    control.UpdateLoadError();
                    control.UpdateEmptyState();
                }));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty LoadErrorTextProperty =
            DependencyProperty.Register(nameof(LoadErrorText), typeof(string), typeof(UnifiedListView),
                new PropertyMetadata(string.Empty, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    control.UpdateLoadError();
                    control.UpdateEmptyState();
                }));

        public string LoadErrorText
        {
            get => (string)GetValue(LoadErrorTextProperty);
            set => SetValue(LoadErrorTextProperty, value);
        }

        private bool HasLoadError => !string.IsNullOrWhiteSpace(LoadErrorText);

        public static readonly DependencyProperty ShowEmptyStateProperty =
            DependencyProperty.Register(nameof(ShowEmptyState), typeof(bool), typeof(UnifiedListView),
                new PropertyMetadata(true, (d, _) => ((UnifiedListView)d).UpdateEmptyState()));

        public bool ShowEmptyState
        {
            get => (bool)GetValue(ShowEmptyStateProperty);
            set => SetValue(ShowEmptyStateProperty, value);
        }

        public static readonly DependencyProperty EmptyStateTextProperty =
            DependencyProperty.Register(nameof(EmptyStateText), typeof(string), typeof(UnifiedListView),
                new PropertyMetadata("موردی یافت نشد", (d, _) => ((UnifiedListView)d).ApplyEmptyStateTexts()));

        public string EmptyStateText
        {
            get => (string)GetValue(EmptyStateTextProperty);
            set => SetValue(EmptyStateTextProperty, value);
        }

        public static readonly DependencyProperty EmptyStateHintTextProperty =
            DependencyProperty.Register(nameof(EmptyStateHintText), typeof(string), typeof(UnifiedListView),
                new PropertyMetadata("برای افزودن، از دکمه «جدید» استفاده کنید", (d, _) => ((UnifiedListView)d).ApplyEmptyStateTexts()));

        public string EmptyStateHintText
        {
            get => (string)GetValue(EmptyStateHintTextProperty);
            set => SetValue(EmptyStateHintTextProperty, value);
        }

        private void ApplyEmptyStateTexts()
        {
            if (EmptyStateTextBlock != null)
                EmptyStateTextBlock.Text = EmptyStateText ?? "";
            if (EmptyStateHintBlock != null)
                EmptyStateHintBlock.Text = EmptyStateHintText ?? "";
        }

        private void UpdateEmptyState()
        {
            if (EmptyStateOverlay == null) return;

            var hasRealItems = false;
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    if (item is IListRowItem row && !row.IsEmpty)
                    {
                        hasRealItems = true;
                        break;
                    }
                }
            }

            var show = ShowEmptyState && _itemsSourceEverSet && !hasRealItems && !IsLoading && !HasLoadError;
            EmptyStateOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            UpdateGridMinHeight();
        }

        public static readonly DependencyProperty ShowRowNumberProperty =
            DependencyProperty.Register(nameof(ShowRowNumber), typeof(bool), typeof(UnifiedListView),
                new PropertyMetadata(true));

        public bool ShowRowNumber
        {
            get => (bool)GetValue(ShowRowNumberProperty);
            set => SetValue(ShowRowNumberProperty, value);
        }

        public static readonly DependencyProperty ShowCheckBoxProperty =
            DependencyProperty.Register(nameof(ShowCheckBox), typeof(bool), typeof(UnifiedListView),
                new PropertyMetadata(true));

        public bool ShowCheckBox
        {
            get => (bool)GetValue(ShowCheckBoxProperty);
            set => SetValue(ShowCheckBoxProperty, value);
        }

        public ObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollection<DataGridColumn>();

        public DataGrid Grid => DataGridView;

        public object DoubleClickedItem { get; private set; }

        public void RefreshVisualState()
        {
            UpdateRowBorders();
            UpdateHeaderSelectAllState();
        }

        public event EventHandler NextPageRequested;
        public event EventHandler PreviousPageRequested;
        public event EventHandler<int> PageRequested;
        public event EventHandler<int> PageSizeRequested;
        public event EventHandler GridDoubleClicked;
        public event EventHandler CheckedItemsChanged;

        public event EventHandler<bool> SelectAllToggled;

        public event EventHandler<IListRowItem> RowEditRequested;

        public event EventHandler<IListRowItem> RowDeleteRequested;

        private void UnifiedListView_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyColumnVisibility();

            UpdateGridMinHeight();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                WireScrollChanged();
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
            }), DispatcherPriority.Background);
        }

        private void UpdateGridMinHeight()
        {
            if (GridArea == null) return;

            var hasRealItems = false;
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    if (item is IListRowItem row && !row.IsEmpty)
                    {
                        hasRealItems = true;
                        break;
                    }
                }
            }

            GridArea.MinHeight = (!hasRealItems || IsLoading) ? 200 : 0;
        }

        private void ApplyColumnVisibility()
        {
            if (_builtColumnsHandled || DataGridView == null) return;
            _builtColumnsHandled = true;

            var checkbox = DataGridView.Columns.FirstOrDefault(c => c.Header == null && c is DataGridTemplateColumn);
            var rowNumber = DataGridView.Columns.FirstOrDefault(c => c is DataGridTextColumn &&
                                                                      c.Header is string s && s == "#");

            if (checkbox != null && !ShowCheckBox)
                DataGridView.Columns.Remove(checkbox);
            if (rowNumber != null && !ShowRowNumber)
                DataGridView.Columns.Remove(rowNumber);

            foreach (var column in Columns)
                DataGridView.Columns.Add(column);
        }

        private void ShowLoading(bool show)
        {
            if (PaginationBar != null)
                PaginationBar.IsProcessing = show;

            if (LoadingOverlay != null)
                LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (DataGridView != null)
                DataGridView.IsHitTestVisible = !show;

            UpdateGridMinHeight();
        }

        private void UpdateLoadError()
        {
            if (LoadErrorOverlay == null) return;

            if (LoadErrorTextBlock != null)
                LoadErrorTextBlock.Text = string.IsNullOrWhiteSpace(LoadErrorText)
                    ? "خطا در بارگذاری اطلاعات"
                    : LoadErrorText;

            LoadErrorOverlay.Visibility = HasLoadError && !IsLoading
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void DataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridView.SelectedItem != null)
                DataGridView.SelectedItem = null;
        }

        private void DataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GetRowFromMouse(e.OriginalSource as DependencyObject) is DataGridRow row &&
                row.DataContext is IListRowItem item && !item.IsEmpty)
            {
                DoubleClickedItem = row.DataContext;
                GridDoubleClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private IListRowItem _contextRowItem;

        private void DataGridView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GetRowFromMouse(e.OriginalSource as DependencyObject) is DataGridRow row &&
                row.DataContext is IListRowItem item && !item.IsEmpty)
            {
                _contextRowItem = item;
                RowContextMenu.ShowMenu(RowEditRequested != null, RowDeleteRequested != null,
                    e.GetPosition(RowContextMenu));
                e.Handled = true;
            }
        }

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (GetRowFromMouse(e.OriginalSource as DependencyObject) == null)
                return;

            if (!IsInsideCheckBox(e.OriginalSource as DependencyObject) && !IsInsideFilterButton(e.OriginalSource as DependencyObject))
                e.Handled = true;
        }

        private void DataGridRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GetRowFromMouse(e.OriginalSource as DependencyObject) == null)
                return;

            if (!IsInsideCheckBox(e.OriginalSource as DependencyObject) && !IsInsideFilterButton(e.OriginalSource as DependencyObject))
                e.Handled = true;
        }

        private static DataGridRow GetRowFromMouse(DependencyObject originalSource)
        {
            return FindAncestor<DataGridRow>(originalSource);
        }

        private static T FindAncestor<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T match) return match;

                element = element is Visual || element is System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(element)
                    : LogicalTreeHelper.GetParent(element);
            }
            return null;
        }

        private bool IsInsideCheckBox(DependencyObject element)
        {
            while (element != null)
            {
                if (element is FrameworkElement fe &&
                    (fe.Name == "CheckBoxBorder" || fe.Name == "SelectAllBorder"))
                    return true;
                if (element is Visual)
                    element = VisualTreeHelper.GetParent(element);
                else
                    break;
            }
            return false;
        }

        private bool IsInsideFilterButton(DependencyObject element)
        {
            while (element != null)
            {
                if (element is Button)
                    return true;
                if (element is Visual)
                    element = VisualTreeHelper.GetParent(element);
                else
                    break;
            }
            return false;
        }

        private void CheckBoxBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is IListRowItem item && !item.IsEmpty)
            {
                item.IsSelected = !item.IsSelected;
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
                CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        private void RowToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is IListRowItem item && !item.IsEmpty)
            {

                item.IsSelected = true;
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
                CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RowToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is IListRowItem item && !item.IsEmpty)
            {

                item.IsSelected = false;
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
                CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DataGridView_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            ScheduleRowBordersUpdate();
        }

        private static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T found)
                    return found;
                var result = FindChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void SelectAllBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ToggleButton)
            {
                var realItems = GetRealItems();
                var allSelected = realItems.Count > 0 && realItems.All(p => p.IsSelected);
                var newState = !allSelected;

                foreach (var item in realItems)
                    item.IsSelected = newState;

                SelectAllToggled?.Invoke(this, newState);

                UpdateRowBorders();
                UpdateHeaderSelectAllState();
                CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        private List<IListRowItem> GetRealItems()
        {
            if (DataGridView == null || DataGridView.Items == null) return new List<IListRowItem>();
            return DataGridView.Items
                .OfType<IListRowItem>()
                .Where(i => !i.IsEmpty)
                .ToList();
        }

        private void UpdateHeaderSelectAllState()
        {
            if (DataGridView == null) return;

            var toggle = FindDescendantByName(DataGridView, "SelectAllBorder") as ToggleButton;
            if (toggle == null) return;

            var realItems = GetRealItems();
            var allSelected = realItems.Count > 0 && realItems.All(p => p.IsSelected);
            toggle.IsChecked = allSelected;
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
                if (result != null)
                    return result;
            }
            return null;
        }

        private void ScheduleRowBordersUpdate()
        {
            if (_rowBordersUpdatePending || Dispatcher.HasShutdownStarted)
                return;

            _rowBordersUpdatePending = true;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _rowBordersUpdatePending = false;
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
            }), DispatcherPriority.Background);
        }

        private void UpdateRowBorders()
        {
            if (DataGridView == null) return;

            var items = DataGridView.Items;
            var blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
            var transparent = Brushes.Transparent;

            int lastRealIndex = -1;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is IListRowItem rowItem && !rowItem.IsEmpty)
                {
                    lastRealIndex = i;
                    break;
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                var row = DataGridView.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (row == null) continue;

                if (items[i] is not IListRowItem item || item.IsEmpty)
                {
                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                    NormalizeOuterCellBorder(row);
                    continue;
                }

                bool previousRowSelected = i > 0 &&
                    items[i - 1] is IListRowItem previousItem &&
                    !previousItem.IsEmpty &&
                    previousItem.IsSelected;
                bool isLastRealRow = i == lastRealIndex;

                if (item.IsSelected)
                {

                    row.BorderBrush = blue;
                    row.BorderThickness = new Thickness(0, previousRowSelected ? 0 : 1, 0, 1);
                }
                else if (isLastRealRow)
                {

                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                }
                else
                {
                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                }

                NormalizeOuterCellBorder(row);
            }

            NormalizeOuterHeaderBorders();
        }

        private void NormalizeOuterCellBorder(DataGridRow row)
        {
            if (row == null || DataGridView == null)
                return;

            foreach (var cell in FindVisualChildren<DataGridCell>(row))
            {

                var thickness = cell.BorderThickness;
                if (cell.TransformToAncestor(DataGridView).Transform(new Point(0, 0)).X <= 0)
                    cell.BorderThickness = new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom);

                if (cell.TransformToAncestor(DataGridView).Transform(new Point(cell.ActualWidth, 0)).X >= DataGridView.ActualWidth)
                {
                    thickness = cell.BorderThickness;
                    cell.BorderThickness = new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom);
                }
            }
        }

        private void NormalizeOuterHeaderBorders()
        {
            if (DataGridView == null)
                return;

            foreach (var header in FindVisualChildren<DataGridColumnHeader>(DataGridView))
            {
                var position = header.TransformToAncestor(DataGridView).Transform(new Point(0, 0));
                var thickness = header.BorderThickness;
                if (position.X <= 0)
                    header.BorderThickness = new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom);

                if (position.X + header.ActualWidth >= DataGridView.ActualWidth)
                {
                    thickness = header.BorderThickness;
                    header.BorderThickness = new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom);
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T match)
                    yield return match;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }

        private void WireScrollChanged()
        {
            if (DataGridView == null || _scrollWired) return;

            if (FindDescendantByName(DataGridView, "DG_ScrollViewer") is ScrollViewer sv)
            {
                _innerScrollViewer = sv;
                sv.Padding = new Thickness(0);
                sv.Margin = new Thickness(0);
                sv.VerticalContentAlignment = VerticalAlignment.Top;

                _scrollWired = true;
                sv.ScrollChanged += (s, _) => ScheduleRowBordersUpdate();
            }
        }

        private void DataGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataGridView == null || DataGridView.ActualWidth <= 0 || DataGridView.ActualHeight <= 0) return;
            DataGridView.Clip = new RectangleGeometry(
                new Rect(0, 0, DataGridView.ActualWidth, DataGridView.ActualHeight),
                GridCornerRadius, GridCornerRadius);
        }

        private void ScrollGridToTop()
        {
            _innerScrollViewer?.ScrollToTop();
        }

    }
}
