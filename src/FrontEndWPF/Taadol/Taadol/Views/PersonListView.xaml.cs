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
using System.Windows.Threading;

namespace Taadol.Views
{
    /// <summary>
    /// فرم لیست اشخاص.
    /// اطلاعات شخص + تماس‌ها (موبایل/تلفن) + آدرس (استان/شهر) رو به‌صورت یکپارچه نشون می‌ده.
    /// دارای فیلترهای پاپ‌آپ روی ستون‌های وضعیت/استان/شهر.
    /// الگوی async/scope از BranchListView برداشته شده.
    /// </summary>
    public partial class PersonListView : UserControl
    {
        // ===== Data Collections =====
        public ObservableCollection<PersonItem> AllPersons { get; set; }
        public ObservableCollection<PersonItem> FilteredPersons { get; set; }

        // ===== State =====
        private int _pageSize = 15;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _lastFilteredListCount = 0;
        private bool _isLoadedOnce = false;

        // ===== Filter Selections =====
        // فیلتر تب‌ها (چند انتخابه)
        private readonly HashSet<string> _selectedTabs = new HashSet<string> { "all" };
        // برای فیلتر پاپ‌آپ: مقادیر انتخاب‌شده توسط کاربر
        // اگه خالی باشه = هیچ فیلتری اعمال نشده (همه نشون داده می‌شه)
        private readonly HashSet<string> _selectedStatuses = new HashSet<string>();
        private readonly HashSet<string> _selectedProvinces = new HashSet<string>();
        private readonly HashSet<string> _selectedCities = new HashSet<string>();
        private readonly HashSet<string> _selectedLegalStatuses = new HashSet<string>();
        private readonly HashSet<string> _selectedAccountStatuses = new HashSet<string>();

        public PersonListView()
        {
            InitializeComponent();
            FillEmptyRows();

            SearchBox.TextChanged += (s, e) =>
            {
                _currentPage = 1;
                ApplyFilters();
            };
            DataGridBorder.SizeChanged += DataGridBorder_SizeChanged;

            Loaded += PersonListView_Loaded;
        }
        private void DataGridBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataGridClipGeometry != null && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                DataGridClipGeometry.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            }
        }

        // ======================================================
        //  Async Data Load
        // ======================================================
        private async void PersonListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;
            _isLoadedOnce = true;
            await LoadDataAsync();
        }

        /// <summary>
        /// لود کردن اشخاص از دیتابیس به‌صورت async.
        /// برای هر شخص، تماس‌ها (موبایل/تلفن) و آدرس (استان/شهر) رو هم از سرویس‌های جداگانه می‌گیره.
        /// </summary>
        private async Task LoadDataAsync()
        {
            ShowLoading(true);

            // دادن فرصت به UI برای نمایش اسپینر قبل از کار سنگین
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var sp = scope.ServiceProvider;

                    var personApp = sp.GetRequiredService<IPersonApplication>();
                    var persons = personApp.GetPersons() ?? new List<PersonViewModel>();

                    var personIds = persons.Select(p => p.Id).ToList();

                    var allContacts = new List<PersonContactViewModel>();
                    var allAddresses = new List<PersonAddressViewModel>();

                    var contactTasks = personIds.Select(pid =>
                        Task.Run(() =>
                        {
                            using var s = App.ServiceProvider.CreateScope();
                            var app = s.ServiceProvider.GetRequiredService<IPersonContactApplication>();
                            try { return app.GetByPersonId(pid) ?? new List<PersonContactViewModel>(); }
                            catch { return new List<PersonContactViewModel>(); }
                        })).ToList();

                    var addressTasks = personIds.Select(pid =>
                        Task.Run(() =>
                        {
                            using var s = App.ServiceProvider.CreateScope();
                            var app = s.ServiceProvider.GetRequiredService<IPersonAddressApplication>();
                            try { return app.GetByPersonId(pid) ?? new List<PersonAddressViewModel>(); }
                            catch { return new List<PersonAddressViewModel>(); }
                        })).ToList();

                    Task.WaitAll(contactTasks.ToArray());
                    Task.WaitAll(addressTasks.ToArray());

                    foreach (var t in contactTasks) allContacts.AddRange(t.Result);
                    foreach (var t in addressTasks) allAddresses.AddRange(t.Result);

                    // گروه‌بندی تماس‌ها بر اساس PersonId
                    var contactsByPerson = allContacts
                        .GroupBy(c => c.PersonId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // گروه‌بندی آدرس‌ها بر اساس PersonId (آدرس پیش‌فرض یا اولین)
                    var addressesByPerson = allAddresses
                        .GroupBy(a => a.PersonId)
                        .ToDictionary(g => g.Key, g => g.FirstOrDefault());

                    return persons.Select((p, index) =>
                    {
                        // پیدا کردن موبایل و تلفن از لیست تماس‌ها
                        string mobile = "—";
                        string phone = "—";

                        if (contactsByPerson.TryGetValue(p.Id, out var contacts) && contacts.Count > 0)
                        {
                            // موبایل: تماسی که نوعش "موبایل" هست
                            var mobileContact = contacts.FirstOrDefault(c =>
                                c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("موبایل"));
                            if (mobileContact != null && !string.IsNullOrWhiteSpace(mobileContact.Value))
                                mobile = mobileContact.Value;

                            // تلفن: تماسی که نوعش "تلفن" هست
                            var phoneContact = contacts.FirstOrDefault(c =>
                                c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("تلفن"));
                            if (phoneContact != null && !string.IsNullOrWhiteSpace(phoneContact.Value))
                                phone = phoneContact.Value;
                        }

                        // پیدا کردن استان و شهر از آدرس
                        string province = "—";
                        string city = "—";

                        if (addressesByPerson.TryGetValue(p.Id, out var address) && address != null)
                        {
                            if (!string.IsNullOrWhiteSpace(address.ProvinceName))
                                province = address.ProvinceName;
                            if (!string.IsNullOrWhiteSpace(address.CityName))
                                city = address.CityName;
                        }

                        // نام کامل
                        string fullName;
                        if (p.IsLegal)
                        {
                            // شخص حقوقی: نام شرکت در FirstName قرار داره
                            fullName = p.FirstName ?? "";
                        }
                        else
                        {
                            fullName = $"{p.FirstName ?? ""} {p.LastName ?? ""}".Trim();
                            if (string.IsNullOrEmpty(fullName))
                                fullName = "—";
                        }

                        return new PersonItem
                        {
                            Id = p.Id,
                            RowNumber = index + 1,
                            Code = string.IsNullOrWhiteSpace(p.Code) ? p.Id.ToString() : p.Code,
                            Category = string.IsNullOrWhiteSpace(p.PersonType) ? "—" : p.PersonType,
                            Status = p.IsActive ? "فعال" : "غیرفعال",
                            Nickname = "—",
                            FullNameText = fullName,
                            Company = string.IsNullOrWhiteSpace(p.BranchName) ? "—" : p.BranchName,
                            Province = province,
                            City = city,
                            Phone = phone,
                            Mobile = mobile,
                            NationalId = string.IsNullOrWhiteSpace(p.NationalCode) ? "—" : p.NationalCode,
                            EconomicId = string.IsNullOrWhiteSpace(p.EconomicCode) ? "—" : p.EconomicCode,

                            // ★ اینها رو باید بعداً از سرویس تراکنش/حساب واقعی پر کنی
                            TransactionType = "—",
                            TransactionDate = "—",
                            AccountStatus = "—",
                            BalanceDisplay = "—",

                            IsLegal = p.IsLegal,
                            PersonType = p.PersonType,
                            IsEmpty = false
                        };
                    }).ToList();
                });

                AllPersons = new ObservableCollection<PersonItem>(items);
                _currentPage = 1;
                UpdateTabCounts();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(BuildFullExceptionMessage(ex), "خطا در لود اشخاص",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AllPersons = new ObservableCollection<PersonItem>();
                ApplyFilters();
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void ShowLoading(bool show)
        {
            if (LoadingOverlay != null)
                LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (PersonsDataGrid != null)
                PersonsDataGrid.IsHitTestVisible = !show;
        }

        // ======================================================
        //  Empty Rows (برای حفظ ارتفاع DataGrid وقتی داده‌ای نیست)
        // ======================================================
        private void FillEmptyRows()
        {
            FilteredPersons = new ObservableCollection<PersonItem>();

            for (int i = 1; i <= _pageSize; i++)
            {
                FilteredPersons.Add(new PersonItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            if (PersonsDataGrid != null)
                PersonsDataGrid.ItemsSource = FilteredPersons;
        }

        // ======================================================
        //  Filtering + Pagination
        // ======================================================
        private void ApplyFilters()
        {
            if (AllPersons == null) return;

            var query = AllPersons.AsEnumerable();

            // فیلتر نوع شخص (تب‌های بالا - چند انتخابه)
            if (!_selectedTabs.Contains("all"))
            {
                var tabFilters = new List<string>();
                if (_selectedTabs.Contains("customer")) tabFilters.Add("مشتری");
                if (_selectedTabs.Contains("supplier")) tabFilters.Add("تامین");
                if (_selectedTabs.Contains("personnel")) tabFilters.Add("پرسنل");

                if (tabFilters.Count > 0)
                {
                    query = query.Where(p => p.PersonType != null &&
                        tabFilters.Any(f => p.PersonType.Contains(f)));
                }
            }

            // فیلتر پاپ‌آپ وضعیت
            if (_selectedStatuses.Count > 0)
            {
                query = query.Where(p => p.Status != null && _selectedStatuses.Contains(p.Status));
            }

            // فیلتر پاپ‌آپ استان
            if (_selectedProvinces.Count > 0)
            {
                query = query.Where(p => p.Province != null && p.Province != "—" && _selectedProvinces.Contains(p.Province));
            }

            // فیلتر پاپ‌آپ شهر
            if (_selectedCities.Count > 0)
            {
                query = query.Where(p => p.City != null && p.City != "—" && _selectedCities.Contains(p.City));
            }

            // فیلتر پاپ‌آپ نوع
            if (_selectedLegalStatuses.Count > 0)
            {
                query = query.Where(p => p.LegalStatus != null && _selectedLegalStatuses.Contains(p.LegalStatus));
            }

            // فیلتر پاپ‌آپ وضعیت حساب
            if (_selectedAccountStatuses.Count > 0)
            {
                query = query.Where(p => p.AccountStatus != null && p.AccountStatus != "—" && _selectedAccountStatuses.Contains(p.AccountStatus));
            }

            var searchText = SearchBox?.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    (!string.IsNullOrWhiteSpace(p.Code) && p.Code.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.FullName) && p.FullName.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.NationalId) && p.NationalId.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.EconomicId) && p.EconomicId.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.Company) && p.Company.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.Mobile) && p.Mobile.Contains(searchText)) ||
                    (!string.IsNullOrWhiteSpace(p.Phone) && p.Phone.Contains(searchText))
                );
            }

            var filteredList = query.ToList();
            _lastFilteredListCount = filteredList.Count;

            _totalPages = (int)Math.Ceiling(filteredList.Count / (double)_pageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = filteredList
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = ((_currentPage - 1) * _pageSize) + i + 1;

            FilteredPersons = new ObservableCollection<PersonItem>(pageItems);

            int realCount = FilteredPersons.Count;
            for (int i = realCount + 1; i <= _pageSize; i++)
            {
                FilteredPersons.Add(new PersonItem
                {
                    RowNumber = 0,
                    IsEmpty = true
                });
            }

            if (PersonsDataGrid != null)
                PersonsDataGrid.ItemsSource = FilteredPersons;

            BuildPaginationButtons();
            UpdatePageInfo();
            UpdateSummaryBar();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateRowBorders();
                AdjustDataGridHeight();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void BuildPaginationButtons()
        {
            if (PageButtonsItemsControl == null) return;

            var pages = new ObservableCollection<PageItem>();

            if (_totalPages <= 5)
            {
                for (int i = 1; i <= _totalPages; i++)
                {
                    pages.Add(new PageItem
                    {
                        PageNumber = i,
                        PageNumberDisplay = ToPersianNumber(i),
                        IsCurrent = i == _currentPage
                    });
                }
                PageButtonsItemsControl.ItemsSource = pages;
                return;
            }

            // صفحه اول
            pages.Add(new PageItem
            {
                PageNumber = 1,
                PageNumberDisplay = ToPersianNumber(1),
                IsCurrent = _currentPage == 1
            });

            // سه نقطه اول — همیشه ثابت
            pages.Add(new PageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });

            // صفحات وسط (۳ تا)
            int middleStart = _currentPage - 1;
            int middleEnd = _currentPage + 1;

            if (_currentPage <= 3)
            {
                middleStart = 2;
                middleEnd = 4;
            }
            else if (_currentPage >= _totalPages - 2)
            {
                middleStart = _totalPages - 3;
                middleEnd = _totalPages - 1;
            }

            for (int i = middleStart; i <= middleEnd; i++)
            {
                if (i > 1 && i < _totalPages)
                {
                    pages.Add(new PageItem
                    {
                        PageNumber = i,
                        PageNumberDisplay = ToPersianNumber(i),
                        IsCurrent = i == _currentPage
                    });
                }
            }

            // سه نقطه دوم — همیشه ثابت
            pages.Add(new PageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });

            // صفحه آخر
            pages.Add(new PageItem
            {
                PageNumber = _totalPages,
                PageNumberDisplay = ToPersianNumber(_totalPages),
                IsCurrent = _currentPage == _totalPages
            });

            PageButtonsItemsControl.ItemsSource = pages;
        }

        // ======================================================
        //  Event Handlers
        // ======================================================
        private void FilterTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton tb) return;

            string tabName = GetTabName(tb);

            if (tb.IsChecked == true)
            {
                if (tabName == "all")
                {
                    // همه: بقیه رو خاموش کن
                    _selectedTabs.Clear();
                    _selectedTabs.Add("all");
                    UpdateTabStates();
                }
                else
                {
                    // غیرفعال کردن "همه"
                    _selectedTabs.Remove("all");
                    tabAll.IsChecked = false;
                    _selectedTabs.Add(tabName);
                }
            }
            else
            {
                // وقتی یکی خاموش میشه
                _selectedTabs.Remove(tabName);

                // اگه هیچ‌کدوم انتخاب نباشه، همه فعال بشه
                if (_selectedTabs.Count == 0)
                {
                    _selectedTabs.Add("all");
                    tabAll.IsChecked = true;
                }
            }

            _currentPage = 1;
            ApplyFilters();
        }

        private string GetTabName(ToggleButton tb)
        {
            if (tb == tabAll) return "all";
            if (tb == tabCustomers) return "customer";
            if (tb == tabSuppliers) return "supplier";
            if (tb == tabPersonnel) return "personnel";
            return "all";
        }

        private void UpdateTabStates()
        {
            tabAll.IsChecked = _selectedTabs.Contains("all");
            tabCustomers.IsChecked = _selectedTabs.Contains("customer");
            tabSuppliers.IsChecked = _selectedTabs.Contains("supplier");
            tabPersonnel.IsChecked = _selectedTabs.Contains("personnel");
        }

        private void UpdateTabCounts()
        {
            if (AllPersons == null) return;

            var validPersons = AllPersons.Where(p => !p.IsEmpty).ToList();
            int total = validPersons.Count;
            int customers = validPersons.Count(p => p.PersonType != null && p.PersonType.Contains("مشتری"));
            int suppliers = validPersons.Count(p => p.PersonType != null && p.PersonType.Contains("تامین"));
            int personnel = validPersons.Count(p => p.PersonType != null && p.PersonType.Contains("پرسنل"));

            tabAll.Tag = $"( {ToPersianNumber(total)} )";
            tabCustomers.Tag = $"( {ToPersianNumber(customers)} )";
            tabSuppliers.Tag = $"( {ToPersianNumber(suppliers)} )";
            tabPersonnel.Tag = $"( {ToPersianNumber(personnel)} )";
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // پشتیبانی از حذف چندتایی با چک‌باکس (مثل BranchListView) + حذف تک‌تک با انتخاب ردیف
            var selectedItems = AllPersons?
                .Where(p => p.IsSelected && !p.IsEmpty)
                .ToList() ?? new List<PersonItem>();

            if (selectedItems.Count == 0)
            {
                if (PersonsDataGrid.SelectedItem is PersonItem item && !item.IsEmpty)
                    selectedItems.Add(item);
                else
                {
                    MessageBox.Show("لطفاً یک شخص انتخاب کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var names = string.Join("\n", selectedItems.Take(5).Select(p => $"• {p.FullName}"));
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
                var idsToDelete = selectedItems.Select(p => p.Id).ToList();
                await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var personApp = scope.ServiceProvider.GetRequiredService<IPersonApplication>();

                    foreach (var id in idsToDelete)
                    {
                        var op = personApp.Remove(id);
                        if (!op.IsSucceeded)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"Failed to delete person {id}: {op.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف: " + ex.Message, "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // رفرش لیست
            _isLoadedOnce = false;
            await LoadDataAsync();

            MessageBox.Show("عملیات حذف انجام شد.", "موفق",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is PersonItem item && !item.IsEmpty)
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

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            // باز کردن فرم ثبت شخص جدید در MainWindow
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
                else
                {
                    MessageBox.Show("فرم شخص جدید", "شخص جدید",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("فرم شخص جدید", "شخص جدید",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("منوی بیشتر — شامل: فعال/غیرفعال کردن، صادرات Excel، چاپ، و...",
                "عملیات", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _isLoadedOnce = false;
            _ = LoadDataAsync();
        }

        private void DetailPanelContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DetailPanelScroll == null) return;

            if (e.Delta > 0)
                DetailPanelScroll.LineUp();
            else
                DetailPanelScroll.LineDown();

            e.Handled = true;
        }

        private void CheckBoxBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is PersonItem item)
            {
                item.IsSelected = !item.IsSelected;
                UpdateRowBorders();
                UpdateDetailPanels();
                UpdateSummaryBar();
                e.Handled = true;
            }
        }
        private void AdjustDataGridHeight()
        {
            if (PersonsDataGrid == null || PersonsDataGrid.Items == null)
                return;

            const double headerHeight = 44;
            const double rowHeight = 36;

            // فقط ردیف‌های واقعی رو بشمار، نه پدینگ خالی
            int itemCount = PersonsDataGrid.Items
                .OfType<PersonItem>()
                .Count(p => !p.IsEmpty);

            double requiredDataGridHeight = headerHeight + (itemCount * rowHeight);

            double totalHeight = RootBorder.ActualHeight;
            double usedHeight = HeaderBorder.ActualHeight + FooterBorder.ActualHeight;
            double availableHeight = Math.Max(totalHeight - usedHeight, headerHeight);

            double otherComponentsHeight = 0;
            if (FilterBarBorder != null) otherComponentsHeight += FilterBarBorder.ActualHeight;
            if (SummaryBorder != null) otherComponentsHeight += SummaryBorder.ActualHeight;
            if (PaginationBorder != null) otherComponentsHeight += PaginationBorder.ActualHeight;

            double maxAllowedHeight = Math.Max(availableHeight - otherComponentsHeight, headerHeight);

            if (requiredDataGridHeight <= maxAllowedHeight)
            {
                // داده کمه → گرید جمع بشه، بدون فضای خالی زیرش
                PersonsDataGrid.Height = requiredDataGridHeight;
                PersonsDataGrid.MaxHeight = requiredDataGridHeight;
            }
            else
            {
                // داده زیاده → کل فضای موجود پر بشه و اسکرول فعال شه
                PersonsDataGrid.Height = maxAllowedHeight;
                PersonsDataGrid.MaxHeight = maxAllowedHeight;
            }
        }
        private void RootBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => AdjustDataGridHeight()), DispatcherPriority.Background);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // با تأخیر تا Layout کامل شود
            Dispatcher.BeginInvoke(new Action(() => AdjustDataGridHeight()), DispatcherPriority.Background);
        }
        private void UpdateRowBorders()
        {
            var items = PersonsDataGrid.Items;
            var blue = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2667FF"));
            var transparent = System.Windows.Media.Brushes.Transparent;

            for (int i = 0; i < items.Count; i++)
            {
                var row = PersonsDataGrid.ItemContainerGenerator.ContainerFromIndex(i) as System.Windows.Controls.DataGridRow;
                if (row == null) continue;

                var item = items[i] as PersonItem;
                if (item == null || item.IsEmpty)
                {
                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                    continue;
                }

                bool prevSelected = (i > 0) && items[i - 1] is PersonItem prev && !prev.IsEmpty && prev.IsSelected;

                if (item.IsSelected)
                {
                    row.BorderBrush = blue;
                    row.BorderThickness = new Thickness(0, prevSelected ? 0 : 1, 0, 1);
                }
                else
                {
                    row.BorderBrush = transparent;
                    row.BorderThickness = new Thickness(0);
                }
            }
        }

        private async void UpdateDetailPanels()
        {
            var selectedItems = AllPersons?
                .Where(p => p.IsSelected && !p.IsEmpty)
                .ToList() ?? new List<PersonItem>();

            SelectedCountText.Text = $"({selectedItems.Count})";

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
                var balance = item.BalanceDisplay ?? "—";
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
                            BankName = b.BankName ?? "—",
                            BranchName = b.BankBranchName ?? "—",
                            CardNumber = b.CardNumber ?? "—",
                            ShebaNumber = b.Shaba ?? "—",
                            AccountNumber = b.AccountNumber ?? "—",
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
            var item = AllPersons?.FirstOrDefault(p => p.Id == personId);
            if (item != null)
            {
                item.IsSelected = false;
                UpdateRowBorders();
                UpdateDetailPanels();
                UpdateSummaryBar();
            }
        }

        private void DetailPanel_EditRequested(object sender, long personId)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToEditPerson(personId);
        }

        private async void DetailPanel_DeleteRequested(object sender, long personId)
        {
            var item = AllPersons?.FirstOrDefault(p => p.Id == personId && !p.IsEmpty);
            if (item == null) return;

            var result = MessageBox.Show(
                $"آیا از حذف «{item.FullName}» مطمئن هستید؟",
                "حذف شخص",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var personApp = scope.ServiceProvider.GetRequiredService<IPersonApplication>();
                var op = personApp.Remove(item.Id);
                if (!op.IsSucceeded)
                {
                    MessageBox.Show("خطا در حذف: " + op.Message, "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف: " + ex.Message, "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var panelToRemove = DetailPanelsStack.Children
                .OfType<Taadol.Controls.PersonDetailPanel>()
                .FirstOrDefault(p => p.PersonId == personId);
            if (panelToRemove != null)
                DetailPanelsStack.Children.Remove(panelToRemove);

            _isLoadedOnce = false;
            await LoadDataAsync();

            MessageBox.Show("عملیات حذف انجام شد.", "موفق",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateSummaryBar()
        {
            if (AllPersons == null) return;

            var validPersons = AllPersons.Where(p => !p.IsEmpty).ToList();
            var selectedItems = validPersons.Where(p => p.IsSelected).ToList();

            // جمع بدهکار / بستانکار
            long totalDebit = 0;
            long totalCredit = 0;
            foreach (var p in validPersons)
            {
                if (long.TryParse(p.BalanceDisplay?.Replace(",", "").Replace("ریال", "").Trim(), out long bal))
                {
                    if (p.AccountStatus == "بدهکار")
                        totalDebit += bal;
                    else if (p.AccountStatus == "بستانکار")
                        totalCredit += bal;
                }
            }

            if (TotalDebitText != null)
                TotalDebitText.Text = $"{ToPersianNumber(totalDebit)} ریال";
            if (TotalCreditText != null)
                TotalCreditText.Text = $"{ToPersianNumber(totalCredit)} ریال";
            if (SelectedSummaryText != null)
                SelectedSummaryText.Text = $"جمع اشخاص انتخاب شده ({ToPersianNumber(selectedItems.Count)})";
            if (SelectedTotalText != null)
            {
                long selectedTotal = 0;
                foreach (var p in selectedItems)
                {
                    if (long.TryParse(p.BalanceDisplay?.Replace(",", "").Replace("ریال", "").Trim(), out long bal))
                        selectedTotal += bal;
                }
                SelectedTotalText.Text = $"{ToPersianNumber(selectedTotal)} ریال";
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyFilters();
            }
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFilters();
            }
        }

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int pageNumber = Convert.ToInt32(btn.Tag);
                if (pageNumber <= 0) return;
                _currentPage = pageNumber;
                ApplyFilters();
            }
        }

        private void PageSizeSelector_SelectionChanged(object sender, int newSize)
        {
            _pageSize = newSize;
            _currentPage = 1;
            ApplyFilters();
        }

        private void UpdatePageInfo()
        {
            if (PageInfoText == null) return;
            int currentPageCount = FilteredPersons.Count(p => !p.IsEmpty);
            int totalCount = (int)Math.Ceiling(FilteredPersons.Count / (double)_pageSize) > 0
                ? (FilteredPersons.Count - FilteredPersons.Count(p => p.IsEmpty)) + (FilteredPersons.Count(p => !p.IsEmpty) > 0 ? 0 : 0)
                : 0;
            // Use the filtered source count from the last ApplyFilters
            int totalFiltered = _lastFilteredListCount;
            PageInfoText.Text = $"نمایش {ToPersianNumber(currentPageCount)} از {ToPersianNumber(totalFiltered)} مورد";
        }

        private void PersonsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // جلوگیری از انتخاب خودکار توسط DataGrid
            if (PersonsDataGrid.SelectedItem != null)
            {
                PersonsDataGrid.SelectedItem = null;
            }
        }

        private void PersonsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is PersonItem item && !item.IsEmpty)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.NavigateToEditPerson(item.Id);
            }
        }
        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // فقط اگر کلیک داخل چک‌باکس بود، اجازه بده
            if (!IsInsideCheckBox(e.OriginalSource as DependencyObject))
            {
                e.Handled = true;
            }
        }
        private void DataGridRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsInsideCheckBox(e.OriginalSource as DependencyObject))
            {
                e.Handled = true;
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.DataGridRow row && row.DataContext is PersonItem item && !item.IsEmpty)
            {
                row.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EFF6FF"));
            }
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.DataGridRow row && row.DataContext is PersonItem item && !item.IsEmpty)
            {
                bool isAlt = row.AlternationIndex == 1;
                var bgColor = isAlt ? (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8F8F8") : System.Windows.Media.Colors.White;
                row.Background = new System.Windows.Media.SolidColorBrush(bgColor);
            }
        }
        private bool IsInsideCheckBox(DependencyObject element)
        {
            while (element != null)
            {
                if (element is FrameworkElement fe && fe.Name == "CheckBoxBorder")
                    return true;
                element = System.Windows.Media.VisualTreeHelper.GetParent(element);
            }
            return false;
        }
        // ======================================================
        //  Popup Filter Handlers (وضعیت / استان / شهر)
        //  همه‌ی منطق پاپ‌آپ داخل FilterPopupControl قرار داره.
        //  برای ویرایش ظاهر، فایل Controls/FilterPopupControl.xaml رو ببینید.
        // ======================================================

        /// <summary>کلیک روی آیکون فیلتر وضعیت → پاپ‌آپ با «فعال» و «غیرفعال»</summary>
        private void StatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر وضعیت",
                options: new List<string> { "فعال", "غیرفعال" },
                selected: _selectedStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    _selectedStatuses.Clear();
                    foreach (var r in result) _selectedStatuses.Add(r);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        /// <summary>کلیک روی آیکون فیلتر استان</summary>
        private void ProvinceFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllPersons?
                .Select(p => p.Province)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "—")
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر استان",
                options: options,
                selected: _selectedProvinces,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    _selectedProvinces.Clear();
                    foreach (var r in result) _selectedProvinces.Add(r);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        /// <summary>کلیک روی آیکون فیلتر شهر</summary>
        private void CityFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllPersons?
                .Select(p => p.City)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "—")
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر شهر",
                options: options,
                selected: _selectedCities,
                showSearch: true,
                immediateApply: false,
                onSelectionChanged: result =>
                {
                    _selectedCities.Clear();
                    foreach (var r in result) _selectedCities.Add(r);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        /// <summary>کلیک روی آیکون فیلتر نوع</summary>
        private void LegalStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر نوع",
                options: new List<string> { "حقیقی", "حقوقی" },
                selected: _selectedLegalStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    _selectedLegalStatuses.Clear();
                    foreach (var r in result) _selectedLegalStatuses.Add(r);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        /// <summary>کلیک روی آیکون فیلتر وضعیت حساب</summary>
        private void AccountStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            var options = AllPersons?
                .Select(p => p.AccountStatus)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "—")
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            ShowFilterPopup(
                anchor: sender as Button,
                title: "فیلتر وضعیت حساب",
                options: options,
                selected: _selectedAccountStatuses,
                showSearch: false,
                immediateApply: true,
                onSelectionChanged: result =>
                {
                    _selectedAccountStatuses.Clear();
                    foreach (var r in result) _selectedAccountStatuses.Add(r);
                    _currentPage = 1;
                    ApplyFilters();
                });
        }

        /// <summary>
        /// راه‌اندازی FilterPopupControl و نمایش آن.
        /// همه‌ی منطق پاپ‌آپ (ظاهر، سرچ، چک‌باکس‌ها، دکمه‌ها) داخل UserControl قرار داره.
        /// </summary>
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
        private string ToPersianNumber(long number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";
            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];
            return result;
        }

        private string ToPersianNumber(int number) => ToPersianNumber((long)number);

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

        private void ActionButton_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ActionButton_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }

    // ======================================================
    //  PersonItem (مدل ردیف DataGrid)
    // ======================================================
    public class PersonItem : INotifyPropertyChanged
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

        // ★ فیلدهای جدید برای ستون‌های جدید گرید
        public string LastTransaction { get; set; } = "—";
        public bool IsLegal { get; set; }
        public string LegalStatus => IsEmpty ? "" : IsLegal ? "حقوقی" : "حقیقی";
        public string TransactionType { get; set; } = "—";
        public string TransactionDate { get; set; } = "—";
        public string BalanceDisplay { get; set; } = "—";

        public string FullName =>
            IsEmpty ? "" : !string.IsNullOrWhiteSpace(FullNameText)
                ? FullNameText
                : $"{FirstName} {LastName}".Trim();

        public string RowNumberDisplay =>
            RowNumber > 0 && !IsEmpty ? ToPersianNumber(RowNumber) : "";

        public Visibility StatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(Status) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility AccountStatusVisibility =>
            IsEmpty || string.IsNullOrEmpty(AccountStatus) || AccountStatus == "—"
                ? Visibility.Collapsed : Visibility.Visible;

        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
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