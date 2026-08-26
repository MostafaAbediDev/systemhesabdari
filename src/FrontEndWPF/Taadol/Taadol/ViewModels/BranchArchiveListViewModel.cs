using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneralInfoManagement.Application.Contract.BranchArchice;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taadol.Controls;
using Taadol.Models;

namespace Taadol.ViewModels
{
    public partial class BranchArchiveListViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private int _totalPages = 1;
        private int _lastFilteredListCount;
        private int _loadRequestVersion;
        private string _lastAppliedFilterKey;
        private string _cachedFilterKey;
        private List<BranchArchiveItem> _cachedFilteredItems;

        public BranchArchiveListViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            AllArchives = new ObservableCollection<BranchArchiveItem>();
            FilteredArchives = new ObservableCollection<BranchArchiveItem>();
            Pages = new ObservableCollection<ArchivePageItem>();
        }

        [ObservableProperty] private ObservableCollection<BranchArchiveItem> _allArchives;
        [ObservableProperty] private ObservableCollection<BranchArchiveItem> _filteredArchives;
        [ObservableProperty] private ObservableCollection<ArchivePageItem> _pages;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private int _pageSize = 15;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _loadErrorText = string.Empty;
        [ObservableProperty] private string _pageInfoText = string.Empty;
        [ObservableProperty] private string _selectedCountText = string.Empty;
        [ObservableProperty] private string _totalCountText = string.Empty;
        [ObservableProperty] private string _selectedSummaryText = string.Empty;

        [ObservableProperty] private ObservableCollection<CompanyFilterItem> _companyItems = new();
        [ObservableProperty] private CompanyFilterItem _selectedCompany;
        [ObservableProperty] private ObservableCollection<BranchFilterItem> _branchItems = new();
        [ObservableProperty] private BranchFilterItem _selectedBranch;

        private List<long> _companyBranchIds = new();

        // تبدیل تاریخ میلادی دریافتی از سرویس به تاریخ شمسی (همان الگوی لیست‌های دیگر)
        private string ToPersianDate(object dateValue)
        {
            if (dateValue == null)
                return "—";

            DateTime date;
            if (dateValue is DateTime dt)
            {
                date = dt;
            }
            else
            {
                if (!DateTime.TryParse(dateValue.ToString(), out date))
                    return "—";
            }

            PersianCalendar pc = new PersianCalendar();
            int year = pc.GetYear(date);
            int month = pc.GetMonth(date);
            int day = pc.GetDayOfMonth(date);

            return $"{year:0000}/{month:00}/{day:00}";
        }

        public List<BranchArchiveItem> GetSelectedItems()
        {
            return AllArchives?.Where(p => p.IsSelected && !p.IsEmpty).ToList() ?? new List<BranchArchiveItem>();
        }

        public void SelectAll()
        {
            if (FilteredArchives == null) return;
            foreach (var p in FilteredArchives.Where(p => !p.IsEmpty))
                p.IsSelected = true;
        }

        public void DeselectAll()
        {
            if (FilteredArchives == null) return;
            foreach (var p in FilteredArchives.Where(p => !p.IsEmpty))
                p.IsSelected = false;
        }

        public void UpdateSelectedCount()
        {
            var selected = AllArchives?.Count(p => p.IsSelected && !p.IsEmpty) ?? 0;
            SelectedCountText = $"({ToPersianNumber(selected)})";
            SelectedSummaryText = $"آرشیو انتخاب شده ({ToPersianNumber(selected)})";
        }

        public void UpdateTotalCount()
        {
            var total = AllArchives?.Count(p => !p.IsEmpty) ?? 0;
            TotalCountText = $"{ToPersianNumber(total)} مورد";
        }

        public async Task LoadDataAsync()
        {
            var requestVersion = Interlocked.Increment(ref _loadRequestVersion);
            _lastAppliedFilterKey = null;
            _cachedFilterKey = null;
            _cachedFilteredItems = null;
            IsLoading = true;
            LoadErrorText = null;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var archiveApp = scope.ServiceProvider.GetRequiredService<IBranchArchiveApplication>();
                var branchApp = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                var companyApp = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();

                // خطای هر منبع ضروری باید به حالت Error برسد؛ تبدیل خطا به لیست خالی
                // باعث می‌شد کاربر پیام Empty ببیند و علت واقعی بارگذاری پنهان بماند.
                var archives = await Task.Run(() =>
                    archiveApp.GetBranchArchives() ?? new List<BranchArchiveViewModel>());

                var branches = await Task.Run(() =>
                    branchApp.GetBranches() ?? new List<BranchViewModel>());

                var companies = await Task.Run(() =>
                    companyApp.GetCompanies() ?? new List<CompanyViewModel>());

                CompanyItems = new ObservableCollection<CompanyFilterItem>(
                    companies.Select(c => new CompanyFilterItem { Id = c.Id, Title = c.Title }));

                var branchDict = branches.ToDictionary(b => b.Id);
                var companyDict = companies.ToDictionary(c => c.Id);

                var items = archives.Select((a, index) =>
                {
                    string companyName = "—";
                    if (branchDict.TryGetValue(a.BranchId, out var br) && companyDict.TryGetValue(br.CompanyId, out var comp))
                        companyName = comp.Title;

                    string branchName = a.BranchTitle;
                    if (string.IsNullOrWhiteSpace(branchName) && branchDict.TryGetValue(a.BranchId, out var br2))
                        branchName = br2.Title;

                    return new BranchArchiveItem
                    {
                        Id = a.Id,
                        RowNumber = index + 1,
                        Title = string.IsNullOrWhiteSpace(a.Title) ? "—" : a.Title,
                        Description = string.IsNullOrWhiteSpace(a.Description) ? "—" : a.Description,
                        File = string.IsNullOrWhiteSpace(a.File) ? "—" : a.File,
                        BranchTitle = string.IsNullOrWhiteSpace(branchName) ? "—" : branchName,
                        CompanyName = string.IsNullOrWhiteSpace(companyName) ? "—" : companyName,
                        CreationDate = string.IsNullOrWhiteSpace(a.CreationDate) ? "—" : ToPersianDate(a.CreationDate),
                        IsEmpty = false
                    };
                }).ToList();

                if (requestVersion != Volatile.Read(ref _loadRequestVersion))
                    return;

                AllArchives = new ObservableCollection<BranchArchiveItem>(items);
                CurrentPage = 1;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                if (requestVersion != Volatile.Read(ref _loadRequestVersion))
                    return;

                LoadErrorText = "خطا در بارگذاری آرشیو شعبه";
                ToastManager.Error("خطا در لود آرشیو شعبه: " + ex.Message);
                AllArchives = new ObservableCollection<BranchArchiveItem>();
                ApplyFilters();
            }
            finally
            {
                if (requestVersion == Volatile.Read(ref _loadRequestVersion))
                    IsLoading = false;
            }
        }

        public void OnCompanyChanged()
        {
            if (SelectedCompany == null)
            {
                BranchItems = new ObservableCollection<BranchFilterItem>();
                SelectedBranch = null;
                _companyBranchIds = new List<long>();
                ApplyFilters();
                return;
            }

            _ = LoadBranchesForCompanyAsync();
        }

        private async Task LoadBranchesForCompanyAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var branchApp = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                var branches = await Task.Run(() => branchApp.GetBranches())
                    ?? new List<BranchViewModel>();
                var filtered = branches.Where(b => b.CompanyId == SelectedCompany.Id).ToList();
                _companyBranchIds = filtered.Select(b => b.Id).ToList();

                BranchItems = new ObservableCollection<BranchFilterItem>(
                    filtered.Select(b => new BranchFilterItem { Id = b.Id, Title = b.Title }));
                SelectedBranch = null;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BranchArchive OnCompanyChanged failed: " + ex.Message);
            }
        }

        public void OnBranchChanged()
        {
            ApplyFilters();
        }

        public void ApplyFilters()
        {
            if (AllArchives == null) return;

            var filterKey = BuildFilterKey();
            if (filterKey == _lastAppliedFilterKey)
                return;

            var filterOnlyKey = BuildFilterOnlyKey();
            List<BranchArchiveItem> filteredList;
            if (filterOnlyKey == _cachedFilterKey && _cachedFilteredItems != null)
            {
                filteredList = _cachedFilteredItems;
            }
            else
            {
                var query = AllArchives.AsEnumerable();

            if (SelectedCompany != null && _companyBranchIds.Count > 0)
                query = query.Where(a => _companyBranchIds.Contains(a.BranchId));

            if (SelectedBranch != null)
                query = query.Where(a => a.BranchId == SelectedBranch.Id);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.Trim();
                query = query.Where(a =>
                    (!string.IsNullOrWhiteSpace(a.Title) && a.Title.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(a.BranchTitle) && a.BranchTitle.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(a.CompanyName) && a.CompanyName.Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(a.Description) && a.Description.Contains(s))
                );
            }

                filteredList = query.ToList();
                _cachedFilterKey = filterOnlyKey;
                _cachedFilteredItems = filteredList;
            }

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

            FilteredArchives = new ObservableCollection<BranchArchiveItem>(pageItems);

            BuildPages();
            UpdatePageInfo();
            UpdateSelectedCount();
            UpdateTotalCount();
            _lastAppliedFilterKey = filterKey;
        }

        private string BuildFilterOnlyKey()
        {
            return string.Join("|", SearchText,
                SelectedCompany?.Id ?? 0,
                SelectedBranch?.Id ?? 0);
        }

        private string BuildFilterKey()
        {
            return string.Join("|", CurrentPage, PageSize, SearchText,
                SelectedCompany?.Id ?? 0,
                SelectedBranch?.Id ?? 0);
        }

        private void BuildPages()
        {
            var pages = new ObservableCollection<ArchivePageItem>();
            if (_totalPages <= 5)
            {
                for (int i = 1; i <= _totalPages; i++)
                    pages.Add(new ArchivePageItem { PageNumber = i, PageNumberDisplay = ToPersianNumber(i), IsCurrent = i == CurrentPage });
                Pages = pages;
                return;
            }

            pages.Add(new ArchivePageItem { PageNumber = 1, PageNumberDisplay = ToPersianNumber(1), IsCurrent = CurrentPage == 1 });
            pages.Add(new ArchivePageItem { PageNumber = 0, PageNumberDisplay = "...", IsCurrent = false });

            int middleStart = CurrentPage - 1;
            int middleEnd = CurrentPage + 1;
            if (CurrentPage <= 3) { middleStart = 2; middleEnd = 4; }
            else if (CurrentPage >= _totalPages - 2) { middleStart = _totalPages - 3; middleEnd = _totalPages - 1; }

            for (int i = middleStart; i <= middleEnd; i++)
            {
                if (i > 1 && i < _totalPages)
                    pages.Add(new ArchivePageItem { PageNumber = i, PageNumberDisplay = ToPersianNumber(i), IsCurrent = i == CurrentPage });
            }

            pages.Add(new ArchivePageItem { PageNumber = 0, PageNumberDisplay = "...", IsCurrent = false });
            pages.Add(new ArchivePageItem { PageNumber = _totalPages, PageNumberDisplay = ToPersianNumber(_totalPages), IsCurrent = CurrentPage == _totalPages });
            Pages = pages;
        }

        private void UpdatePageInfo()
        {
            int currentPageCount = FilteredArchives.Count(p => !p.IsEmpty);
            PageInfoText = $"نمایش {ToPersianNumber(currentPageCount)} از {ToPersianNumber(_lastFilteredListCount)} مورد";
        }

        public async Task AddNewArchiveAsync(string title, string description, string filePath, long branchId)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                ToastManager.Warning("ابتدا یک فایل انتخاب کنید.");
                return;
            }

            await Task.Run(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var app = scope.ServiceProvider.GetRequiredService<IBranchArchiveApplication>();
                app.Create(new CreateBranchArchive
                {
                    Title = string.IsNullOrWhiteSpace(title) ? Path.GetFileName(filePath) : title.Trim(),
                    Description = description ?? "",
                    File = filePath,
                    BranchId = branchId
                });
            });

            await LoadDataAsync();
        }

        public async Task DeleteSelectedAsync()        {
            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0) return;

            var idsToDelete = selectedItems.Select(p => p.Id).ToList();
            await Task.Run(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var app = scope.ServiceProvider.GetRequiredService<IBranchArchiveApplication>();
                foreach (var id in idsToDelete)
                    app.Remove(id);
            });
            await LoadDataAsync();
        }

        public async Task RefreshAsync() => await LoadDataAsync();

        public void GoToPage(int pageNumber) { if (pageNumber > 0) { CurrentPage = pageNumber; ApplyFilters(); } }
        public void GoToNextPage() { if (CurrentPage < _totalPages) { CurrentPage++; ApplyFilters(); } }
        public void GoToPreviousPage() { if (CurrentPage > 1) { CurrentPage--; ApplyFilters(); } }
        public void ChangePageSize(int newSize) { PageSize = newSize; CurrentPage = 1; ApplyFilters(); }
        public void HandleSearchTextChanged(string text) { SearchText = text; CurrentPage = 1; ApplyFilters(); }

        private static string ToPersianNumber(long number)
        {
            string[] pd = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string r = "";
            foreach (char c in number.ToString()) r += pd[int.Parse(c.ToString())];
            return r;
        }
        private static string ToPersianNumber(int number) => ToPersianNumber((long)number);
    }

    // ─── Models ───
    public class BranchArchiveItem : INotifyPropertyChanged, IListRowItem
    {
        private int _rowNumber;
        private bool _isSelected;

        public long Id { get; set; }
        public long BranchId { get; set; }

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

        public string Title { get; set; } = "—";
        public string Description { get; set; } = "—";
        public string File { get; set; } = "—";
        public string BranchTitle { get; set; } = "—";
        public string CompanyName { get; set; } = "—";
        public string CreationDate { get; set; } = "—";
        public bool IsEmpty { get; set; }

        public string FileIconSource
        {
            get
            {
                if (string.IsNullOrEmpty(File) || File == "—")
                    return "/Assets/Icons/add_doc.svg";
                var ext = System.IO.Path.GetExtension(File)?.ToLower() ?? "";
                return ext switch
                {
                    ".pdf" => "/Assets/Icons/add_doc.svg",
                    ".doc" or ".docx" => "/Assets/Icons/add_doc.svg",
                    ".xls" or ".xlsx" => "/Assets/Icons/add_doc.svg",
                    ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp" => "/Assets/Icons/add_doc.svg",
                    ".zip" or ".rar" or ".7z" => "/Assets/Icons/add_doc.svg",
                    ".exe" or ".msi" => "/Assets/Icons/add_doc.svg",
                    ".txt" => "/Assets/Icons/add_doc.svg",
                    _ => "/Assets/Icons/add_doc.svg"
                };
            }
        }

        public string RowNumberDisplay =>
            RowNumber > 0 && !IsEmpty ? ToPersianNumber(RowNumber) : "";

        public event PropertyChangedEventHandler PropertyChanged;

        private string ToPersianNumber(int number)
        {
            string[] pd = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string r = "";
            foreach (char c in number.ToString()) r += pd[int.Parse(c.ToString())];
            return r;
        }
    }

    public class ArchivePageItem
    {
        public int PageNumber { get; set; }
        public string PageNumberDisplay { get; set; } = "";
        public bool IsCurrent { get; set; }
    }

    public class CompanyFilterItem
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
    }

    public class BranchFilterItem
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
    }
}
