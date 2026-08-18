using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Application.Contract.Persons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Taadol.Models;
using Taadol.Views;

namespace Taadol.ViewModels
{
    public partial class PersonListViewModel : ObservableObject
    {
        // فرهنگ ثابت fa-IR برای فرمت مبلغ: رقم فارسی + جداکننده هزارگان «٬»
        // (به فرهنگ سیستم وابسته نیست تا روی ویندوز انگلیسی «،» لاتین نمایش ندهد)
        private static readonly CultureInfo AmountCulture = CultureInfo.GetCultureInfo("fa-IR");

        private readonly IServiceProvider _serviceProvider;

        private int _totalPages = 1;
        private int _lastFilteredListCount;

        // کش حساب‌های بانکی هر شخص — در LoadDataAsync همراه تماس‌ها و آدرس‌ها پر می‌شود
        private Dictionary<long, List<PersonBankViewModel>> _bankAccountsByPerson = new();

        // کش تماس‌ها و آدرس‌ها — برای پنل جزئیات (ایمیل، آدرس کامل)
        private Dictionary<long, List<PersonContactViewModel>> _contactsByPerson = new();
        private Dictionary<long, PersonAddressViewModel> _addressesByPerson = new();

        public PersonListViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            AllPersons = new ObservableCollection<PersonItem>();
            FilteredPersons = new ObservableCollection<PersonItem>();
            Pages = new ObservableCollection<PageItem>();
            SelectedTabs = new HashSet<string> { "all" };
            SelectedStatuses = new HashSet<string>();
            SelectedProvinces = new HashSet<string>();
            SelectedCities = new HashSet<string>();
            SelectedLegalStatuses = new HashSet<string>();
            SelectedAccountStatuses = new HashSet<string>();
        }

        [ObservableProperty]
        private ObservableCollection<PersonItem> _allPersons;

        [ObservableProperty]
        private ObservableCollection<PersonItem> _filteredPersons;

        [ObservableProperty]
        private ObservableCollection<PageItem> _pages;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 15;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _pageInfoText = string.Empty;

        [ObservableProperty]
        private string _totalDebitText = string.Empty;

        [ObservableProperty]
        private string _totalCreditText = string.Empty;

        [ObservableProperty]
        private string _selectedSummaryText = string.Empty;

        [ObservableProperty]
        private string _selectedTotalText = string.Empty;

        [ObservableProperty]
        private string _selectedTotalTextColor = "#2563EB";

        [ObservableProperty]
        private string _selectedCountText = string.Empty;

        public HashSet<string> SelectedTabs { get; }
        public HashSet<string> SelectedStatuses { get; }
        public HashSet<string> SelectedProvinces { get; }
        public HashSet<string> SelectedCities { get; }
        public HashSet<string> SelectedLegalStatuses { get; }
        public HashSet<string> SelectedAccountStatuses { get; }

        public List<PersonItem> GetSelectedItems()
        {
            return AllPersons?.Where(p => p.IsSelected && !p.IsEmpty).ToList() ?? new List<PersonItem>();
        }

        public void SelectAll()
        {
            if (FilteredPersons == null) return;
            foreach (var p in FilteredPersons.Where(p => !p.IsEmpty))
                p.IsSelected = true;
        }

        public void DeselectAll()
        {
            if (FilteredPersons == null) return;
            foreach (var p in FilteredPersons.Where(p => !p.IsEmpty))
                p.IsSelected = false;
        }

        public List<PersonItem> GetAllSelectedItems()
        {
            return AllPersons?.Where(p => !p.IsEmpty).ToList() ?? new List<PersonItem>();
        }

        /// <summary>
        /// حساب‌های بانکی کش‌شده در LoadDataAsync — بدون کوئری دیتابیس.
        /// </summary>
        public List<PersonBankViewModel> GetBankAccounts(long personId)
        {
            return _bankAccountsByPerson.TryGetValue(personId, out var list)
                ? list
                : new List<PersonBankViewModel>();
        }

        /// <summary>
        /// ایمیل شخص از تماس‌های کش‌شده (نوع تماس شامل «ایمیل») — null اگر موجود نباشد.
        /// </summary>
        public string GetEmail(long personId)
        {
            if (!_contactsByPerson.TryGetValue(personId, out var contacts))
                return null;

            var email = contacts.FirstOrDefault(c =>
                c.ContactTypeTitle != null &&
                c.ContactTypeTitle.Contains("ایمیل", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(c.Value));

            return email?.Value;
        }

        /// <summary>
        /// آدرس از آدرس‌های کش‌شده — null اگر موجود نباشد.
        /// </summary>
        public string GetAddress(long personId)
        {
            if (!_addressesByPerson.TryGetValue(personId, out var address) || address == null)
                return null;

            return string.IsNullOrWhiteSpace(address.Address) ? null : address.Address.Trim();
        }

        /// <summary>
        /// کد پستی از آدرس‌های کش‌شده — null اگر موجود نباشد.
        /// </summary>
        public string GetPostalCode(long personId)
        {
            if (!_addressesByPerson.TryGetValue(personId, out var address) || address == null)
                return null;

            return string.IsNullOrWhiteSpace(address.PostalCode) ? null : address.PostalCode.Trim();
        }

        public async Task LoadDataAsync()
        {
            IsLoading = true;

            // انتخاب‌های فعلی را حفظ کن تا بعد از بازسازی لیست (مثلاً بعد از حذف یک شخص)
            // بقیه‌ی آیتم‌های انتخابی از حالت انتخاب خارج نشوند.
            var selectedIds = AllPersons?
                .Where(p => p.IsSelected && !p.IsEmpty)
                .Select(p => p.Id)
                .ToHashSet() ?? new HashSet<long>();

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sp = scope.ServiceProvider;

                var personApp = sp.GetRequiredService<IPersonApplication>();
                var persons = (personApp.GetPersons() ?? new List<PersonViewModel>())
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();
                var personIds = persons.Select(p => p.Id).ToList();

                var allContacts = new List<PersonContactViewModel>();
                var allAddresses = new List<PersonAddressViewModel>();

                var contactTasks = personIds.Select(pid =>
                    Task.Run(() =>
                    {
                        using var s = _serviceProvider.CreateScope();
                        var app = s.ServiceProvider.GetRequiredService<IPersonContactApplication>();
                        try { return app.GetByPersonId(pid) ?? new List<PersonContactViewModel>(); }
                        catch { return new List<PersonContactViewModel>(); }
                    })).ToList();

                var addressTasks = personIds.Select(pid =>
                    Task.Run(() =>
                    {
                        using var s = _serviceProvider.CreateScope();
                        var app = s.ServiceProvider.GetRequiredService<IPersonAddressApplication>();
                        try { return app.GetByPersonId(pid) ?? new List<PersonAddressViewModel>(); }
                        catch { return new List<PersonAddressViewModel>(); }
                    })).ToList();

                var bankTasks = personIds.Select(pid =>
                    Task.Run(() =>
                    {
                        using var s = _serviceProvider.CreateScope();
                        var app = s.ServiceProvider.GetRequiredService<IPersonBankApplication>();
                        try { return app.GetByPersonId(pid) ?? new List<PersonBankViewModel>(); }
                        catch { return new List<PersonBankViewModel>(); }
                    })).ToList();

                var allTasks = new List<Task>(contactTasks.Count + addressTasks.Count + bankTasks.Count);
                allTasks.AddRange(contactTasks);
                allTasks.AddRange(addressTasks);
                allTasks.AddRange(bankTasks);
                await Task.WhenAll(allTasks);

                foreach (var t in contactTasks) allContacts.AddRange(await t);
                foreach (var t in addressTasks) allAddresses.AddRange(await t);

                _bankAccountsByPerson = new Dictionary<long, List<PersonBankViewModel>>();
                for (int i = 0; i < personIds.Count; i++)
                    _bankAccountsByPerson[personIds[i]] = await bankTasks[i];

                var contactsByPerson = allContacts
                    .GroupBy(c => c.PersonId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var addressesByPerson = allAddresses
                    .GroupBy(a => a.PersonId)
                    .ToDictionary(g => g.Key, g => g.FirstOrDefault());

                _contactsByPerson = contactsByPerson;
                _addressesByPerson = addressesByPerson;

                var items = persons.Select((p, index) =>
                {
                    string mobile = "—";
                    string phone = "—";

                    if (contactsByPerson.TryGetValue(p.Id, out var contacts) && contacts.Count > 0)
                    {
                        var mobileContact = contacts.FirstOrDefault(c =>
                            c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("موبایل"));
                        if (mobileContact != null && !string.IsNullOrWhiteSpace(mobileContact.Value))
                            mobile = mobileContact.Value;

                        var phoneContact = contacts.FirstOrDefault(c =>
                            c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("تلفن"));
                        if (phoneContact != null && !string.IsNullOrWhiteSpace(phoneContact.Value))
                            phone = phoneContact.Value;
                    }

                    string province = "—";
                    string city = "—";

                    if (addressesByPerson.TryGetValue(p.Id, out var address) && address != null)
                    {
                        if (!string.IsNullOrWhiteSpace(address.ProvinceName))
                            province = address.ProvinceName;
                        if (!string.IsNullOrWhiteSpace(address.CityName))
                            city = address.CityName;
                    }

                    string fullName;
                    if (p.IsLegal)
                    {
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
                        TransactionType = p.CreditLimit > 0 ? "بدهکار" : (p.AvailableCredit > 0 ? "بستانکار" : ""),
                        TransactionDate = p.CreditLimit > 0 || p.AvailableCredit > 0 ? "—" : "",
                        AccountStatus = p.CreditLimit > 0 ? "بدهکار" : (p.AvailableCredit > 0 ? "بستانکار" : "بی حساب"),
                        BalanceDisplay = p.CreditLimit > 0 ? FormatAmount(p.CreditLimit) : (p.AvailableCredit > 0 ? FormatAmount(p.AvailableCredit) : ""),
                        IsLegal = p.IsLegal,
                        PersonType = p.PersonType,
                        IsSelected = selectedIds.Contains(p.Id),
                        IsEmpty = false
                    };
                }).ToList();

                AllPersons = new ObservableCollection<PersonItem>(items);
                CurrentPage = 1;
                UpdateTabCounts();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                // خطا را بی‌صدا نبلع: اگر رفرش بعد از ثبت شکست بخورد،
                // لیست قبلی حفظ می‌شود و کاربر پیام می‌گیرد (به‌جای «۰ از ۰» گمراه‌کننده).
                System.Diagnostics.Debug.WriteLine($"[ERROR] PersonListViewModel.LoadDataAsync: {ex}");

                if (AllPersons == null || AllPersons.Count == 0)
                {
                    AllPersons = new ObservableCollection<PersonItem>();
                    ApplyFilters();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ApplyFilters()
        {
            if (AllPersons == null) return;

            var query = AllPersons.AsEnumerable();

            if (!SelectedTabs.Contains("all"))
            {
                var tabFilters = new List<string>();
                if (SelectedTabs.Contains("customer")) tabFilters.Add("مشتری");
                if (SelectedTabs.Contains("supplier")) tabFilters.Add("تامین");
                if (SelectedTabs.Contains("personnel")) tabFilters.Add("پرسنل");

                if (tabFilters.Count > 0)
                {
                    query = query.Where(p => p.PersonType != null &&
                        tabFilters.Any(f => p.PersonType.Contains(f)));
                }
            }

            if (SelectedStatuses.Count > 0)
                query = query.Where(p => p.Status != null && SelectedStatuses.Contains(p.Status));

            if (SelectedProvinces.Count > 0)
                query = query.Where(p => p.Province != null && p.Province != "—" && SelectedProvinces.Contains(p.Province));

            if (SelectedCities.Count > 0)
                query = query.Where(p => p.City != null && p.City != "—" && SelectedCities.Contains(p.City));

            if (SelectedLegalStatuses.Count > 0)
                query = query.Where(p => p.LegalStatus != null && SelectedLegalStatuses.Contains(p.LegalStatus));

            if (SelectedAccountStatuses.Count > 0)
                query = query.Where(p => p.AccountStatus != null && p.AccountStatus != "—" && SelectedAccountStatuses.Contains(p.AccountStatus));

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.Trim();
                query = query.Where(p =>
                    (!string.IsNullOrWhiteSpace(p.Code) && p.Code.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(p.FullName) && p.FullName.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(p.NationalId) && p.NationalId.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(p.EconomicId) && p.EconomicId.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(p.Company) && p.Company.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(p.Mobile) && p.Mobile.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(p.Phone) && p.Phone.Contains(s))
                );
            }

            var filteredList = query.ToList();
            _lastFilteredListCount = filteredList.Count;

            _totalPages = (int)Math.Ceiling(filteredList.Count / (double)PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (CurrentPage > _totalPages) CurrentPage = _totalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            var pageItems = filteredList
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            for (int i = 0; i < pageItems.Count; i++)
                pageItems[i].RowNumber = ((CurrentPage - 1) * PageSize) + i + 1;

            FilteredPersons = new ObservableCollection<PersonItem>(pageItems);

            BuildPages();
            UpdatePageInfo();
            UpdateSummaryBar();
        }

        private void BuildPages()
        {
            var pages = new ObservableCollection<PageItem>();

            if (_totalPages <= 5)
            {
                for (int i = 1; i <= _totalPages; i++)
                {
                    pages.Add(new PageItem
                    {
                        PageNumber = i,
                        PageNumberDisplay = ToPersianNumber(i),
                        IsCurrent = i == CurrentPage
                    });
                }
                Pages = pages;
                return;
            }

            pages.Add(new PageItem
            {
                PageNumber = 1,
                PageNumberDisplay = ToPersianNumber(1),
                IsCurrent = CurrentPage == 1
            });

            pages.Add(new PageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });

            int middleStart = CurrentPage - 1;
            int middleEnd = CurrentPage + 1;

            if (CurrentPage <= 3)
            {
                middleStart = 2;
                middleEnd = 4;
            }
            else if (CurrentPage >= _totalPages - 2)
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
                        IsCurrent = i == CurrentPage
                    });
                }
            }

            pages.Add(new PageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });

            pages.Add(new PageItem
            {
                PageNumber = _totalPages,
                PageNumberDisplay = ToPersianNumber(_totalPages),
                IsCurrent = CurrentPage == _totalPages
            });

            Pages = pages;
        }

        private void UpdateTabCounts()
        {
            if (AllPersons == null) return;

            var validPersons = AllPersons.Where(p => !p.IsEmpty).ToList();
            int total = validPersons.Count;
            int customers = validPersons.Count(p => p.PersonType != null && p.PersonType.Contains("مشتری"));
            int suppliers = validPersons.Count(p => p.PersonType != null && p.PersonType.Contains("تامین"));
            int personnel = validPersons.Count(p => p.PersonType != null && p.PersonType.Contains("پرسنل"));

            TabAllCount = $"( {ToPersianNumber(total)} )";
            TabCustomersCount = $"( {ToPersianNumber(customers)} )";
            TabSuppliersCount = $"( {ToPersianNumber(suppliers)} )";
            TabPersonnelCount = $"( {ToPersianNumber(personnel)} )";
        }

        [ObservableProperty]
        private string _tabAllCount = string.Empty;

        [ObservableProperty]
        private string _tabCustomersCount = string.Empty;

        [ObservableProperty]
        private string _tabSuppliersCount = string.Empty;

        [ObservableProperty]
        private string _tabPersonnelCount = string.Empty;

        private void UpdatePageInfo()
        {
            int currentPageCount = FilteredPersons.Count(p => !p.IsEmpty);
            PageInfoText = $"نمایش {ToPersianNumber(currentPageCount)} از {ToPersianNumber(_lastFilteredListCount)} مورد";
        }

        public void UpdateSummaryBar()
        {
            if (AllPersons == null) return;

            var validPersons = AllPersons.Where(p => !p.IsEmpty).ToList();
            var selectedItems = validPersons.Where(p => p.IsSelected).ToList();

            long totalDebit = 0;
            long totalCredit = 0;
            foreach (var p in validPersons)
            {
                if (TryParseAmount(p.BalanceDisplay, out long bal))
                {
                    if (p.AccountStatus == "بدهکار")
                        totalDebit += bal;
                    else if (p.AccountStatus == "بستانکار")
                        totalCredit += bal;
                }
            }

            TotalDebitText = $"{FormatAmount(totalDebit)} ریال";
            TotalCreditText = $"{FormatAmount(totalCredit)} ریال";

            long selectedDebit = 0;
            long selectedCredit = 0;
            foreach (var p in selectedItems)
            {
                if (TryParseAmount(p.BalanceDisplay, out long bal))
                {
                    if (p.AccountStatus == "بدهکار")
                        selectedDebit += bal;
                    else if (p.AccountStatus == "بستانکار")
                        selectedCredit += bal;
                }
            }

            long selectedNet = selectedDebit - selectedCredit;
            SelectedSummaryText = $"جمع اشخاص انتخاب شده ({ToPersianNumber(selectedItems.Count)})";
            if (selectedDebit > selectedCredit)
                SelectedTotalTextColor = "#22C55E";
            else if (selectedCredit > selectedDebit)
                SelectedTotalTextColor = "#DC2626";
            else
                SelectedTotalTextColor = "#374151";
            SelectedTotalText = $"{FormatAmount(Math.Abs(selectedNet))} ریال";
            SelectedCountText = $"({selectedItems.Count})";
        }

        /// <summary>
        /// حذف گروهی اشخاص انتخاب‌شده.
        /// نتیجه هر حذف جمع‌آوری می‌شود تا کاربر نتیجه واقعی (موفق/ناموفق) را ببیند،
        /// نه اینکه خطاها فقط در Debug نوشته شوند.
        /// </summary>
        public async Task<(int DeletedCount, List<string> Errors)> DeleteSelectedAsync()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0) return (0, new List<string>());

            var idsToDelete = selectedItems.Select(p => p.Id).ToList();
            var errors = new List<string>();

            await Task.Run(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var personApp = scope.ServiceProvider.GetRequiredService<IPersonApplication>();
                foreach (var id in idsToDelete)
                {
                    var op = personApp.Remove(id);
                    if (!op.IsSucceeded && !string.IsNullOrWhiteSpace(op.Message))
                        errors.Add(op.Message);
                }
            });

            await LoadDataAsync();
            return (idsToDelete.Count - errors.Count, errors);
        }

        public async Task DeletePersonAsync(PersonItem item)
        {
            if (item == null) return;

            await Task.Run(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var personApp = scope.ServiceProvider.GetRequiredService<IPersonApplication>();
                var op = personApp.Remove(item.Id);
                if (!op.IsSucceeded)
                {
                    throw new Exception(op.Message);
                }
            });

            await LoadDataAsync();
        }

        public async Task RefreshAsync()
        {
            await LoadDataAsync();
        }

        public void UpdateTabSelection(string tabName, bool isChecked)
        {
            if (isChecked)
            {
                if (tabName == "all")
                {
                    SelectedTabs.Clear();
                    SelectedTabs.Add("all");
                }
                else
                {
                    SelectedTabs.Remove("all");
                    SelectedTabs.Add(tabName);
                }
            }
            else
            {
                SelectedTabs.Remove(tabName);
                if (SelectedTabs.Count == 0)
                {
                    SelectedTabs.Add("all");
                }
            }

            CurrentPage = 1;
            ApplyFilters();
        }

        public void GoToPage(int pageNumber)
        {
            if (pageNumber <= 0) return;
            CurrentPage = pageNumber;
            ApplyFilters();
        }

        public void GoToNextPage()
        {
            if (CurrentPage < _totalPages)
            {
                CurrentPage++;
                ApplyFilters();
            }
        }

        public void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                ApplyFilters();
            }
        }

        public void ChangePageSize(int newSize)
        {
            PageSize = newSize;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void HandleSearchTextChanged(string text)
        {
            SearchText = text;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void SetFilterResult(string filterType, List<string> selected)
        {
            switch (filterType)
            {
                case "status":
                    SelectedStatuses.Clear();
                    foreach (var r in selected) SelectedStatuses.Add(r);
                    break;
                case "province":
                    SelectedProvinces.Clear();
                    foreach (var r in selected) SelectedProvinces.Add(r);
                    break;
                case "city":
                    SelectedCities.Clear();
                    foreach (var r in selected) SelectedCities.Add(r);
                    break;
                case "legal":
                    SelectedLegalStatuses.Clear();
                    foreach (var r in selected) SelectedLegalStatuses.Add(r);
                    break;
                case "account":
                    SelectedAccountStatuses.Clear();
                    foreach (var r in selected) SelectedAccountStatuses.Add(r);
                    break;
            }

            CurrentPage = 1;
            ApplyFilters();
        }

        public List<string> GetProvinceOptions()
        {
            return AllPersons?
                .Select(p => p.Province)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "—")
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();
        }

        public List<string> GetCityOptions()
        {
            return AllPersons?
                .Select(p => p.City)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "—")
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();
        }

        public List<string> GetAccountStatusOptions()
        {
            return AllPersons?
                .Select(p => p.AccountStatus)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "—")
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();
        }

        private static string ToPersianNumber(long number)
        {
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string result = "";
            foreach (char c in number.ToString())
                result += persianDigits[int.Parse(c.ToString())];
            return result;
        }

        /// <summary>رقم‌های انگلیسی داخل رشته را به فارسی تبدیل می‌کند؛ بقیه دست‌نخورده می‌ماند.</summary>
        private static string ToPersianDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                result[i] = c >= '0' && c <= '9' ? (char)('۰' + (c - '0')) : c;
            }
            return new string(result);
        }

        /// <summary>
        /// فرمت ثابت مبلغ: رقم فارسی + جداکننده هزارگان «٬» (مستقل از فرهنگ سیستم).
        /// مثال: 1234567 → «۱٬۲۳۴٬۵۶۷»
        /// رقم‌ها همین‌جا فارسی می‌شوند (نه فقط در لایه رندر) تا سلول‌هایی که رفتار
        /// NumberFontBehavior را ندارند (Run صریح) هم رقم فارسی نشان بدهند.
        /// </summary>
        private static string FormatAmount(decimal value)
            => ToPersianDigits(value.ToString("N0", AmountCulture));

        /// <summary>
        /// پارس مبلغ نمایش‌داده‌شده (رقم فارسی/لاتین و جداکننده «٬» یا «،» لاتین را می‌پذیرد).
        /// </summary>
        private static bool TryParseAmount(string text, out long value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var normalized = ToAsciiDigits(text).Replace("٬", "").Replace(",", "").Trim();
            return long.TryParse(normalized, out value);
        }

        /// <summary>رقم‌های فارسی داخل رشته را به انگلیسی برمی‌گرداند (برای Parse عددی).</summary>
        private static string ToAsciiDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                result[i] = c >= '۰' && c <= '۹' ? (char)('0' + (c - '۰')) : c;
            }
            return new string(result);
        }

        private static string ToPersianNumber(int number) => ToPersianNumber((long)number);
    }
}
