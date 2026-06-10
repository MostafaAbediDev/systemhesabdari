using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Taadol.Controls
{
    public partial class CategorySearchControl : UserControl
    {
        private bool _isOpen = false;
        private bool _isAnimating = false;
        private List<CategoryItem> _allCategories;
        private DispatcherTimer _searchTimer;
        private string _lastSearchText = "";
        private Dictionary<TreeViewItem, Border> _dotCache = new Dictionary<TreeViewItem, Border>();

        // Events
        public event Action<CategoryItem> CategorySelected;
        public event Action<CategoryItem> ItemAdded;
        public event Action<CategoryItem> ItemEdited;
        public event Action<CategoryItem> ItemDeleted;

        private static readonly Color[] DotColors = new[]
        {
            Color.FromRgb(0x26, 0x67, 0xFF),
            Color.FromRgb(0x78, 0x9B, 0xEA),
            Color.FromRgb(0xC0, 0xD0, 0xF4),
            Color.FromRgb(0xD4, 0xE1, 0xFF),
        };

        public static readonly DependencyProperty MainIconSourceProperty =
            DependencyProperty.Register("MainIconSource", typeof(string), typeof(CategorySearchControl),
                new PropertyMetadata("/Assets/Icons/profile_u.svg", OnMainIconChanged));

        public string MainIconSource
        {
            get => (string)GetValue(MainIconSourceProperty);
            set => SetValue(MainIconSourceProperty, value);
        }

        private static void OnMainIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CategorySearchControl;
            if (control?.MainIcon != null && e.NewValue is string newPath)
                control.MainIcon.Source = new Uri(newPath, UriKind.Relative);
        }

        public CategorySearchControl()
        {
            InitializeComponent();

            VirtualizingPanel.SetIsVirtualizing(CategoryTree, true);
            VirtualizingPanel.SetVirtualizationMode(CategoryTree, VirtualizationMode.Recycling);

            LoadSampleData();

            _searchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _searchTimer.Tick += SearchTimer_Tick;
        }

        private void LoadSampleData()
        {
            _allCategories = new List<CategoryItem>();

            var customer = new CategoryItem { Title = "مشتری", Level = 0 };
            var customerBulk = new CategoryItem { Title = "مشتری عمده", Level = 1 };
            customerBulk.Children.Add(new CategoryItem { Title = "مشتری عمده شیراز", Level = 2 });
            customerBulk.Children.Add(new CategoryItem { Title = "مشتری عمده تهران", Level = 2 });
            var customerRetail = new CategoryItem { Title = "مشتری خرده", Level = 1 };
            customerRetail.Children.Add(new CategoryItem { Title = "مشتری خرده جهرم", Level = 2 });
            customerRetail.Children.Add(new CategoryItem { Title = "مشتری خرده بجگال", Level = 2 });
            customer.Children.Add(customerBulk);
            customer.Children.Add(customerRetail);
            customer.Children.Add(new CategoryItem { Title = "مشتری شیراز", Level = 1 });
            customer.Children.Add(new CategoryItem { Title = "مشتری تهران", Level = 1 });
            customer.Children.Add(new CategoryItem { Title = "مشتری بجگال", Level = 1 });
            customer.Children.Add(new CategoryItem { Title = "مشتری جهرم", Level = 1 });

            var customerSupplier = new CategoryItem { Title = "مشتری و تامین کننده", Level = 0 };
            customerSupplier.Children.Add(new CategoryItem { Title = "مشتری و پرسنل", Level = 1 });
            customerSupplier.Children.Add(new CategoryItem { Title = "تامین کننده و پرسنل", Level = 1 });

            var supplier = new CategoryItem { Title = "تامین کننده", Level = 0 };
            supplier.Children.Add(new CategoryItem { Title = "تامین کننده عمده", Level = 1 });
            supplier.Children.Add(new CategoryItem { Title = "تامین کارتن", Level = 1 });
            supplier.Children.Add(new CategoryItem { Title = "تامین بجگال", Level = 1 });
            supplier.Children.Add(new CategoryItem { Title = "تامین تلویزیون", Level = 1 });

            var personnel = new CategoryItem { Title = "پرسنل", Level = 0 };
            personnel.Children.Add(new CategoryItem { Title = "مدیران", Level = 1 });
            personnel.Children.Add(new CategoryItem { Title = "کارمندان", Level = 1 });
            personnel.Children.Add(new CategoryItem { Title = "پیمانکاران", Level = 1 });

            _allCategories.Add(customer);
            _allCategories.Add(customerSupplier);
            _allCategories.Add(supplier);
            _allCategories.Add(personnel);

            BuildTree(_allCategories);
        }

        private void BuildTree(List<CategoryItem> items, TreeViewItem parent = null)
        {
            if (parent == null)
            {
                CategoryTree.Items.Clear();
                _dotCache.Clear();
            }

            foreach (var item in items)
            {
                var treeItem = new TreeViewItem
                {
                    Header = item.Title,
                    Tag = item
                };

                if (item.Level == 0)
                    treeItem.SetResourceReference(FrameworkElement.StyleProperty, "RootTreeViewItemStyle");
                else if (item.Level == 1)
                    treeItem.SetResourceReference(FrameworkElement.StyleProperty, "ChildTreeViewItemStyle");
                else
                    treeItem.SetResourceReference(FrameworkElement.StyleProperty, "GrandChildTreeViewItemStyle");

                if (item.HasChildren)
                    BuildTree(item.Children.ToList(), treeItem);

                if (parent == null)
                    CategoryTree.Items.Add(treeItem);
                else
                    parent.Items.Add(treeItem);

                // اتچ کردن hover ایونت‌ها بعد از لود شدن آیتم
                treeItem.Loaded += TreeItem_Loaded;
            }
        }

        // ==================== Hover Isolation (راه‌حل اصلی) ====================
        // مشکل: IsMouseOver در WPF به parent ها bubble میکنه
        // راه‌حل: MouseEnter/Leave روی Border داخلی "Bd" که فقط ناحیه خودش رو cover میکنه

        private void TreeItem_Loaded(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as TreeViewItem;
            if (treeItem == null) return;

            // پیدا کردن Border داخلی "Bd" - این border فقط همون row خودشه، نه فرزندها
            var bd = FindChild<Border>(treeItem, "Bd");
            if (bd != null)
            {
                // MouseEnter/Leave روی Border مستقیم - bubble نمیکنه به parent TreeViewItem
                bd.MouseEnter += TreeItemBorder_MouseEnter;
                bd.MouseLeave += TreeItemBorder_MouseLeave;
            }

            // اعمال رنگ dot
            if (treeItem.Tag is CategoryItem catItem)
                ApplyDotColor(treeItem, catItem.Level);
        }

        private void TreeItemBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            var bd = sender as Border;
            if (bd == null) return;

            // پیدا کردن ActionButtons درون همین border
            var actionButtons = FindChild<StackPanel>(bd, "ActionButtons");
            var bdBrush = FindChild<SolidColorBrush>(bd, "BdBrush");

            // اگر SolidColorBrush مستقیم پیدا نشد، از Background بگیریم
            var brush = bd.Background as SolidColorBrush;

            AnimateHoverIn(bd, actionButtons);
        }

        private void TreeItemBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            var bd = sender as Border;
            if (bd == null) return;

            var actionButtons = FindChild<StackPanel>(bd, "ActionButtons");
            AnimateHoverOut(bd, actionButtons);
        }

        private void AnimateHoverIn(Border bd, StackPanel actionButtons)
        {
            if (bd == null) return;

            // انیمیشن نرم برای background
            var colorAnim = new ColorAnimation
            {
                To = Color.FromRgb(0xF0, 0xF5, 0xFF),
                Duration = TimeSpan.FromMilliseconds(280),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            if (bd.Background is SolidColorBrush brush)
            {
                // اگر brush freeze شده باشه، یه نمونه جدید بساز
                if (brush.IsFrozen)
                {
                    brush = new SolidColorBrush(brush.Color);
                    bd.Background = brush;
                }
                brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
            else
            {
                var newBrush = new SolidColorBrush(Colors.Transparent);
                bd.Background = newBrush;
                newBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }

            // انیمیشن نرم برای دکمه‌ها
            if (actionButtons != null)
            {
                var fadeAnim = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(280),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                actionButtons.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }
        }

        private void AnimateHoverOut(Border bd, StackPanel actionButtons)
        {
            if (bd == null) return;

            var colorAnim = new ColorAnimation
            {
                To = Colors.Transparent,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            if (bd.Background is SolidColorBrush brush)
            {
                if (brush.IsFrozen)
                {
                    brush = new SolidColorBrush(brush.Color);
                    bd.Background = brush;
                }
                brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }

            if (actionButtons != null)
            {
                var fadeAnim = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(350),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                };
                actionButtons.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }
        }

        private void ApplyDotColor(TreeViewItem treeItem, int level)
        {
            if (!_dotCache.TryGetValue(treeItem, out var dot))
            {
                dot = FindChild<Border>(treeItem, "Dot");
                if (dot != null)
                    _dotCache[treeItem] = dot;
            }

            if (dot == null) return;

            int colorIndex = Math.Min(Math.Max(level - 1, 0), DotColors.Length - 1);
            dot.Background = new SolidColorBrush(DotColors[colorIndex]);
        }

        // ==================== Popup Animations ====================

        private void MainBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAnimating) return;
            TogglePopup();
        }

        private void TogglePopup()
        {
            if (_isOpen) ClosePopup();
            else OpenPopup();
        }

        private void OpenPopup()
        {
            if (_isAnimating) return;
            _isAnimating = true;
            _isOpen = true;

            SearchPopup.IsOpen = true;
            AnimatePopupIn();
            RotateArrow(180);
            SearchBox.Focus();
            SearchBox.Text = "";
            BuildTree(_allCategories);
        }

        private void ClosePopup()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            AnimatePopupOut(() =>
            {
                _isOpen = false;
                SearchPopup.IsOpen = false;
                RotateArrow(0);
                _isAnimating = false;
            });
        }

        private void AnimatePopupIn()
        {
            var container = PopupContainer;
            if (container == null) { _isAnimating = false; return; }

            container.Opacity = 0;
            var slide = (TranslateTransform)container.RenderTransform;
            slide.Y = -18;

            // fade in آرام‌تر
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var slideIn = new DoubleAnimation
            {
                From = -12,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(320),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            fadeIn.Completed += (s, e) => _isAnimating = false;

            container.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            slide.BeginAnimation(TranslateTransform.YProperty, slideIn);
        }
        public static readonly DependencyProperty HeaderTextProperty =
    DependencyProperty.Register(
        nameof(HeaderText),
        typeof(string),
        typeof(CategorySearchControl),
        new PropertyMetadata("دسته بندی اشخاص"));
        public event Action<CategoryItem> RootCategoryAdded;

        private void AddRootButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog("افزودن دسته اصلی", "نام دسته اصلی جدید:");
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
            {
                var newRoot = new CategoryItem
                {
                    Title = dialog.InputText.Trim(),
                    Level = 0
                };

                _allCategories.Add(newRoot);
                BuildTree(_allCategories);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ExpandAllItems(CategoryTree);
                }), DispatcherPriority.Loaded);
            }
        }
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }
        private void AnimatePopupOut(Action onComplete)
        {
            var container = PopupContainer;
            if (container == null) { onComplete?.Invoke(); return; }

            var slide = (TranslateTransform)container.RenderTransform;

            // fade out آرام‌تر
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(420),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            // slide out آرام‌تر
            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = -12,
                Duration = TimeSpan.FromMilliseconds(420),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) => onComplete?.Invoke();

            container.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            slide.BeginAnimation(TranslateTransform.YProperty, slideOut);
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            _isOpen = false;
            _isAnimating = false;
            RotateArrow(0);
        }

        private void RotateArrow(double angle)
        {
            var animation = new DoubleAnimation
            {
                To = angle,
                Duration = TimeSpan.FromMilliseconds(260),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            ArrowRotation.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        // ==================== Search ====================

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            string searchText = SearchBox.Text.Trim();
            if (_lastSearchText == searchText) return;
            _lastSearchText = searchText;

            if (string.IsNullOrEmpty(searchText))
                BuildTree(_allCategories);
            else
                PerformSearch(searchText);
        }

        private void PerformSearch(string searchText)
        {
            var filtered = FilterCategories(_allCategories, searchText);
            BuildTree(filtered);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ExpandAllItems(CategoryTree);
            }), DispatcherPriority.Loaded);
        }

        private List<CategoryItem> FilterCategories(List<CategoryItem> items, string searchText)
        {
            var result = new List<CategoryItem>();
            searchText = searchText.Trim().ToLowerInvariant();

            foreach (var item in items)
            {
                var selfMatch = (item.Title ?? "").ToLowerInvariant().Contains(searchText);
                var filteredChildren = item.HasChildren
                    ? FilterCategories(item.Children.ToList(), searchText)
                    : new List<CategoryItem>();

                if (selfMatch)
                {
                    result.Add(CloneCategoryTree(item, item.Level, true));
                }
                else if (filteredChildren.Any())
                {
                    var clone = new CategoryItem
                    {
                        Title = item.Title,
                        IconPath = item.IconPath,
                        Level = item.Level
                    };

                    foreach (var child in filteredChildren)
                        clone.Children.Add(child);

                    result.Add(clone);
                }
            }

            return result;
        }

        private CategoryItem CloneCategoryTree(CategoryItem source, int level, bool includeAllChildren)
        {
            var clone = new CategoryItem
            {
                Title = source.Title,
                IconPath = source.IconPath,
                Level = level
            };

            if (includeAllChildren && source.HasChildren)
            {
                foreach (var child in source.Children)
                    clone.Children.Add(CloneCategoryTree(child, level + 1, true));
            }

            return clone;
        }

        private void ExpandAllItems(ItemsControl parent)
        {
            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    treeItem.IsExpanded = true;
                    ExpandAllItems(treeItem);
                }
            }
        }

        // ==================== Item Operations ====================

        private void AddButton_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var treeItem = FindParentTreeViewItem(border);

            if (treeItem?.Tag is CategoryItem category)
            {
                ItemAdded?.Invoke(category);
                var dialog = new InputDialog("افزودن زیرمجموعه", $"نام زیرمجموعه برای {category.Title}:");
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
                {
                    var newItem = new CategoryItem { Title = dialog.InputText, Level = category.Level + 1 };
                    category.Children.Add(newItem);
                    BuildTree(_allCategories);
                    treeItem.IsExpanded = true;
                }
            }
            e.Handled = true;
        }

        private void EditButton_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var treeItem = FindParentTreeViewItem(border);

            if (treeItem?.Tag is CategoryItem category)
            {
                ItemEdited?.Invoke(category);
                var dialog = new InputDialog("ویرایش", $"ویرایش {category.Title}:", category.Title);
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.InputText))
                {
                    category.Title = dialog.InputText;
                    treeItem.Header = dialog.InputText;
                    BuildTree(_allCategories);
                }
            }
            e.Handled = true;
        }

        private void DeleteButton_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var treeItem = FindParentTreeViewItem(border);

            if (treeItem?.Tag is CategoryItem category)
            {
                var result = MessageBox.Show($"آیا از حذف '{category.Title}' اطمینان دارید؟",
                    "تأیید حذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ItemDeleted?.Invoke(category);
                    RemoveCategory(_allCategories, category);
                    BuildTree(_allCategories);
                }
            }
            e.Handled = true;
        }

        private bool RemoveCategory(List<CategoryItem> items, CategoryItem target)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == target)
                {
                    items.RemoveAt(i);
                    return true;
                }
                if (items[i].HasChildren && RemoveCategory(items[i].Children.ToList(), target))
                    return true;
            }
            return false;
        }

        // ==================== Selection ====================

        private void CategoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem && selectedItem.Tag is CategoryItem category)
            {
                SelectedText.Text = category.Title;
                SelectedText.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#0D2159"));
                ClosePopup();
                CategorySelected?.Invoke(category);
            }
        }

        // ==================== Public API ====================

        public void SetSelectedCategory(string title)
        {
            SelectedText.Text = title;
            SelectedText.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#0D2159"));
        }

        public void SetCategories(List<CategoryItem> categories)
        {
            _allCategories = categories;
            BuildTree(_allCategories);
        }

        public void ClearSelection()
        {
            SelectedText.Text = "یک دسته جستجو کنید...";
            SelectedText.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#737791"));
        }

        // ==================== Helper Methods ====================

        private TreeViewItem FindParentTreeViewItem(DependencyObject child)
        {
            while (child != null)
            {
                if (child is TreeViewItem item)
                    return item;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && child is FrameworkElement fe && fe.Name == childName)
                    return typedChild;
                var result = FindChild<T>(child, childName);
                if (result != null) return result;
            }
            return null;
        }

        public void Dispose()
        {
            _searchTimer?.Stop();
            _searchTimer = null;
            _allCategories?.Clear();
            _dotCache?.Clear();
        }

        // ==================== Nested Classes ====================

        public class CategoryItem
        {
            public string Title { get; set; }
            public string IconPath { get; set; }
            public ObservableCollection<CategoryItem> Children { get; set; }
            public int Level { get; set; }

            public bool HasChildren => Children != null && Children.Count > 0;

            public CategoryItem()
            {
                Children = new ObservableCollection<CategoryItem>();
            }
        }

        public class InputDialog : Window
        {
            public string InputText { get; private set; }

            public InputDialog(string title, string prompt, string defaultValue = "")
            {
                Title = title;
                Width = 350;
                Height = 180;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                FlowDirection = FlowDirection.RightToLeft;

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Margin = new Thickness(15);

                var promptText = new TextBlock
                {
                    Text = prompt,
                    FontFamily = (FontFamily)FindResource("IRANSans"),
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                Grid.SetRow(promptText, 0);

                var textBox = new TextBox
                {
                    Text = defaultValue,
                    FontFamily = (FontFamily)FindResource("IRANSans"),
                    FontSize = 13,
                    Height = 30,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                Grid.SetRow(textBox, 1);

                var buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var okButton = new Button
                {
                    Content = "تأیید",
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontFamily = (FontFamily)FindResource("IRANSans")
                };
                okButton.Click += (s, e) => { InputText = textBox.Text; DialogResult = true; };

                var cancelButton = new Button
                {
                    Content = "انصراف",
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(5),
                    FontFamily = (FontFamily)FindResource("IRANSans")
                };
                cancelButton.Click += (s, e) => { DialogResult = false; };

                buttonPanel.Children.Add(okButton);
                buttonPanel.Children.Add(cancelButton);
                Grid.SetRow(buttonPanel, 2);

                grid.Children.Add(promptText);
                grid.Children.Add(textBox);
                grid.Children.Add(buttonPanel);
                Content = grid;
            }
        }
    }
}