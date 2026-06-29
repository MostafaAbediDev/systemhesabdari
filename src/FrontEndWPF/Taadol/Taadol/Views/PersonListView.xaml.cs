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
        private string _currentFilter = "all";
        private bool _isLoadedOnce = false;

        // ===== Filter Selections =====
        // برای فیلتر پاپ‌آپ: مقادیر انتخاب‌شده توسط کاربر
        // اگه خالی باشه = هیچ فیلتری اعمال نشده (همه نشون داده می‌شه)
        private readonly HashSet<string> _selectedStatuses = new HashSet<string>();
        private readonly HashSet<string> _selectedProvinces = new HashSet<string>();
        private readonly HashSet<string> _selectedCities = new HashSet<string>();

        public PersonListView()
        {
            InitializeComponent();
            FillEmptyRows();

            SearchBox.TextChanged += (s, e) =>
            {
                _currentPage = 1;
                ApplyFilters();
            };

            Loaded += PersonListView_Loaded;
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
                    var contactApp = sp.GetRequiredService<IPersonContactApplication>();
                    var addressApp = sp.GetRequiredService<IPersonAddressApplication>();

                    var persons = personApp.GetPersons() ?? new List<PersonViewModel>();

                    // کش از تماس‌ها و آدرس‌ها برای همه‌ی اشخاص (N+1 جلوگیری)
                    var personIds = persons.Select(p => p.Id).ToList();
                    var allContacts = new List<PersonContactViewModel>();
                    var allAddresses = new List<PersonAddressViewModel>();

                    foreach (var pid in personIds)
                    {
                        try { allContacts.AddRange(contactApp.GetByPersonId(pid) ?? new List<PersonContactViewModel>()); } catch { }
                        try { allAddresses.AddRange(addressApp.GetByPersonId(pid) ?? new List<PersonAddressViewModel>()); } catch { }
                    }

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
                            AccountStatus = p.AvailableCredit > 0 ? "فعال" : "—",
                            PersonType = p.PersonType,
                            IsEmpty = false
                        };
                    }).ToList();
                });

                AllPersons = new ObservableCollection<PersonItem>(items);
                _currentPage = 1;
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

            // فیلتر نوع شخص (تب‌های بالا)
            switch (_currentFilter)
            {
                case "customer":
                    query = query.Where(p => p.PersonType != null && p.PersonType.Contains("مشتری"));
                    break;
                case "supplier":
                    query = query.Where(p => p.PersonType != null && p.PersonType.Contains("تامین"));
                    break;
                case "personnel":
                    query = query.Where(p => p.PersonType != null && p.PersonType.Contains("پرسنل"));
                    break;
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

            pages.Add(new PageItem
            {
                PageNumber = 1,
                PageNumberDisplay = ToPersianNumber(1),
                IsCurrent = _currentPage == 1
            });

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

            if (middleStart > 2)
            {
                pages.Add(new PageItem
                {
                    PageNumber = 0,
                    PageNumberDisplay = "...",
                    IsCurrent = false
                });
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

            if (middleEnd < _totalPages - 1)
            {
                pages.Add(new PageItem
                {
                    PageNumber = 0,
                    PageNumberDisplay = "...",
                    IsCurrent = false
                });
            }

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
        private void FilterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                _currentFilter = "all";

                if (rb == tabCustomers) _currentFilter = "customer";
                else if (rb == tabSuppliers) _currentFilter = "supplier";
                else if (rb == tabPersonnel) _currentFilter = "personnel";

                _currentPage = 1;

                if (AllPersons != null)
                    ApplyFilters();
            }
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
                using var scope = App.ServiceProvider.CreateScope();
                var personApp = scope.ServiceProvider.GetRequiredService<IPersonApplication>();

                foreach (var item in selectedItems)
                {
                    var op = personApp.Remove(item.Id);
                    if (!op.IsSucceeded)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"Failed to delete person {item.Id}: {op.Message}");
                    }
                }
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
                // باز کردن فرم ویرایش شخص در MainWindow
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    // استفاده از reflection یا متد عمومی برای دسترسی به MainContent
                    // فعلاً NewPersonView رو در حالت ویرایش می‌سازیم
                    var editView = new NewPersonView();
                    editView.LoadPerson(item.Id);

                    // پیدا کردن ContentControl اصلی در MainWindow و قرار دادن فرم
                    var mainContent = mainWindow.FindName("MainContent") as ContentControl;
                    if (mainContent != null)
                    {
                        mainContent.Content = editView;

                        var mainContentBorder = mainWindow.FindName("MainContentBorder") as Border;
                        if (mainContentBorder != null)
                            mainContentBorder.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show($"ویرایش شخص: {item.FullName} (ID: {item.Id})",
                            "ویرایش شخص", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"ویرایش شخص: {item.FullName} (ID: {item.Id})",
                        "ویرایش شخص", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

        private void PersonsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

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
        private string ToPersianNumber(int number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";
            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];
            return result;
        }

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
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
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
