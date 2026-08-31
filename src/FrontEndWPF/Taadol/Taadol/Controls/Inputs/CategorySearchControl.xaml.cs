using _0_Framework.Application;
using Microsoft.Extensions.DependencyInjection;
using PayrollSystemManagement.Application.Contracts.Department;
using PayrollSystemManagement.Application.Contracts.JobTitle;
using PersonManagement.Application.Contract.PersonCategory;
using Taadol.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private int _visibleItemCount;
        private string _visibleSearchText = "";
        private bool _isLoadingNextPage;
        private int _lazyRenderVersion;
        private List<CategoryItem> _visibleSourceItems = new();
        private Dictionary<TreeViewItem, Border> _dotCache = new Dictionary<TreeViewItem, Border>();
        private CancellationTokenSource _operationCts = new();
        private int _operationVersion;

        // Events
        public event Action<CategoryItem> CategorySelected;
        public event Action SelectionCleared;
        public event Action<CategoryItem> ItemAdded;
        public event Action<CategoryItem> ItemEdited;
        public event Action<CategoryItem> ItemDeleted;
        public event Action DataChanged;

        /// <summary>
        /// شناسه نوع شخصی که دسته‌بندی‌ها به آن تعلق دارند.
        /// این مقدار باید قبل از عملیات Add/Edit/Delete توسط View والد ست شود.
        /// </summary>
        public long PersonTypeId { get; set; }

        /// <summary>
        /// نوع منبع داده کنترل در حالت SearchOnDemand (دپارتمان یا عنوان شغلی).
        /// فقط برای کنترل‌های SearchOnDemand معنا دارد.
        /// </summary>
        public enum SearchSourceKind { None, Department, JobTitle }

        public static readonly DependencyProperty SourceKindProperty =
            DependencyProperty.Register(
                nameof(SourceKind),
                typeof(SearchSourceKind),
                typeof(CategorySearchControl),
                new PropertyMetadata(SearchSourceKind.None));

        /// <summary>نوع منبع داده کنترل در حالت SearchOnDemand.</summary>
        public SearchSourceKind SourceKind
        {
            get => (SearchSourceKind)GetValue(SourceKindProperty);
            set => SetValue(SourceKindProperty, value);
        }

        /// <summary>
        /// نام دپارتمان انتخاب‌شده برای فیلتر عنوان‌های شغلی در سمت Frontend.
        /// </summary>
        public string DepartmentFilterName { get; set; }

        /// <summary>
        /// شناسه دپارتمان انتخاب‌شده برای ایجاد عنوان شغلی.
        /// </summary>
        public long DepartmentId { get; set; }
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
        }

        public CategorySearchControl()
        {
            InitializeComponent();

            VirtualizingPanel.SetIsVirtualizing(CategoryTree, true);
            VirtualizingPanel.SetVirtualizationMode(CategoryTree, VirtualizationMode.Recycling);

            _searchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _searchTimer.Tick += SearchTimer_Tick;

            Unloaded += CategorySearchControl_Unloaded;
        }

        private void CategorySearchControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CancelAndDisposeOperation();
            Dispose();
        }

        private CancellationToken BeginOperation(out int version)
        {
            var next = new CancellationTokenSource();
            var previous = Interlocked.Exchange(ref _operationCts, next);
            Interlocked.Increment(ref _operationVersion);
            try { previous?.Cancel(); } catch (ObjectDisposedException) { }
            previous?.Dispose();
            version = Volatile.Read(ref _operationVersion);
            return next.Token;
        }

        private void CancelAndDisposeOperation()
        {
            Interlocked.Increment(ref _operationVersion);
            var current = Interlocked.Exchange(ref _operationCts, null);
            if (current == null) return;
            try { current.Cancel(); } catch (ObjectDisposedException) { }
            current.Dispose();
        }
        // این متد رو حذف کن:
        // private void LoadSampleData() { ... }

        // به جاش این متد رو اضافه کن:
        /// <summary>
        /// بارگذاری درخت دسته‌بندی از بک‌اند (PersonCategoryTreeViewModel)
        /// </summary>
        public void LoadFromTreeDto(List<PersonCategoryTreeViewModel> tree, int level = 0)
        {
            // ★ جلوگیری از NullReferenceException اگه tree برابر null باشه
            if (tree == null)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ LoadFromTreeDto: tree is null");
                _allCategories = new List<CategoryItem>();
                BuildTree(_allCategories);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"📊 LoadFromTreeDto: loading {tree.Count} root categories");

            _allCategories = new List<CategoryItem>();
            int totalChildren = 0;

            foreach (var node in tree)
            {
                if (node == null) continue;

                var item = new CategoryItem
                {
                    Id = node.Id,
                    Title = node.Title ?? "(بدون عنوان)",
                    Level = level
                };

                if (node.Children != null && node.Children.Count > 0)
                {
                    totalChildren += node.Children.Count;
                    AddChildrenRecursive(item, node.Children, level + 1);
                }

                _allCategories.Add(item);
            }

            System.Diagnostics.Debug.WriteLine($"✅ LoadFromTreeDto: built {_allCategories.Count} root(s), total children in tree = {totalChildren}, _allCategories first root children = {(_allCategories.Count > 0 && _allCategories[0].HasChildren ? _allCategories[0].Children.Count : 0)}");

            // در حالت SearchOnDemand درخت کامل ساخته نمی‌شود؛ فقط پیش‌نمایش محدود + جستجو.
            ResetLazyItems(SearchOnDemand ? BuildSearchOnDemandSource() : _allCategories);
        }

        /// <summary>
        /// پیش‌نمایش سبک برای حالت SearchOnDemand:
        /// فرزندانِ ریشه (دپارتمان‌ها/عناوین شغلی) به‌صورت سطح‌اول و با سقف MaxSearchResults
        /// نمایش داده می‌شوند تا UI فریز نشود ولی لیست خالی به نظر نرسد.
        /// </summary>
        private List<CategoryItem> BuildSearchOnDemandSource()
        {
            var result = new List<CategoryItem>();
            if (_allCategories == null) return result;

            foreach (var root in _allCategories)
            {
                if (root == null) continue;
                if (root.HasChildren)
                    result.AddRange(root.Children.Where(child => child != null));
                else if (root.Id != 0 || !string.IsNullOrWhiteSpace(root.Title))
                    result.Add(root);
            }
            return result;
        }

        private void ResetLazyItems(List<CategoryItem> source, string searchText = "")
        {
            _lazyRenderVersion++;
            _isLoadingNextPage = false;
            SetLazyLoadingVisualState(false);

            _visibleSourceItems = source ?? new List<CategoryItem>();
            _visibleSearchText = searchText ?? "";
            _visibleItemCount = Math.Min(LazyPageSize, _visibleSourceItems.Count);
            BuildLazyItems();
        }

        private void LoadNextLazyPage()
        {
            if (!SearchOnDemand || _isLoadingNextPage || _visibleItemCount >= _visibleSourceItems.Count)
                return;

            _isLoadingNextPage = true;
            SetLazyLoadingVisualState(true);

            var renderVersion = _lazyRenderVersion;
            var startIndex = _visibleItemCount;
            var endIndex = Math.Min(_visibleItemCount + LazyPageSize, _visibleSourceItems.Count);
            _visibleItemCount = endIndex;

            // صفحه‌ی جدید در batchهای کوچک اضافه می‌شود تا Render فرصت اجرای Shimmer داشته باشد.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AppendLazyItemsInBatches(startIndex, endIndex, renderVersion);
            }), DispatcherPriority.ContextIdle);
        }

        private void AppendLazyItemsInBatches(int startIndex, int endIndex, int renderVersion)
        {
            if (renderVersion != _lazyRenderVersion)
                return;

            try
            {
                var batchEnd = Math.Min(startIndex + LazyBatchSize, endIndex);
                var batch = _visibleSourceItems
                    .Skip(startIndex)
                    .Take(batchEnd - startIndex)
                    .Select(item => CloneCategoryTree(item, 0, true))
                    .ToList();

                // فقط آیتم‌های جدید اضافه می‌شوند؛ آیتم‌های قبلی دوباره ساخته نمی‌شوند.
                BuildTree(batch, null, clearRoot: false);

                if (batchEnd < endIndex)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AppendLazyItemsInBatches(batchEnd, endIndex, renderVersion);
                    }), DispatcherPriority.Background);
                }
                else
                {
                    _isLoadingNextPage = false;
                    SetLazyLoadingVisualState(false);
                }
            }
            catch
            {
                if (renderVersion == _lazyRenderVersion)
                {
                    _isLoadingNextPage = false;
                    SetLazyLoadingVisualState(false);
                }
                throw;
            }
        }

        private void SetLazyLoadingVisualState(bool isLoading)
        {
            if (LazyLoadingPanel == null || LazyLoadingHighlight == null) return;

            if (isLoading)
            {
                LazyLoadingPanel.Visibility = Visibility.Visible;
                LazyLoadingHighlightTransform.X = -LazyLoadingHighlight.Width;

                var shimmer = new DoubleAnimation
                {
                    From = -LazyLoadingHighlight.Width,
                    To = LazyLoadingPanel.ActualWidth > 0
                        ? LazyLoadingPanel.ActualWidth
                        : 900,
                    Duration = TimeSpan.FromSeconds(0.9),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                LazyLoadingHighlightTransform.BeginAnimation(
                    TranslateTransform.XProperty,
                    shimmer,
                    HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                LazyLoadingHighlightTransform.BeginAnimation(
                    TranslateTransform.XProperty,
                    null);
                LazyLoadingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void BuildLazyItems()
        {
            var items = _visibleSourceItems
                .Take(_visibleItemCount)
                .Select(item => CloneCategoryTree(item, 0, true))
                .ToList();
            BuildTree(items);
        }

        private void AddChildrenRecursive(CategoryItem parent, List<PersonCategoryTreeViewModel> children, int level)
        {
            if (children == null) return;

            foreach (var child in children)
            {
                if (child == null) continue;

                var item = new CategoryItem
                {
                    Id = child.Id,
                    Title = child.Title ?? "(بدون عنوان)",
                    Level = level
                };

                if (child.Children != null && child.Children.Count > 0)
                    AddChildrenRecursive(item, child.Children, level + 1);

                parent.Children.Add(item);
            }
        }

        private void BuildTree(List<CategoryItem> items, TreeViewItem parent = null, bool clearRoot = true)
        {
            // ★ جلوگیری از NullReferenceException
            if (items == null) return;
            if (CategoryTree == null) return;

            if (parent == null && clearRoot)
            {
                CategoryTree.Items.Clear();
                _dotCache.Clear();
            }

            foreach (var item in items)
            {
                if (item == null) continue;

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

                if (item.HasChildren && item.Children != null)
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

            // در حالت SearchOnDemand فقط پیش‌نمایش محدود رندر می‌شود تا ۵۰۰+ آیتم یکجا ساخته نشوند.
            ResetLazyItems(SearchOnDemand ? BuildSearchOnDemandSource() : _allCategories);
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

        private void AddButtonBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddRootButton_Click(sender, e);
        }

        private async void AddRootButton_Click(object sender, RoutedEventArgs e)
        {
            // حالت دپارتمان/عنوان شغلی: مستقیم از سرویس Payroll اضافه می‌شود
            if (SearchOnDemand)
            {
                await AddPayrollItemAsync();
                return;
            }

            if (PersonTypeId <= 0)
            {
                ToastManager.Warning("برای افزودن دسته‌بندی، ابتدا باید نوع شخص را انتخاب کنید.");
                return;
            }

            var title = ModernDialog.ShowInput(
                "افزودن دسته اصلی",
                "نام دسته اصلی جدید را وارد کنید:",
                "",
                ModernDialog.DialogType.Primary,
                "افزودن",
                "انصراف",
                Window.GetWindow(this));

            if (string.IsNullOrWhiteSpace(title)) return;
            title = title.Trim();

            var token = BeginOperation(out var version);
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                    var operationResult = app.Create(new CreatePersonCategory
                    {
                        Title = title,
                        PersonTypeId = PersonTypeId,
                        ParentId = null
                    });
                    token.ThrowIfCancellationRequested();
                    return operationResult;
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;

                if (result.IsSucceeded)
                {
                    if (version != Volatile.Read(ref _operationVersion)) return;
                    await RefreshTreeAsync();
                    DataChanged?.Invoke();
                    ToastManager.Success($"دسته «{title}» با موفقیت اضافه شد.");
                }
                else
                {
                    ToastManager.Error(result.Message ?? "افزودن دسته با خطا مواجه شد.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategorySearchControl] Category operation error: {ex}");
                ToastManager.Error("خطا در عملیات دسته‌بندی");
            }
        }
        /// <summary>
        /// افزودن مستقیم دپارتمان/عنوان شغلی از سرویس‌های Payroll (حالت SearchOnDemand).
        /// </summary>
        private async Task AddPayrollItemAsync()
        {
            var title = ModernDialog.ShowInput(
                SourceKind == SearchSourceKind.JobTitle ? "افزودن عنوان شغلی" : "افزودن دپارتمان",
                SourceKind == SearchSourceKind.JobTitle
                    ? "نام عنوان شغلی جدید را وارد کنید:"
                    : "نام دپارتمان جدید را وارد کنید:",
                "",
                ModernDialog.DialogType.Primary,
                "افزودن",
                "انصراف",
                Window.GetWindow(this));

            if (string.IsNullOrWhiteSpace(title)) return;
            title = title.Trim();            var token = BeginOperation(out var version);
            // DependencyProperty به UI thread وابسته است؛ قبل از Task.Run کپی می‌کنیم.
            var sourceKind = SourceKind;
            var departmentId = DepartmentId;
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();

                    if (sourceKind == SearchSourceKind.JobTitle)
                    {
                        if (departmentId <= 0)
                            return new OperationResult().Failed("ابتدا دپارتمان را انتخاب کنید.");

                        var app = scope.ServiceProvider.GetRequiredService<IJobTitleApplication>();
                        return app.Create(new CreateJobTitle { Title = title, DepartmentId = departmentId });
                    }
                    else
                    {
                        var app = scope.ServiceProvider.GetRequiredService<IDepartmentApplication>();
                        return app.Create(new CreateDepartment { Name = title });
                    }
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;

                if (result.IsSucceeded)
                {
                    await RefreshPayrollTreeAsync();
                    DataChanged?.Invoke();
                    ToastManager.Success($"«{title}» با موفقیت اضافه شد.");
                }
                else
                {
                    ToastManager.Error(result.Message ?? "افزودن با خطا مواجه شد.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategorySearchControl] Payroll add error: {ex}");
                ToastManager.Error("خطا در افزودن");
            }
        }

        /// <summary>
        /// رفرش درخت در حالت SearchOnDemand از سرویس Payroll مربوطه.
        /// </summary>
        public async Task RefreshPayrollTreeAsync()
        {            var token = BeginOperation(out var version);
            // DependencyProperty به UI thread وابسته است؛ قبل از Task.Run کپی می‌کنیم.
            var sourceKind = SourceKind;
            var departmentFilterName = DepartmentFilterName;
            List<PersonCategoryTreeViewModel> tree;
            try
            {
                tree = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();

                    if (sourceKind == SearchSourceKind.JobTitle)
                    {
                        var app = scope.ServiceProvider.GetRequiredService<IJobTitleApplication>();
                        var jobTitles = app.GetJobTitles()?.Where(j => j.IsActive).ToList();
                        if (!string.IsNullOrWhiteSpace(departmentFilterName))
                            jobTitles = jobTitles?.Where(j => j.DepartmentName == departmentFilterName).ToList();
                        return PersonFormHelper.BuildJobTitleTree(jobTitles);
                    }
                    else
                    {
                        var app = scope.ServiceProvider.GetRequiredService<IDepartmentApplication>();
                        return PersonFormHelper.BuildDepartmentTree(
                            app.GetDepartments()?.Where(d => d.IsActive).ToList());
        }
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RefreshPayrollTreeAsync failed: " + ex.Message);
                return;
            }

            if (tree == null) return;

            LoadFromTreeDto(tree);
        }

        public async Task RefreshTreeAsync(long? expandToId = null)
        {
            if (PersonTypeId <= 0) return;

            var token = BeginOperation(out var version);
            List<PersonCategoryTreeViewModel> tree = null;
            try
            {
                tree = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                    return app.GetTree(PersonTypeId);
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RefreshTreeAsync failed: " + ex.Message);
                return;
            }

            if (tree == null) return;

            if (token.IsCancellationRequested || version != Volatile.Read(ref _operationVersion)) return;
            LoadFromTreeDto(tree);

            if (expandToId.HasValue)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ExpandToId(CategoryTree, expandToId.Value);
                }), DispatcherPriority.Loaded);
            }
        }

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(
                nameof(IsRequired),
                typeof(bool),
                typeof(CategorySearchControl),
                new PropertyMetadata(false));

        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        /// <summary>
        /// وقتی true باشد، درخت خالی/سبک باز می‌شود و آیتم‌ها فقط بر اساس جستجو ساخته می‌شوند
        /// (با سقف <see cref="MaxSearchResults"/>). برای لیست‌های بزرگ مثل دپارتمان/عنوان شغلی
        /// (صدها رکورد) استفاده می‌شود تا از فریز UI روی کلیک جلوگیری شود.
        /// وقتی false باشد (پیش‌فرض)، رفتار فعلی یعنی نمایش کامل درخت حفظ می‌شود.
        /// </summary>
        public static readonly DependencyProperty SearchOnDemandProperty =
            DependencyProperty.Register(
                nameof(SearchOnDemand),
                typeof(bool),
                typeof(CategorySearchControl),
                new PropertyMetadata(false));

        public bool SearchOnDemand
        {
            get => (bool)GetValue(SearchOnDemandProperty);
            set => SetValue(SearchOnDemandProperty, value);
        }

        /// <summary>سقف تعداد نتایج در حالت SearchOnDemand.</summary>
        public const int MaxSearchResults = 60;
        private const int LazyPageSize = 20;
        private const int LazyBatchSize = 4;

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

            if (SearchOnDemand)
            {
                var source = string.IsNullOrEmpty(searchText)
                    ? BuildSearchOnDemandSource()
                    : FilterCategoriesCapped(BuildSearchOnDemandSource(), searchText, MaxSearchResults);
                ResetLazyItems(source, searchText);
            }
            else if (string.IsNullOrEmpty(searchText))
                BuildTree(_allCategories);
            else
                PerformSearch(searchText);
        }

        private void PerformSearch(string searchText)
        {
            var filtered = SearchOnDemand
                ? FilterCategoriesCapped(_allCategories, searchText, MaxSearchResults)
                : FilterCategories(_allCategories, searchText);
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

        /// <summary>
        /// نسخه‌ای از فیلتر که برای حالت SearchOnDemand استفاده می‌شود:
        /// فقط آیتم‌های تکی (برگ‌های درخت) که عنوانشان شامل عبارت باشد را برمی‌گرداند
        /// و حداکثر <paramref name="maxResults"/> نتیجه را برمی‌گرداند تا درخت سنگین نشود.
        /// </summary>
        private List<CategoryItem> FilterCategoriesCapped(List<CategoryItem> items, string searchText, int maxResults)
        {
            var result = new List<CategoryItem>();
            searchText = searchText.Trim().ToLowerInvariant();

            foreach (var item in items)
            {
                if (result.Count >= maxResults) break;
                if (item == null) continue;

                var excludeRoot =
                    (item.Children == null || item.Children.Count == 0) &&
                    ((item.Title ?? "").Trim().Length == 0 || item.Id == 0);

                if (!excludeRoot && (item.Title ?? "").ToLowerInvariant().Contains(searchText))
                {
                    var clone = CloneCategoryTree(item, item.Level, true);
                    result.Add(clone);
                    if (result.Count >= maxResults) break;
                }

                if (item.HasChildren)
                {
                    foreach (var child in FilterCategoriesCapped(item.Children.ToList(), searchText, maxResults - result.Count))
                        result.Add(child);
                }
            }

            return result;
        }

        private CategoryItem CloneCategoryTree(CategoryItem source, int level, bool includeAllChildren)
        {
            var clone = new CategoryItem
            {
                Id = source.Id,
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

        private async void EditButton_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var border = sender as Border;
            var treeItem = FindParentTreeViewItem(border);
            if (treeItem?.Tag is not CategoryItem category) return;

            var newTitle = ModernDialog.ShowInput(
                SearchOnDemand ? "ویرایش" : "ویرایش دسته",
                $"ویرایش «{category.Title}»:",
                category.Title,
                ModernDialog.DialogType.Success,
                "ذخیره",
                "انصراف",
                Window.GetWindow(this));

            if (string.IsNullOrWhiteSpace(newTitle)) return;
            newTitle = newTitle.Trim();
            if (newTitle == category.Title) return;

            var token = BeginOperation(out var version);
            // DependencyProperty به UI thread وابسته است؛ قبل از Task.Run کپی می‌کنیم.
            var sourceKind = SourceKind;
            var departmentId = DepartmentId;
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();

                    if (sourceKind == SearchSourceKind.Department)
                    {
                        var app = scope.ServiceProvider.GetRequiredService<IDepartmentApplication>();
                        var details = app.GetDetails(category.Id);
                        if (details == null) return new OperationResult().Failed("رکورد یافت نشد.");
                        return app.Edit(new EditDepartment { Id = category.Id, Name = newTitle, Description = details.Description });
                    }
                    else if (sourceKind == SearchSourceKind.JobTitle)
                    {
                        var app = scope.ServiceProvider.GetRequiredService<IJobTitleApplication>();
                        var details = app.GetDetails(category.Id);
                        if (details == null) return new OperationResult().Failed("رکورد یافت نشد.");
                        return app.Edit(new EditJobTitle { Id = category.Id, Title = newTitle, Description = details.Description, DepartmentId = details.DepartmentId });
                    }
                    else
                    {
                        var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                        var details = app.GetDetails(category.Id);
                        if (details == null) return new OperationResult().Failed("رکورد یافت نشد.");
                        return app.Edit(new EditPersonCategory { Id = category.Id, Title = newTitle, PersonTypeId = details.PersonTypeId, ParentId = details.ParentId });
                    }
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;

                if (result.IsSucceeded)
                {
                    category.Title = newTitle;
                    ItemEdited?.Invoke(category);
                    if (SearchOnDemand) await RefreshPayrollTreeAsync(); else await RefreshTreeAsync(expandToId: category.Id);
                    DataChanged?.Invoke();
                    ModernDialog.ShowConfirm("ذخیره شد", $"تغییر با موفقیت اعمال شد.", ModernDialog.DialogType.Success, "عالی", "بستن", Window.GetWindow(this));
                }
                else
                {
                    ModernDialog.ShowConfirm("خطا در ویرایش", result.Message ?? "ویرایش با خطا مواجه شد.", ModernDialog.DialogType.Warning, "متوجه شدم", "بستن", Window.GetWindow(this));
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategorySearchControl] Category operation error: {ex}");
                ToastManager.Error("خطا در عملیات دسته‌بندی");
            }
        }
        private async void DeleteButton_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var border = sender as Border;
            var treeItem = FindParentTreeViewItem(border);
            if (treeItem?.Tag is not CategoryItem category) return;

            bool confirm = ModernDialog.ShowConfirm(
                "حذف",
                $"آیا از حذف «{category.Title}» اطمینان دارید؟ این عملیات قابل بازگشت نیست.",
                ModernDialog.DialogType.Danger,
                "حذف",
                "انصراف",
                Window.GetWindow(this));

            if (!confirm) return;

            var token = BeginOperation(out var version);
            // DependencyProperty به UI thread وابسته است؛ قبل از Task.Run کپی می‌کنیم.
            var sourceKind = SourceKind;
            var departmentId = DepartmentId;
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();

                    if (sourceKind == SearchSourceKind.Department)
                        return scope.ServiceProvider.GetRequiredService<IDepartmentApplication>().Remove(category.Id);
                    else if (sourceKind == SearchSourceKind.JobTitle)
                        return scope.ServiceProvider.GetRequiredService<IJobTitleApplication>().Remove(category.Id);
                    else
                        return scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>().Remove(category.Id);
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;

                if (result.IsSucceeded)
                {
                    ItemDeleted?.Invoke(category);
                    if (SearchOnDemand) await RefreshPayrollTreeAsync(); else await RefreshTreeAsync();
                    DataChanged?.Invoke();
                    ModernDialog.ShowConfirm("حذف شد", $"«{category.Title}» با موفقیت حذف شد.", ModernDialog.DialogType.Success, "عالی", "بستن", Window.GetWindow(this));
                }
                else
                {
                    ModernDialog.ShowConfirm(
                        "امکان حذف نیست",
                        result.Message ?? "حذف با خطا مواجه شد.",
                        ModernDialog.DialogType.Warning,
                        "متوجه شدم",
                        "بستن",
                        Window.GetWindow(this));
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategorySearchControl] Category operation error: {ex}");
                ToastManager.Error("خطا در عملیات دسته‌بندی");
            }
        }

        private async void AddButton_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var border = sender as Border;
            var treeItem = FindParentTreeViewItem(border);
            if (treeItem?.Tag is not CategoryItem parent) return;

            var title = ModernDialog.ShowInput(
                SearchOnDemand ? "افزودن" : "افزودن زیرمجموعه",
                SearchOnDemand
                    ? $"نام جدید را وارد کنید:"
                    : $"نام زیرمجموعه برای «{parent.Title}» را وارد کنید:",
                "",
                ModernDialog.DialogType.Primary,
                "افزودن",
                "انصراف",
                Window.GetWindow(this));

            if (string.IsNullOrWhiteSpace(title)) return;
            title = title.Trim();

            var token = BeginOperation(out var version);
            // DependencyProperty به UI thread وابسته است؛ قبل از Task.Run کپی می‌کنیم.
            var sourceKind = SourceKind;
            var departmentId = DepartmentId;
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();

                    if (sourceKind == SearchSourceKind.Department)
                        return scope.ServiceProvider.GetRequiredService<IDepartmentApplication>().Create(new CreateDepartment { Name = title });
                    else if (sourceKind == SearchSourceKind.JobTitle)
                    {
                        if (departmentId <= 0)
                            return new OperationResult().Failed("ابتدا دپارتمان را انتخاب کنید.");

                        return scope.ServiceProvider.GetRequiredService<IJobTitleApplication>().Create(
                            new CreateJobTitle { Title = title, DepartmentId = departmentId });
                    }
                    else
                    {
                        if (PersonTypeId <= 0) return new OperationResult().Failed("ابتدا نوع شخص را انتخاب کنید.");
                        var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                        return app.Create(new CreatePersonCategory { Title = title, PersonTypeId = PersonTypeId, ParentId = parent.Id });
                    }
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _operationVersion)) return;

                if (result.IsSucceeded)
                {
                    ItemAdded?.Invoke(parent);
                    if (SearchOnDemand) await RefreshPayrollTreeAsync(); else await RefreshTreeAsync(expandToId: parent.Id);
                    DataChanged?.Invoke();
                    ToastManager.Success($"«{title}» با موفقیت اضافه شد.");
                }
                else
                {
                    ToastManager.Error(result.Message ?? "افزودن با خطا مواجه شد.");
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategorySearchControl] Category operation error: {ex}");
                ToastManager.Error("خطا در عملیات");
            }
        }
        private bool ExpandToId(ItemsControl parent, long targetId)
        {
            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    if (treeItem.Tag is CategoryItem cat && cat.Id == targetId)
                    {
                        treeItem.IsExpanded = true;
                        treeItem.Focus();
                        treeItem.IsSelected = true;
                        return true;
                    }

                    treeItem.IsExpanded = true;
                    if (ExpandToId(treeItem, targetId))
                        return true;

                    treeItem.IsExpanded = false;
                }
            }
            return false;
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

                // ★ نگه داشتن Id دسته انتخاب‌شده
                _selectedCategoryId = category.Id;

                CategorySelected?.Invoke(category);
            }
        }

        private void CategoryScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!SearchOnDemand || e.VerticalChange <= 0) return;
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 24)
                LoadNextLazyPage();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            if (SearchOnDemand && e.Delta < 0 && scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight - 24)
                LoadNextLazyPage();

            if (e.Delta > 0)
                scrollViewer.LineUp();
            else
                scrollViewer.LineDown();

            e.Handled = true;
        }

        // ==================== Public API ====================

        public void SetSelectedCategory(string title)
        {
            SelectedText.Text = title;
            SelectedText.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#0D2159"));
        }

        /// <summary>
        /// ★ انتخاب دسته‌بندی با Id — برای لود دسته‌بندی شخص در فرم ویرایش
        /// این متد درخت دسته‌بندی‌ها رو می‌گرده و اگه دسته با Id مورد نظر پیدا کنه،
        /// اون رو به‌عنوان دسته انتخاب‌شده تنظیم می‌کنه.
        /// </summary>
        public void SelectCategoryById(long categoryId)
        {
            if (categoryId <= 0 || _allCategories == null || _allCategories.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ SelectCategoryById: categoryId={categoryId}, _allCategories is null or empty");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"🔍 SelectCategoryById: searching for categoryId={categoryId}");

            var found = FindCategoryById(_allCategories, categoryId);
            if (found != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ SelectCategoryById: found '{found.Title}'");
                SetSelectedCategory(found.Title);

                // یادآوری: در event دسته‌بندی هم اطلاع بدیم
                _selectedCategoryId = found.Id;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ SelectCategoryById: categoryId={categoryId} not found in tree");
            }
        }

        /// <summary>
        /// جستجوی دسته‌بندی با Id در درخت (به‌صورت بازگشتی)
        /// </summary>
        private CategoryItem FindCategoryById(List<CategoryItem> items, long categoryId)
        {
            if (items == null) return null;

            foreach (var item in items)
            {
                if (item == null) continue;

                if (item.Id == categoryId)
                    return item;

                if (item.HasChildren && item.Children != null)
                {
                    var found = FindCategoryById(item.Children.ToList(), categoryId);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        // ★ فیلد برای نگه داشتن Id دسته انتخاب‌شده
        private long? _selectedCategoryId;

        /// <summary>
        /// Id دسته‌بندی انتخاب‌شده (null اگه هیچی انتخاب نشده)
        /// </summary>
        public long? SelectedCategoryId => _selectedCategoryId;

        public void SetCategories(List<CategoryItem> categories)
        {
            _allCategories = categories;
            BuildTree(_allCategories);
        }

        public void ClearSelection(bool notify = true)
        {
            SelectedText.Text = "یک دسته جستجو کنید...";
            SelectedText.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#737791"));
            _selectedCategoryId = null;
            if (notify)
                SelectionCleared?.Invoke();
        }

        public string SelectedCategoryTitle => _selectedCategoryId.HasValue
            ? SelectedText.Text
            : null;

        // ==================== Helper Methods ====================

        private TreeViewItem FindParentTreeViewItem(DependencyObject child)
        {
            while (child != null)
            {
                if (child is TreeViewItem item)
                    return item;

                // کلیک روی متن گره، OriginalSource را یک Run (ContentElement) می‌کند که
                // Visual نیست؛ VisualTreeHelper.GetParent روی آن InvalidOperationException
                // می‌اندازد. برای عناصر غیر-Visual از LogicalTree بالا می‌رویم.
                child = child is Visual || child is System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(child)
                    : LogicalTreeHelper.GetParent(child);
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
            if (_searchTimer != null)
            {
                _searchTimer.Stop();
                _searchTimer.Tick -= SearchTimer_Tick;
                _searchTimer = null;
            }

            if (CategoryTree != null)
            {
                foreach (TreeViewItem item in CategoryTree.Items)
                    UnsubscribeTreeItemEvents(item);
            }

            _allCategories?.Clear();
            _dotCache?.Clear();
        }

        private void UnsubscribeTreeItemEvents(TreeViewItem treeItem)
        {
            if (treeItem == null) return;

            treeItem.Loaded -= TreeItem_Loaded;

            var bd = FindChild<Border>(treeItem, "Bd");
            if (bd != null)
            {
                bd.MouseEnter -= TreeItemBorder_MouseEnter;
                bd.MouseLeave -= TreeItemBorder_MouseLeave;
            }

            foreach (TreeViewItem child in treeItem.Items)
                UnsubscribeTreeItemEvents(child);
        }

        // ==================== Nested Classes ====================

        public class CategoryItem
        {
            public long Id { get; set; }              // ← اضافه بشه
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


    }
}