using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Application.Contract.Persons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Taadol.Controls;
using Taadol.Models;
using Taadol.ViewModels;

namespace Taadol.Views
{
    public partial class PersonListView : UserControl
    {
        public PersonListViewModel ViewModel { get; }
        private bool _isLoadedOnce = false;
        private bool _isPanelOpen = true;
        private bool _sizeWired = false;
        private bool _isUpdatingPanels = false;

        public PersonListView()
        {
            InitializeComponent();

            // نوار خلاصه به شکاف زیر گرید منتقل می‌شود تا همیشه زیر گرید بچسبد
            // (اول از والد فعلی جدا می‌شود تا خطای «Must disconnect child» رخ ندهد)
            if (SummaryBorder.Parent is Panel parent)
                parent.Children.Remove(SummaryBorder);
            PersonsGrid.Footer = SummaryBorder;

            using var scope = App.ServiceProvider.CreateScope();
            ViewModel = new PersonListViewModel(App.ServiceProvider);

            DataContext = ViewModel;
            UpdateEmptyStateMessage();

            // جستجو با Debounce داخلی SearchBoxControl (پیش‌فرض ۳۰۰ms)
            SearchBox.SearchTextChanged += (s, text) =>
            {
                ViewModel.HandleSearchTextChanged(text);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    PersonsGrid.RefreshVisualState();
                }), DispatcherPriority.Loaded);
            };

            PersonsGrid.NextPageRequested += (s, e) => ViewModel.GoToNextPage();
            PersonsGrid.PreviousPageRequested += (s, e) => ViewModel.GoToPreviousPage();
            PersonsGrid.PageRequested += (s, page) => ViewModel.GoToPage(page);
            PersonsGrid.PageSizeRequested += (s, size) => ViewModel.ChangePageSize(size);

            PersonsGrid.GridDoubleClicked += (s, e) =>
            {
                // دسترسی به ویرایش شخص فقط از طریق دکمه داخل پنل جزئیات رخ می‌دهد،
                // نه با دابل‌کلیک روی ردیف گرید.
            };

            PersonsGrid.CheckedItemsChanged += (s, e) =>
            {
                UpdateDetailPanels();
                ViewModel.UpdateSummaryBar();
            };

            // انتخاب همه = فقط همین صفحه؛ خاموش‌کردن هدر = پاک کردن انتخاب کل لیست
            PersonsGrid.SelectAllToggled += (s, select) =>
            {
                if (select || ViewModel.AllPersons == null) return;
                foreach (var item in ViewModel.AllPersons.Where(p => !p.IsEmpty))
                    item.IsSelected = false;
            };

            // منوی راست‌کلیک ردیف: ویرایش و حذف (حذف تک‌مورد مثل پنل جزئیات)
            PersonsGrid.RowEditRequested += (s, item) =>
            {
                if (item is PersonItem p && Window.GetWindow(this) is MainWindow mw)
                    mw.NavigateToEditPerson(p.Id);
            };

            PersonsGrid.RowDeleteRequested += (s, item) =>
            {
                if (item is PersonItem p)
                    DetailPanel_DeleteRequested(s, p.Id);
            };

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            Loaded += PersonListView_Loaded;
            this.Unloaded += PersonListView_Unloaded;
        }

        private void PersonListView_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelPendingLoads();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            this.Unloaded -= PersonListView_Unloaded;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonListViewModel.TotalDebitText))
            {
                if (TotalDebitText != null)
                    TotalDebitText.Text = ViewModel.TotalDebitText;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.TotalCreditText))
            {
                if (TotalCreditText != null)
                    TotalCreditText.Text = ViewModel.TotalCreditText;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.SelectedSummaryText))
            {
                if (SelectedSummaryText != null)
                    SelectedSummaryText.Text = ViewModel.SelectedSummaryText;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.SelectedTotalText))
            {
                if (SelectedTotalText != null)
                {
                    SelectedTotalText.Text = ViewModel.SelectedTotalText;
                    try
                    {
                        var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ViewModel.SelectedTotalTextColor);
                        SelectedTotalText.Foreground = new System.Windows.Media.SolidColorBrush(color);
                    }
                    catch
                    {
                        SelectedTotalText.Foreground = System.Windows.Media.Brushes.Black;
                    }
                }
            }
            else if (e.PropertyName == nameof(PersonListViewModel.SelectedCountText))
            {
                if (SelectedCountText != null)
                    SelectedCountText.Text = ViewModel.SelectedCountText;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.TabAllCount))
            {
                tabAll.Tag = ViewModel.TabAllCount;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.TabCustomersCount))
            {
                tabCustomers.Tag = ViewModel.TabCustomersCount;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.TabSuppliersCount))
            {
                tabSuppliers.Tag = ViewModel.TabSuppliersCount;
            }
            else if (e.PropertyName == nameof(PersonListViewModel.TabPersonnelCount))
            {
                tabPersonnel.Tag = ViewModel.TabPersonnelCount;
            }

            // پیام Empty State را با وضعیت فیلتر/جستجو هماهنگ کن
            UpdateEmptyStateMessage();
        }

        /// <summary>
        /// وقتی هیچ شخصی نمایش داده نمی‌شود، پیام وسط گرید بر اساس وضعیت فیلتر انتخاب می‌شود:
        /// «هنوز شخصی ثبت نشده» برای حالت بدون داده، «موردی مطابق فیلتر پیدا نشد» برای فیلتر فعال.
        /// </summary>
        private void UpdateEmptyStateMessage()
        {
            if (PersonsGrid == null) return;

            bool filterActive =
                !string.IsNullOrWhiteSpace(ViewModel.SearchText) ||
                ViewModel.SelectedStatuses.Count > 0 ||
                ViewModel.SelectedProvinces.Count > 0 ||
                ViewModel.SelectedCities.Count > 0 ||
                ViewModel.SelectedLegalStatuses.Count > 0 ||
                ViewModel.SelectedAccountStatuses.Count > 0 ||
                (ViewModel.SelectedTabs.Count > 0 && !ViewModel.SelectedTabs.Contains("all"));

            if (filterActive)
            {
                PersonsGrid.EmptyStateText = "موردی مطابق فیلتر پیدا نشد";
                PersonsGrid.EmptyStateHintText = "فیلترها یا عبارت جستجو را تغییر دهید";
            }
            else
            {
                PersonsGrid.EmptyStateText = "هنوز شخصی ثبت نشده است";
                PersonsGrid.EmptyStateHintText = "برای افزودن، از دکمه «+ شخص جدید» استفاده کنید";
            }
        }

        private async void PersonListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;
            _isLoadedOnce = true;
            try
            {
                await ViewModel.LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PersonListView] Load persons error: {ex}");
                ToastManager.Error("خطا در بارگذاری اشخاص");
            }
        }

        // ======================================================
        //  Tab Filter Handlers
        // ======================================================
        private void FilterTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton tb) return;

            string tabName = GetTabName(tb);
            bool isChecked = tb.IsChecked == true;

            ViewModel.UpdateTabSelection(tabName, isChecked);

            tabAll.IsChecked = ViewModel.SelectedTabs.Contains("all");
            tabCustomers.IsChecked = ViewModel.SelectedTabs.Contains("customer");
            tabSuppliers.IsChecked = ViewModel.SelectedTabs.Contains("supplier");
            tabPersonnel.IsChecked = ViewModel.SelectedTabs.Contains("personnel");
        }

        private string GetTabName(ToggleButton tb)
        {
            if (tb == tabAll) return "all";
            if (tb == tabCustomers) return "customer";
            if (tb == tabSuppliers) return "supplier";
            if (tb == tabPersonnel) return "personnel";
            return "all";
        }

        // ======================================================
        //  CRUD Handlers
        // ======================================================
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = ViewModel.GetSelectedItems();

            if (selectedItems.Count == 0)
            {
                if (PersonsGrid.Grid.SelectedItem is PersonItem item && !item.IsEmpty)
                    selectedItems.Add(item);
                else
                {
                    ToastManager.Warning("لطفاً یک شخص انتخاب کنید.");
                    return;
                }
            }

            var names = string.Join("\n", selectedItems.Take(5).Select(p => $"\u2022 {p.FullName}"));
            if (selectedItems.Count > 5)
                names += $"\n... و {selectedItems.Count - 5} مورد دیگر";

            var result = MessageBox.Show(
                $"آیا از حذف {selectedItems.Count} شخص مطمئن هستید؟\n\n{names}",
                "حذف شخص",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var (deletedCount, errors) = await ViewModel.DeleteSelectedAsync();
                _isLoadedOnce = false;

                // پنل‌های حذف‌شده‌ها حذف و بقیه‌ی انتخاب‌ها/جمع‌ها با لیست همگام شوند
                PersonsGrid.RefreshVisualState();
                UpdateDetailPanels();
                ViewModel.UpdateSummaryBar();

                if (errors.Count == 0)
                {
                    ToastManager.Success("عملیات حذف انجام شد.");
                }
                else
                {
                    var failed = string.Join("، ", errors.Take(3).Distinct());
                    if (errors.Count > 3)
                        failed += " و موارد دیگر";

                    ToastManager.Warning(
                        $"{ToPersianDigits(errors.Count)} مورد از اشخاص انتخاب‌شده حذف نشدند.{Environment.NewLine}{failed}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PersonListView] Delete error: {ex}");
                ToastManager.Error("خطا در حذف");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsGrid.Grid.SelectedItem is PersonItem item && !item.IsEmpty)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateToEditPerson(item.Id);
            }
            else
            {
                ToastManager.Warning("لطفاً یک شخص انتخاب کنید.");
            }
        }

        private void BtnNew_Click(object sender, MouseButtonEventArgs e)
        {
            // فرم «شخص جدید» به‌صورت مودال روی همین گرید باز می‌شود (گرید بسته نمی‌شود)
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.OpenNewPerson();
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.CloseCurrentForm();
        }

        /// <summary>
        /// رفرش داده‌های گرید از بیرون (مثلاً بعد از بسته‌شدن فرم «شخص جدید» در مودال).
        /// </summary>
        public async Task RefreshGridAsync()
        {
            _isLoadedOnce = false;
            try
            {
                await ViewModel.RefreshAsync();
                // پنل‌های موجود حذف شوند تا با داده‌ی جدید دوباره ساخته شوند
                ClearDetailPanels();
                UpdateDetailPanels();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PersonListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _isLoadedOnce = false;
            try
            {
                await ViewModel.RefreshAsync();
                ClearDetailPanels();
                UpdateDetailPanels();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PersonListView] Refresh error: {ex}");
                ToastManager.Error("خطا در بروزرسانی");
            }
        }

        // ======================================================
        //  Panel Toggle Animation
        // ======================================================
        private void TogglePanelBtn_Click(object sender, MouseButtonEventArgs e)
        {
            TogglePanel();
        }

        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel();
        }

        private void TogglePanel()
        {
            _isPanelOpen = !_isPanelOpen;

            var anim = new System.Windows.Media.Animation.DoubleAnimation();
            anim.Duration = TimeSpan.FromMilliseconds(250);
            var ease = new System.Windows.Media.Animation.CubicEase();
            var arrow = BtnToggleSidebar.Template?.FindName("ArrowRotation", BtnToggleSidebar) as System.Windows.Media.RotateTransform;

            if (_isPanelOpen)
            {
                anim.To = 340;
                ease.EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut;
                anim.EasingFunction = ease;
                DetailPanelContainer.Visibility = Visibility.Visible;
                DetailPanelContainer.Opacity = 0;
                PanelHeaderText.Visibility = Visibility.Visible;
                PanelHeaderText.Opacity = 0;
                BtnNewText.Visibility = Visibility.Visible;
                BtnNewText.Opacity = 0;
                BtnNewStack.HorizontalAlignment = HorizontalAlignment.Center;
                BtnNewBorder.Margin = new Thickness(10, 8, 10, 8);
                BtnNewBorder.Padding = new Thickness(0, 10, 0, 10);
                BtnNewBorder.Width = double.NaN;
                BtnNewBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                BtnNewIcon.Margin = new Thickness(0, 0, 10, 0);
                PanelHeaderBorder.Padding = new Thickness(10, 6, 10, 6);
                SelectedCountBadge.Padding = new Thickness(8, 2, 8, 2);
                SelectedCountBadge.Visibility = Visibility.Visible;
                PanelHeaderText.Visibility = Visibility.Visible;
                PanelHeaderText.Opacity = 1;
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
                fadeIn.EasingFunction = ease;
                DetailPanelContainer.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                PanelHeaderText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                BtnNewText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                var btnMarginAnim = new System.Windows.Media.Animation.ThicknessAnimation(
                    new Thickness(332, 40, 0, 0), TimeSpan.FromMilliseconds(250));
                btnMarginAnim.EasingFunction = ease;
                BtnToggleSidebar.BeginAnimation(FrameworkElement.MarginProperty, btnMarginAnim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MaxWidthProperty, anim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MinWidthProperty, anim);
                anim.Completed += (s, ev) =>
                {
                    DetailPanelColumn.Width = new GridLength(340);
                    BtnToggleSidebar.Margin = new Thickness(332, 40, 0, 0);
                };
                if (arrow != null) arrow.Angle = 0;

                if (DetailPanelsStack.Children.Count == 0)
                    UpdateDetailPanels();
            }
            else
            {
                anim.To = 64;
                ease.EasingMode = System.Windows.Media.Animation.EasingMode.EaseIn;
                anim.EasingFunction = ease;
                BtnNewStack.HorizontalAlignment = HorizontalAlignment.Center;
                BtnNewIcon.Margin = new Thickness(0);
                PanelHeaderBorder.Padding = new Thickness(10, 6, 10, 6);
                SelectedCountBadge.Padding = new Thickness(8, 2, 8, 2);
                SelectedCountBadge.Visibility = Visibility.Collapsed;
                PanelHeaderText.Visibility = Visibility.Collapsed;

                // پنل و عنوان مثل قبل همزمان با ستون محو می‌شوند...
                var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
                fadeOut.EasingFunction = ease;
                fadeOut.Completed += (s, ev) =>
                {
                    DetailPanelContainer.Visibility = Visibility.Collapsed;
                };

                // ...ولی متن دکمه «شخص جدید» سریع‌تر محو می‌شود تا وقتی دکمه همراه با
                // ستون جمع می‌شود، متن هیچ‌وقت بریده دیده نشود.
                var fadeOutBtnText = new System.Windows.Media.Animation.DoubleAnimation(0, TimeSpan.FromMilliseconds(160));
                fadeOutBtnText.EasingFunction = ease;
                fadeOutBtnText.Completed += (s, ev) =>
                {
                    BtnNewText.Visibility = Visibility.Collapsed;
                };
                DetailPanelContainer.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                PanelHeaderText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                BtnNewText.BeginAnimation(UIElement.OpacityProperty, fadeOutBtnText);
                var btnMarginAnim = new System.Windows.Media.Animation.ThicknessAnimation(
                    new Thickness(56, 40, 0, 0), TimeSpan.FromMilliseconds(250));
                btnMarginAnim.EasingFunction = ease;
                BtnToggleSidebar.BeginAnimation(FrameworkElement.MarginProperty, btnMarginAnim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MaxWidthProperty, anim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MinWidthProperty, anim);
                anim.Completed += (s, ev) =>
                {
                    DetailPanelColumn.Width = new GridLength(64);
                    BtnToggleSidebar.Margin = new Thickness(56, 40, 0, 0);

                    // بعد از کامل‌شدن جمع‌شدن (متن دیگر نیست)، دکمه به حالت مربعی
                    // متمرکز فقط-آیکون می‌رود — بدون هیچ پرشی چون عرضش در این لحظه
                    // دقیقاً 44px است (64 منهای حاشیه‌ها).
                    BtnNewBorder.Width = 44;
                    BtnNewBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    BtnNewBorder.Margin = new Thickness(6, 8, 6, 8);
                    BtnNewBorder.Padding = new Thickness(0, 8, 0, 8);
                };
                if (arrow != null) arrow.Angle = 180;
            }
        }

        // ======================================================
        //  Detail Panel Management
        // ======================================================
        /// <summary>
        /// تمام پنل‌های جزئیات باز را حذف می‌کند (با cleanup handlerها).
        /// </summary>
        private void ClearDetailPanels()
        {
            var panels = DetailPanelsStack.Children
                .OfType<Taadol.Controls.PersonDetailPanel>()
                .ToList();
            foreach (var panel in panels)
            {
                panel.CloseRequested -= DetailPanel_CloseRequested;
                panel.EditRequested -= DetailPanel_EditRequested;
                panel.DeleteRequested -= DetailPanel_DeleteRequested;
                DetailPanelsStack.Children.Remove(panel);
            }
        }

        private async void UpdateDetailPanels()
        {
            if (_isUpdatingPanels) return;
            _isUpdatingPanels = true;
            try
            {
                // پنل‌هایی که دیگر انتخاب نیستند حذف شوند
                var currentIds = ViewModel.GetSelectedItems().Select(p => p.Id).ToHashSet();
                var toRemove = DetailPanelsStack.Children
                    .OfType<Taadol.Controls.PersonDetailPanel>()
                    .Where(p => !currentIds.Contains(p.PersonId))
                    .ToList();

                foreach (var panel in toRemove)
                {
                    // ✅ Event handler cleanup: قبل از حذف پنل، handler ها را جدا کن
                    // تا اگر reference به پنل باقی ماند، handler اجرا نشود
                    panel.CloseRequested -= DetailPanel_CloseRequested;
                    panel.EditRequested -= DetailPanel_EditRequested;
                    panel.DeleteRequested -= DetailPanel_DeleteRequested;

                    DetailPanelsStack.Children.Remove(panel);
                }

                // ساخت تدریجی پنل‌ها: بین هر پنل به Dispatcher فرصت render می‌دهیم
                // تا ساخت هم‌زمان پنل‌های زیاد (مثلاً «انتخاب همه») برنامه را فریز نکند.
                // در هر تکرار، انتخاب‌ها دوباره خوانده می‌شوند تا اگر در این بین رویداد جدیدی
                // آمد (کلیک روی چند ردیف)، پنل‌ها دوباره/دوبار ساخته نشوند.
                while (true)
                {
                    var selectedItems = ViewModel.GetSelectedItems();
                    var existingIds = DetailPanelsStack.Children
                        .OfType<Taadol.Controls.PersonDetailPanel>()
                        .Select(p => p.PersonId)
                        .ToHashSet();

                    var item = selectedItems.FirstOrDefault(p => !existingIds.Contains(p.Id));
                    if (item == null)
                        break;

                    var panel = new Taadol.Controls.PersonDetailPanel();

                    // PersonType نام نوع انتخاب‌شده (مثلاً «مشتری و تامین کننده») است؛
                    // IsLegal فقط حقیقی/حقوقی را نشان می‌دهد و نباید در ردیف دسته‌بندی نمایش داده شود.
                    var personType = string.IsNullOrWhiteSpace(item.PersonType)
                        ? "—"
                        : item.PersonType;
                    var category = string.IsNullOrWhiteSpace(item.PersonCategoryTitle)
                        ? "—"
                        : item.PersonCategoryTitle;
                    var balance = item.BalanceDisplay ?? "\u2014";
                    var balanceStatus = item.AccountStatus ?? "";
                    var phone = !string.IsNullOrEmpty(item.Phone) && !string.IsNullOrEmpty(item.Mobile)
                        ? $"{item.Phone} / {item.Mobile}"
                        : !string.IsNullOrEmpty(item.Mobile) ? item.Mobile
                        : item.Phone ?? "";
                    var city = !string.IsNullOrEmpty(item.Province) && !string.IsNullOrEmpty(item.City)
                        ? $"{item.Province} / {item.City}"
                        : !string.IsNullOrEmpty(item.Province) ? item.Province
                        : item.City ?? "";
                    var nationalId = item.NationalId ?? "";
                    var email = ViewModel.GetEmail(item.Id) ?? "—";
                    var address = ViewModel.GetAddress(item.Id) ?? "—";
                    var postalCode = ViewModel.GetPostalCode(item.Id) ?? "";

                    panel.LoadData(
                        item.Id,
                        item.FullName,
                        personType,
                        category,
                        nationalId,
                        phone,
                        email,
                        city,
                        address,
                        postalCode,
                        balance,
                        balanceStatus,
                        item.Status == "فعال");

                    // حساب‌های بانکی از کش (پر شده در LoadDataAsync) — بدون کوئری دیتابیس
                    var bankItems = ViewModel.GetBankAccounts(item.Id)
                        .Select(b => new Taadol.Models.BankAccountItem
                        {
                            BankName = b.BankName ?? "\u2014",
                            BranchName = b.BankBranchName ?? "\u2014",
                            CardNumber = b.CardNumber ?? "\u2014",
                            ShebaNumber = b.Shaba ?? "\u2014",
                            AccountNumber = b.AccountNumber ?? "\u2014",
                            OtherAccount = "ندارد",
                            IsDefault = b.IsDefault
                        }).ToList();
                    panel.LoadBankAccounts(bankItems);

                    panel.CloseRequested += DetailPanel_CloseRequested;
                    panel.EditRequested += DetailPanel_EditRequested;
                    panel.DeleteRequested += DetailPanel_DeleteRequested;

                    DetailPanelsStack.Children.Insert(0, panel);

                    // پنل جدید بالای لیست اضافه شد — اسکرول را نرم به ابتدا ببر
                    // تا نفر تازه‌انتخاب‌شده دقیقاً دیده شود.
                    if (DetailPanelScroll != null)
                        DetailPanelScroll.SmoothScrollToTop();

                    // به UI فرصت render بده
                    await Dispatcher.Yield(DispatcherPriority.Background);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PersonListView] Show details error: {ex}");
                ToastManager.Error("خطا در نمایش جزئیات");
            }
            finally
            {
                _isUpdatingPanels = false;
            }
        }

        private void DetailPanel_CloseRequested(object sender, long personId)
        {
            var item = ViewModel.AllPersons?.FirstOrDefault(p => p.Id == personId);
            if (item != null)
            {
                item.IsSelected = false;
                PersonsGrid.RefreshVisualState();
                UpdateDetailPanels();
                ViewModel.UpdateSummaryBar();
            }
        }

        private void DetailPanel_EditRequested(object sender, long personId)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToEditPerson(personId);
        }

        private async void DetailPanel_DeleteRequested(object sender, long personId)
        {
            var item = ViewModel.AllPersons?.FirstOrDefault(p => p.Id == personId && !p.IsEmpty);
            if (item == null) return;

            var result = MessageBox.Show(
                $"آیا از حذف \u00AB{item.FullName}\u00BB مطمئن هستید؟",
                "حذف شخص",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await ViewModel.DeletePersonAsync(item);

                var panelToRemove = DetailPanelsStack.Children
                    .OfType<Taadol.Controls.PersonDetailPanel>()
                    .FirstOrDefault(p => p.PersonId == personId);
                if (panelToRemove != null)
                    DetailPanelsStack.Children.Remove(panelToRemove);

                _isLoadedOnce = false;

                // انتخاب‌های باقی‌مانده حفظ شده‌اند؛ وضعیت چک‌باکس‌ها، پنل‌ها و جمع‌ها را همگام کن
                PersonsGrid.RefreshVisualState();
                UpdateDetailPanels();
                ViewModel.UpdateSummaryBar();

                ToastManager.Success("عملیات حذف انجام شد.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PersonListView] Delete error: {ex}");
                ToastManager.Error("خطا در حذف");
            }
        }

        // ======================================================
        //  Scroll & Layout
        // ======================================================
        private void DetailPanelContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DetailPanelScroll == null) return;

            if (e.Delta > 0)
                DetailPanelScroll.LineUp();
            else
                DetailPanelScroll.LineDown();

            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FillAvailableSpace();
            }), DispatcherPriority.Background);
        }

        // فرم لیست باید همیشه کل فضای محتوا رو پر کنه حتی اگه ردیف جدول کم باشه
        // یا پنل جزئیات بسته باشه (چون MainContent با Top/Left فقط اندازه محتوا رو می‌گیره).
        private void FillAvailableSpace()
        {
            if (Window.GetWindow(this) is not Window window) return;
            if (window.FindName("MainContentBorder") is not Border border) return;

            if (!_sizeWired)
            {
                _sizeWired = true;
                border.SizeChanged += (s, _) => FillAvailableSpace();
            }

            var pad = border.Padding;
            var margin = Margin;
            var w = border.ActualWidth - pad.Left - pad.Right - margin.Left - margin.Right;
            var h = border.ActualHeight - pad.Top - pad.Bottom - margin.Top - margin.Bottom;
            Width = w > 0 ? w : 0;
            Height = h > 0 ? h : 0;
        }

        // ======================================================
        //  Popup Filter Handlers
        // ======================================================
        private void StatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر وضعیت",
                options: new List<string> { "فعال", "غیرفعال" },
                selected: ViewModel.SelectedStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    ViewModel.SetFilterResult("status", result);
                });
        }

        private void ProvinceFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر استان",
                options: ViewModel.GetProvinceOptions(),
                selected: ViewModel.SelectedProvinces,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    ViewModel.SetFilterResult("province", result);
                });
        }

        private void CityFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر شهر",
                options: ViewModel.GetCityOptions(),
                selected: ViewModel.SelectedCities,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    ViewModel.SetFilterResult("city", result);
                });
        }

        private void LegalStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر نوع",
                options: new List<string> { "حقیقی", "حقوقی" },
                selected: ViewModel.SelectedLegalStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    ViewModel.SetFilterResult("legal", result);
                });
        }

        private void AccountStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر وضعیت حساب",
                options: ViewModel.GetAccountStatusOptions(),
                selected: ViewModel.SelectedAccountStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    ViewModel.SetFilterResult("account", result);
                });
        }

        private void ShowFilterPopup(
            Button anchor,
            string title,
            List<string> options,
            HashSet<string> selected,
            bool showSearch,
            bool immediateApply,
            Action<List<string>> onSelectionChanged)
        {
            if (anchor == null) return;

            var popup = new Taadol.Controls.FilterPopupControl
            {
                Title = title,
                Options = options,
                SelectedOptions = new HashSet<string>(selected),
                ShowSearch = showSearch,
                ImmediateApply = immediateApply
            };

            popup.SelectionChanged += (selectedList) =>
            {
                onSelectionChanged(selectedList);
            };

            popup.ShowAt(anchor);
        }

        // ======================================================
        //  Helpers
        // ======================================================
        private static string BuildFullExceptionMessage(Exception ex)
        {
            if (ex == null) return "خطای ناشناخته.";
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Type: {ex.GetType().Name}");
            sb.AppendLine($"Message: {ex.Message}");
            var inner = ex.InnerException;
            int depth = 1;
            while (inner != null && depth <= 5)
            {
                sb.AppendLine($"--- Inner #{depth} ({inner.GetType().Name}) ---");
                sb.AppendLine($"Message: {inner.Message}");
                inner = inner.InnerException;
                depth++;
            }
            return sb.ToString();
        }


        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
        }

        private static string ToPersianDigits(int number)
        {
            string[] pd = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            var sb = new System.Text.StringBuilder();
            foreach (char c in number.ToString())
                sb.Append(char.IsDigit(c) ? pd[c - '0'] : c);
            return sb.ToString();
        }

    }

    // ======================================================
    //  PersonItem (مدل ردیف DataGrid)
    // ======================================================
    public class PersonItem : INotifyPropertyChanged, IListRowItem
    {
        private int _rowNumber;
        private bool _isSelected;

        public long Id { get; set; }

        public int RowNumber
        {
            get => _rowNumber;
            set
            {
                _rowNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumber)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumberDisplay)));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public string Code { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameText { get; set; }
        public string Company { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string NationalId { get; set; }
        public string EconomicId { get; set; }
        public string AccountStatus { get; set; }
        public string PersonType { get; set; }
        public string PersonCategoryTitle { get; set; }
        public bool IsEmpty { get; set; }
        public string LastTransaction { get; set; } = "\u2014";
        public bool IsLegal { get; set; }
        public string LegalStatus => IsEmpty ? "" : IsLegal ? "حقوقی" : "حقیقی";
        public string TransactionType { get; set; } = "\u2014";
        public string TransactionDate { get; set; } = "\u2014";
        public string BalanceDisplay { get; set; } = "\u2014";

        public string RowBackground => AccountStatus == "بستانکار" ? "#FCEBEC" : "White";
        public string TransactionIconPath => AccountStatus == "بدهکار" ? "/Assets/Icons/export.svg" : (AccountStatus == "بستانکار" ? "/Assets/Icons/import.svg" : "");
        public string BalanceColor => AccountStatus == "بدهکار" ? "#22C55E" : (AccountStatus == "بستانکار" ? "#DC2626" : "#374151");

        public string FullName =>
            IsEmpty ? "" : !string.IsNullOrWhiteSpace(FullNameText)
                ? FullNameText
                : $"{FirstName} {LastName}".Trim();

        public string RowNumberDisplay =>
            RowNumber > 0 && !IsEmpty ? ToPersianNumber(RowNumber) : "";

        public Visibility StatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(Status) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility AccountStatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(AccountStatus) || AccountStatus == "\u2014"
                ? Visibility.Collapsed : Visibility.Visible;

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "\u06F0", "\u06F1", "\u06F2", "\u06F3", "\u06F4", "\u06F5", "\u06F6", "\u06F7", "\u06F8", "\u06F9" };
            string result = "";
            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PageItem
    {
        public int PageNumber { get; set; }
        public string PageNumberDisplay { get; set; }
        public bool IsCurrent { get; set; }
    }

    /// <summary>
    /// اسکرول نرم برای ScrollViewer. چون VerticalOffset قابل انیمیشن مستقیم نیست،
    /// از طریق یک attached property واسطه انیمیت می‌شود.
    /// </summary>
    public static class SmoothScrollHelper
    {
        private static readonly DependencyProperty AnimatedOffsetProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedOffset", typeof(double), typeof(SmoothScrollHelper),
                new PropertyMetadata(0.0, OnAnimatedOffsetChanged));

        private static void OnAnimatedOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer sv && e.NewValue is double value)
                sv.ScrollToVerticalOffset(value);
        }

        public static void SmoothScrollToTop(this ScrollViewer sv, double durationMs = 400)
        {
            var animation = new DoubleAnimation
            {
                From = sv.VerticalOffset,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            animation.Completed += (s, e) => sv.BeginAnimation(AnimatedOffsetProperty, null);
            sv.BeginAnimation(AnimatedOffsetProperty, animation);
        }
    }
}
