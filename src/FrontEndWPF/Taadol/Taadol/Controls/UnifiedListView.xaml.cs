using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Taadol.Models;

namespace Taadol.Controls
{
    /// <summary>
    /// گرید لیست مشترک: استایل‌های یکسان هدر/سلول/ردیف، ستون چک‌باکس، شماره ردیف،
    /// صفحه‌بندی و لودینگ. ستون‌های هر فرم از طریق خاصیت Columns اضافه می‌شوند.
    /// </summary>
    public partial class UnifiedListView : UserControl
    {
        private bool _scrollWired = false;
        private bool _builtColumnsHandled = false;

        public UnifiedListView()
        {
            InitializeComponent();
            Loaded += UnifiedListView_Loaded;

            DataGridView.PreviewMouseLeftButtonDown += DataGridRow_PreviewMouseLeftButtonDown;
            DataGridView.MouseLeftButtonDown += DataGridRow_MouseLeftButtonDown;

            PaginationBar.NextPageRequested += (s, e) => NextPageRequested?.Invoke(this, e);
            PaginationBar.PreviousPageRequested += (s, e) => PreviousPageRequested?.Invoke(this, e);
            PaginationBar.PageRequested += (s, page) => PageRequested?.Invoke(this, page);
            PaginationBar.PageSizeRequested += (s, size) => PageSizeRequested?.Invoke(this, size);
        }

        // ══════════════════════════════════════════════════════
        //  Dependency Properties
        // ══════════════════════════════════════════════════════

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(UnifiedListView),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    if (control.DataGridView != null)
                        control.DataGridView.ItemsSource = control.ItemsSource;

                    control.Dispatcher.BeginInvoke(new Action(control.RefreshVisualState), DispatcherPriority.Loaded);
                }));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
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
                }));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
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

        // ══════════════════════════════════════════════════════
        //  Public Surface
        // ══════════════════════════════════════════════════════

        /// <summary>ستون‌های دامنه هر فرم (بعد از چک‌باکس و شماره ردیف اضافه می‌شوند).</summary>
        public ObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollection<DataGridColumn>();

        /// <summary>دسترسی مستقیم به DataGrid داخلی.</summary>
        public DataGrid Grid => DataGridView;

        /// <summary>ردیفی که دوبار کلیک شده است (قبل از رویداد GridDoubleClicked مقدار می‌گیرد).</summary>
        public object DoubleClickedItem { get; private set; }

        /// <summary>رسم مجدد خطوط ردیف‌ها و وضعیت چک‌باکس سرستون (بعد از تغییر داده‌ها).</summary>
        public void RefreshVisualState()
        {
            UpdateRowBorders();
            UpdateHeaderSelectAllState();
        }

        // رویدادها — فرم‌ها برای اتصال به ViewModel خود subscribe می‌کنند
        public event EventHandler NextPageRequested;
        public event EventHandler PreviousPageRequested;
        public event EventHandler<int> PageRequested;
        public event EventHandler<int> PageSizeRequested;
        public event EventHandler GridDoubleClicked;
        public event EventHandler CheckedItemsChanged;

        // ══════════════════════════════════════════════════════
        //  Lifecycle
        // ══════════════════════════════════════════════════════

        private void UnifiedListView_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyColumnVisibility();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                WireScrollChanged();
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
            }), DispatcherPriority.Background);
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

        // ══════════════════════════════════════════════════════
        //  Loading
        // ══════════════════════════════════════════════════════

        private void ShowLoading(bool show)
        {
            if (LoadingOverlay != null)
                LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (DataGridView != null)
                DataGridView.IsHitTestVisible = !show;
        }

        // ══════════════════════════════════════════════════════
        //  DataGrid Visual Handlers
        // ══════════════════════════════════════════════════════

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

        private static DataGridRow GetRowFromMouse(DependencyObject originalSource)
        {
            return FindAncestor<DataGridRow>(originalSource);
        }

        private static T FindAncestor<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T match) return match;
                element = VisualTreeHelper.GetParent(element);
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
                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }

        // ══════════════════════════════════════════════════════
        //  CheckBox / Select-All
        // ══════════════════════════════════════════════════════

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

        private void SelectAllBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border)
            {
                var realItems = GetRealItems();
                var allSelected = realItems.Count > 0 && realItems.All(p => p.IsSelected);
                foreach (var item in realItems)
                    item.IsSelected = !allSelected;

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

            var headerBorder = FindDescendantByName(DataGridView, "SelectAllBorder") as Border;
            if (headerBorder == null) return;

            var realItems = GetRealItems();
            var allSelected = realItems.Count > 0 && realItems.All(p => p.IsSelected);

            if (headerBorder.FindName("SelectAllCheckMark") is FrameworkElement checkMark)
                checkMark.Visibility = allSelected ? Visibility.Visible : Visibility.Collapsed;

            var blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
            headerBorder.Background = allSelected ? blue : Brushes.White;
            headerBorder.BorderBrush = allSelected ? blue
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
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

        private void UpdateRowBorders()
        {
            if (DataGridView == null) return;

            var items = DataGridView.Items;
            var blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
            var transparent = Brushes.Transparent;
            var gray = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"));

            int lastRealIndex = -1;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is IListRowItem p && !p.IsEmpty)
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
                    continue;
                }

                bool prevSelected = (i > 0) && items[i - 1] is IListRowItem prev && !prev.IsEmpty && prev.IsSelected;
                bool isLast = (i == lastRealIndex);

                if (item.IsSelected)
                {
                    row.BorderBrush = blue;
                    row.BorderThickness = new Thickness(0, prevSelected ? 0 : 1, 0, 1);
                }
                else if (isLast)
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

        // ══════════════════════════════════════════════════════
        //  Scroll & Layout
        // ══════════════════════════════════════════════════════

        private void WireScrollChanged()
        {
            if (DataGridView == null || _scrollWired) return;
            if (FindDescendantByName(DataGridView, "DG_ScrollViewer") is ScrollViewer sv)
            {
                _scrollWired = true;
                sv.ScrollChanged += (s, _) =>
                    Dispatcher.BeginInvoke(new Action(UpdateRowBorders), DispatcherPriority.Background);
            }
        }
    }
}
