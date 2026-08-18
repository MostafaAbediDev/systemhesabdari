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
    /// <summary>
    /// گرید لیست مشترک: استایل‌های یکسان هدر/سلول/ردیف، ستون چک‌باکس، شماره ردیف،
    /// صفحه‌بندی و لودینگ. ستون‌های هر فرم از طریق خاصیت Columns اضافه می‌شوند.
    /// </summary>
    public partial class UnifiedListView : UserControl
    {
        private bool _scrollWired = false;
        private bool _builtColumnsHandled = false;
        // فقط بعد از اولین ست شدن ItemsSource پیام خالی نمایش داده می‌شود؛
        // وگرنه قبل از شروع لود، فلش خالی وسط گرید دیده می‌شود.
        private bool _itemsSourceEverSet = false;

        public UnifiedListView()
        {
            InitializeComponent();
            Loaded += UnifiedListView_Loaded;

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

        // ══════════════════════════════════════════════════════
        //  Dependency Properties
        // ══════════════════════════════════════════════════════

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(UnifiedListView),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (UnifiedListView)d;
                    if (control.DataGridView != null)
                    {
                        // ابتدا ItemsSource را null کن تا کانتینرهای قدیمی (ردیف‌های صفحه قبلی)
                        // کاملاً تخریب شوند؛ وگرنه با reuse شدن کانتینرها و اشتراک آیتم‌ها،
                        // شماره ردیف قدیمی/تکراری نمایش داده می‌شود.
                        control.DataGridView.ItemsSource = null;
                        control.DataGridView.ItemsSource = control.ItemsSource;

                        // بعد از تغییر صفحه/فیلتر، وضعیت چک‌باکس سرستون باید دوباره محاسبه شود
                        // (چک‌باکس هدر فقط وقتی روشن است که همه‌ی ردیف‌های همین صفحه انتخاب باشند).
                        control.Dispatcher.BeginInvoke(new Action(() => control.RefreshVisualState()),
                            DispatcherPriority.Background);

                        // با تغییر صفحه/فیلتر، اسکرول عمودی باید به بالای لیست برگردد؛
                        // وگرنه کاربر در وسط/انتهای صفحه‌ی قبلی می‌ماند و ردیف‌های خالی/بی‌ساختار را می‌بیند.
                        control.Dispatcher.BeginInvoke(new Action(control.ScrollGridToTop),
                            DispatcherPriority.Background);

                        control._itemsSourceEverSet = true;
                        control.UpdateEmptyState();
                    }
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
                    control.UpdateEmptyState();
                }));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        // ══════════════════════════════════════════════════════
        //  Empty State (وقتی هیچ رکورد واقعی‌ای وجود ندارد)
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// وقتی true باشد، اگر ItemsSource هیچ رکورد واقعی (غیر از ردیف‌های خالی پرکننده) نداشته باشد،
        /// پیام خالی وسط گرید نمایش داده می‌شود. به‌طور پیش‌فرض true است تا همه لیست‌ها یکسان رفتار کنند.
        /// </summary>
        public static readonly DependencyProperty ShowEmptyStateProperty =
            DependencyProperty.Register(nameof(ShowEmptyState), typeof(bool), typeof(UnifiedListView),
                new PropertyMetadata(true, (d, _) => ((UnifiedListView)d).UpdateEmptyState()));

        public bool ShowEmptyState
        {
            get => (bool)GetValue(ShowEmptyStateProperty);
            set => SetValue(ShowEmptyStateProperty, value);
        }

        /// <summary>متن اصلی پیام خالی (فرم می‌تواند آن را سفارشی کند).</summary>
        public static readonly DependencyProperty EmptyStateTextProperty =
            DependencyProperty.Register(nameof(EmptyStateText), typeof(string), typeof(UnifiedListView),
                new PropertyMetadata("موردی یافت نشد", (d, _) => ((UnifiedListView)d).ApplyEmptyStateTexts()));

        public string EmptyStateText
        {
            get => (string)GetValue(EmptyStateTextProperty);
            set => SetValue(EmptyStateTextProperty, value);
        }

        /// <summary>زیرنویس کوچک پیام خالی (فرم می‌تواند آن را سفارشی کند).</summary>
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

            // لودینگ اولویت دارد؛ و پیام فقط بعد از اولین بارگذاری نمایش داده می‌شود
            var show = ShowEmptyState && _itemsSourceEverSet && !hasRealItems && !IsLoading;
            EmptyStateOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
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

        /// <summary>
        /// چک‌باکس سرستون فقط همین صفحه را انتخاب می‌کند؛ ولی وقتی خاموش می‌شود،
        /// فرم باید انتخابِ صفحات دیگر را هم پاک کند تا چیزی انتخاب‌شده باقی نماند.
        /// مقدار bool = حالت جدید (true = روشن/انتخاب همین صفحه، false = خاموش/پاک کردن همه).
        /// </summary>
        public event EventHandler<bool> SelectAllToggled;

        /// <summary>راست‌کلیک روی ردیف → «ویرایش» انتخاب شد.</summary>
        public event EventHandler<IListRowItem> RowEditRequested;

        /// <summary>راست‌کلیک روی ردیف → «حذف» انتخاب شد.</summary>
        public event EventHandler<IListRowItem> RowDeleteRequested;

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
            // فقط کلیک‌های داخل ردیف مصرف شوند؛ کلیک روی هدر، اسکرول‌بار یا فضای خالی
            // نباید بلوک شود وگرنه کشیدن thumb اسکرول افقی/عمودی کار نمی‌کند.
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

                // راست‌کلیک/کلیک روی متن سلول، OriginalSource را یک Run (ContentElement)
                // می‌کند که Visual نیست؛ VisualTreeHelper.GetParent روی آن
                // InvalidOperationException می‌اندازد (باگ Runtime تأییدشده).
                // برای عناصر غیر-Visual از LogicalTree پدر را بالا می‌رویم.
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

        private void RowToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is IListRowItem item && !item.IsEmpty)
            {
                // صریح ست می‌کنیم چون رویداد Checked قبل از نوشتن بایندینگ TwoWay
                // روی item.IsSelected فایر می‌شود؛ بدون این خط، مصرف‌کنندگانِ رویداد
                // (پنل جزئیات، جمع‌ها) هنوز مقدار قدیمی را می‌بینند.
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
                // همان توضیح بالا — بایندینگ هنوز IsSelected را false نکرده است.
                item.IsSelected = false;
                UpdateRowBorders();
                UpdateHeaderSelectAllState();
                CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DataGridView_LoadingRow(object sender, DataGridRowEventArgs e)
        {
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

                // وقتی هدر خاموش می‌شود، فرم باید انتخابِ بقیه صفحات را هم پاک کند
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

            // چک‌باکس سرستون همان تمپلیت ردیف‌ها (GridCheckBoxTemplate) را دارد؛
            // فقط IsChecked را ست می‌کنیم تا تریگرهای تمپلیت ظاهر یکسان را بسازند.
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

        /// <summary>اسکرول عمودی گرید را به بالای لیست برمی‌گرداند (بعد از تغییر صفحه/فیلتر).</summary>
        private void ScrollGridToTop()
        {
            if (DataGridView == null) return;
            if (FindDescendantByName(DataGridView, "DG_ScrollViewer") is ScrollViewer sv)
                sv.ScrollToVerticalOffset(0);
        }
    }
}
