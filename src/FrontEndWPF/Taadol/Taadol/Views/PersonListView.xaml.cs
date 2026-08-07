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
using System.Windows.Threading;
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

        public PersonListView()
        {
            InitializeComponent();

            using var scope = App.ServiceProvider.CreateScope();
            ViewModel = new PersonListViewModel(App.ServiceProvider);

            DataContext = ViewModel;

            SearchBox.TextChanged += (s, e) =>
            {
                ViewModel.HandleSearchTextChanged(SearchBox.Text);
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
                if (PersonsGrid.DoubleClickedItem is PersonItem item && !item.IsEmpty)
                {
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow?.NavigateToEditPerson(item.Id);
                }
            };

            PersonsGrid.CheckedItemsChanged += (s, e) =>
            {
                UpdateDetailPanels();
                ViewModel.UpdateSummaryBar();
            };

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            Loaded += PersonListView_Loaded;
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
                    SelectedTotalText.Text = ViewModel.SelectedTotalText;
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
        }

        private async void PersonListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;
            _isLoadedOnce = true;
            await ViewModel.LoadDataAsync();
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
                    MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
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
                await ViewModel.DeleteSelectedAsync();
                _isLoadedOnce = false;
                MessageBox.Show("عملیات حذف انجام شد.", "موفق",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف: " + ex.Message, "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnNew_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var newView = new NewPersonView();
                var mainContent = mainWindow.FindName("MainContent") as ContentControl;
                if (mainContent != null)
                {
                    mainContent.Content = newView;
                    var mainContentBorder = mainWindow.FindName("MainContentBorder") as Border;
                    if (mainContentBorder != null)
                        mainContentBorder.Visibility = Visibility.Visible;
                }
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _isLoadedOnce = false;
            await ViewModel.RefreshAsync();
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
            anim.Duration = TimeSpan.FromMilliseconds(180);
            var ease = new System.Windows.Media.Animation.SineEase();
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
                PanelHeaderStack.HorizontalAlignment = HorizontalAlignment.Left;
                PanelHeaderBorder.Padding = new Thickness(10, 6, 10, 6);
                SelectedCountBadge.Padding = new Thickness(8, 2, 8, 2);
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(1, TimeSpan.FromMilliseconds(180));
                fadeIn.EasingFunction = ease;
                DetailPanelContainer.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                PanelHeaderText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                BtnNewText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                var marginLeftAnim = new System.Windows.Media.Animation.ThicknessAnimation(
                    new Thickness(332, 40, 0, 0),
                    TimeSpan.FromMilliseconds(180));
                marginLeftAnim.EasingFunction = ease;
                BtnToggleSidebar.BeginAnimation(FrameworkElement.MarginProperty, marginLeftAnim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MaxWidthProperty, anim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MinWidthProperty, anim);
                anim.Completed += (s, ev) =>
                {
                    DetailPanelColumn.Width = new GridLength(340);
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
                BtnNewBorder.Margin = new Thickness(6, 8, 6, 8);
                BtnNewBorder.Padding = new Thickness(0, 8, 0, 8);
                BtnNewBorder.Width = 44;
                BtnNewBorder.HorizontalAlignment = HorizontalAlignment.Center;
                BtnNewIcon.Margin = new Thickness(0);
                PanelHeaderStack.HorizontalAlignment = HorizontalAlignment.Center;
                PanelHeaderBorder.Padding = new Thickness(4, 6, 4, 6);
                SelectedCountBadge.Padding = new Thickness(3, 1, 3, 1);
                var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(0, TimeSpan.FromMilliseconds(180));
                fadeOut.EasingFunction = ease;
                fadeOut.Completed += (s, ev) =>
                {
                    DetailPanelContainer.Visibility = Visibility.Collapsed;
                    PanelHeaderText.Visibility = Visibility.Collapsed;
                    BtnNewText.Visibility = Visibility.Collapsed;
                };
                DetailPanelContainer.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                PanelHeaderText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                BtnNewText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                var marginLeftAnim = new System.Windows.Media.Animation.ThicknessAnimation(
                    new Thickness(56, 40, 0, 0),
                    TimeSpan.FromMilliseconds(180));
                marginLeftAnim.EasingFunction = ease;
                BtnToggleSidebar.BeginAnimation(FrameworkElement.MarginProperty, marginLeftAnim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MaxWidthProperty, anim);
                DetailPanelColumn.BeginAnimation(System.Windows.Controls.ColumnDefinition.MinWidthProperty, anim);
                anim.Completed += (s, ev) =>
                {
                    DetailPanelColumn.Width = new GridLength(64);
                };
                if (arrow != null) arrow.Angle = 180;
            }
        }

        // ======================================================
        //  Detail Panel Management
        // ======================================================
        private async void UpdateDetailPanels()
        {
            var selectedItems = ViewModel.GetSelectedItems();

            var existingIds = DetailPanelsStack.Children
                .OfType<Taadol.Controls.PersonDetailPanel>()
                .Select(p => p.PersonId)
                .ToHashSet();

            var currentIds = selectedItems.Select(p => p.Id).ToHashSet();

            var toRemove = DetailPanelsStack.Children
                .OfType<Taadol.Controls.PersonDetailPanel>()
                .Where(p => !currentIds.Contains(p.PersonId))
                .ToList();

            foreach (var panel in toRemove)
                DetailPanelsStack.Children.Remove(panel);

            foreach (var item in selectedItems.Where(p => !existingIds.Contains(p.Id)))
            {
                var panel = new Taadol.Controls.PersonDetailPanel();

                var personType = item.IsLegal ? "حقوقی" : "حقیقی";
                var category = item.Category ?? "";
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

                panel.LoadData(
                    item.Id,
                    item.FullName,
                    personType,
                    category,
                    nationalId,
                    phone,
                    "",
                    city,
                    "",
                    balance,
                    balanceStatus,
                    item.Status == "فعال");

                try
                {
                    var bankItems = await Task.Run(() =>
                    {
                        using var scope = App.ServiceProvider.CreateScope();
                        var bankApp = scope.ServiceProvider.GetRequiredService<IPersonBankApplication>();
                        var banks = bankApp.GetByPersonId(item.Id) ?? new List<PersonBankViewModel>();
                        return banks.Select(b => new Taadol.Models.BankAccountItem
                        {
                            BankName = b.BankName ?? "\u2014",
                            BranchName = b.BankBranchName ?? "\u2014",
                            CardNumber = b.CardNumber ?? "\u2014",
                            ShebaNumber = b.Shaba ?? "\u2014",
                            AccountNumber = b.AccountNumber ?? "\u2014",
                            OtherAccount = "ندارد",
                            IsDefault = b.IsDefault
                        }).ToList();
                    });
                    panel.LoadBankAccounts(bankItems);
                }
                catch { }

                panel.CloseRequested += DetailPanel_CloseRequested;
                panel.EditRequested += DetailPanel_EditRequested;
                panel.DeleteRequested += DetailPanel_DeleteRequested;

                DetailPanelsStack.Children.Insert(0, panel);
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
                MessageBox.Show("عملیات حذف انجام شد.", "موفق",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف: " + ex.Message, "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ActionButton_Loaded(object sender, RoutedEventArgs e) { }
        private void ActionButton_Loaded_1(object sender, RoutedEventArgs e) { }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PersonsGrid_Loaded(object sender, RoutedEventArgs e)
        {

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
                if (_rowNumber != value)
                {
                    _rowNumber = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumber)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowNumberDisplay)));
                }
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
        public bool IsEmpty { get; set; }
        public string LastTransaction { get; set; } = "\u2014";
        public bool IsLegal { get; set; }
        public string LegalStatus => IsEmpty ? "" : IsLegal ? "حقوقی" : "حقیقی";
        public string TransactionType { get; set; } = "\u2014";
        public string TransactionDate { get; set; } = "\u2014";
        public string BalanceDisplay { get; set; } = "\u2014";

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
}
