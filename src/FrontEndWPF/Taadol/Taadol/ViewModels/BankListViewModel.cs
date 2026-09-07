using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankManagement.Application.Contracts.Bank;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Helpers;
using Taadol.Models;

namespace Taadol.ViewModels
{
    public sealed class BankListViewModel : ObservableObject, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource? _loadCts;
        private int _disposeState;
        private int _loadRequestVersion;
        private int _totalPages = 1;
        private int _filteredListCount;
        private string _lastAppliedFilterKey = string.Empty;
        private string _cachedFilterKey = string.Empty;
        private List<BankItem>? _cachedFilteredItems;

        public BankListViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            AllBanks = new BulkObservableCollection<BankItem>();
            FilteredBanks = new BulkObservableCollection<BankItem>();
            Pages = new ObservableCollection<BankPageItem>();
            SelectedCountries = new ObservableCollection<string>();
            SelectedBankTypes = new ObservableCollection<string>();
            AvailableCountries = new ObservableCollection<string>();
            AvailableBankTypes = new ObservableCollection<string>();

            LoadBanksCommand = new AsyncRelayCommand(LoadDataAsync);
            SearchCommand = new RelayCommand(ApplyFilters);
            NextPageCommand = new RelayCommand(GoToNextPage);
            PreviousPageCommand = new RelayCommand(GoToPreviousPage);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            DeleteBankCommand = new AsyncRelayCommand(async () => await DeleteSelectedAsync());
            EditBankCommand = new RelayCommand<BankItem?>(RequestEdit);
        }

        public IAsyncRelayCommand LoadBanksCommand { get; }
        public IRelayCommand SearchCommand { get; }
        public IRelayCommand NextPageCommand { get; }
        public IRelayCommand PreviousPageCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand DeleteBankCommand { get; }
        public IRelayCommand<BankItem?> EditBankCommand { get; }
        public event Action<BankItem>? BankEditRequested;

        public BulkObservableCollection<BankItem> AllBanks { get; }
        public BulkObservableCollection<BankItem> Banks => AllBanks;
        public BulkObservableCollection<BankItem> FilteredBanks { get; }
        public ObservableCollection<BankPageItem> Pages { get; }
        public ObservableCollection<string> SelectedCountries { get; }
        public ObservableCollection<string> SelectedBankTypes { get; }
        public ObservableCollection<string> AvailableCountries { get; }
        public ObservableCollection<string> AvailableBankTypes { get; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            private set => SetProperty(ref _searchText, value);
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            private set => SetProperty(ref _currentPage, value);
        }

        private int _pageSize = 15;
        public int PageSize
        {
            get => _pageSize;
            private set => SetProperty(ref _pageSize, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        private string? _loadErrorText;
        public string? LoadErrorText
        {
            get => _loadErrorText;
            private set => SetProperty(ref _loadErrorText, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            private set => SetProperty(ref _totalPages, value);
        }

        public int TotalCount => _filteredListCount;

        private string _selectedSummaryText = "بانک انتخاب شده (۰)";
        public string SelectedSummaryText
        {
            get => _selectedSummaryText;
            private set => SetProperty(ref _selectedSummaryText, value);
        }

        private string _totalCountText = "۰ مورد";
        public string TotalCountText
        {
            get => _totalCountText;
            private set => SetProperty(ref _totalCountText, value);
        }

        private BankItem? _selectedBank;
        public BankItem? SelectedBank
        {
            get => _selectedBank;
            private set => SetProperty(ref _selectedBank, value);
        }

        public bool IsDisposed => Volatile.Read(ref _disposeState) != 0;

        public async Task LoadDataAsync()
        {
            if (IsDisposed)
                return;

            var requestVersion = Interlocked.Increment(ref _loadRequestVersion);
            var loadCts = new CancellationTokenSource();
            var previousCts = Interlocked.Exchange(ref _loadCts, loadCts);
            CancelAndDispose(previousCts);

            var token = loadCts.Token;
            var selectedIds = AllBanks
                .Where(bank => bank.IsSelected && !bank.IsEmpty)
                .Select(bank => bank.Id)
                .ToHashSet();

            _lastAppliedFilterKey = string.Empty;
            _cachedFilterKey = string.Empty;
            _cachedFilteredItems = null;
            IsLoading = true;
            LoadErrorText = null;

            try
            {
                var items = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = _serviceProvider.CreateScope();
                    var application = scope.ServiceProvider.GetRequiredService<IBankApplication>();
                    var banks = application.Search(new BankSearchModel()) ?? new List<BankViewModel>();

                    return banks.Select((bank, index) => new BankItem
                    {
                        Id = bank.Id,
                        UniqueId = bank.Id.ToString(CultureInfo.InvariantCulture),
                        RowNumber = index + 1,
                        Title = bank.Title ?? string.Empty,
                        Country = string.IsNullOrWhiteSpace(bank.Country) ? "—" : bank.Country,
                        BankType = bank.BankType ?? string.Empty,
                        RegisterDate = ToPersianDate(bank.CreationDate),
                        IsEmpty = false
                    }).ToList();
                }, token);

                token.ThrowIfCancellationRequested();
                if (requestVersion != Volatile.Read(ref _loadRequestVersion) || IsDisposed)
                    return;

                foreach (var item in items)
                    item.IsSelected = selectedIds.Contains(item.Id);

                AllBanks.ReplaceAll(items);
                AvailableCountries.Clear();
                foreach (var country in items.Select(item => item.Country).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().OrderBy(value => value))
                    AvailableCountries.Add(country);

                AvailableBankTypes.Clear();
                foreach (var bankType in items.Select(item => item.BankType).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().OrderBy(value => value))
                    AvailableBankTypes.Add(bankType);

                ApplyFilters();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // لغو بارگذاری هنگام رفرش یا خروج از صفحه، رفتار عادی است.
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[BankListViewModel] خطا در بارگذاری بانک‌ها: {exception}");
                if (requestVersion != Volatile.Read(ref _loadRequestVersion) || IsDisposed)
                    return;

                LoadErrorText = "خطا در بارگذاری بانک‌ها";
                AllBanks.ReplaceAll(Array.Empty<BankItem>());
                ApplyFilters();
                ShowErrorToast("خطا در بارگذاری بانک‌ها");
            }
            finally
            {
                if (requestVersion == Volatile.Read(ref _loadRequestVersion) && !IsDisposed)
                    IsLoading = false;

                Interlocked.CompareExchange(ref _loadCts, null, loadCts);
                loadCts.Dispose();
            }
        }

        public Task RefreshAsync()
        {
            CurrentPage = 1;
            return LoadDataAsync();
        }

        public void HandleSearchTextChanged(string? text)
        {
            SearchText = text?.Trim() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void SetCountryFilter(IEnumerable<string> countries)
        {
            SelectedCountries.Clear();
            foreach (var country in countries?.Where(value => AvailableCountries.Contains(value)).Distinct() ?? Enumerable.Empty<string>())
                SelectedCountries.Add(country);

            CurrentPage = 1;
            ApplyFilters();
        }

        public void SetBankTypeFilter(IEnumerable<string> bankTypes)
        {
            SelectedBankTypes.Clear();
            foreach (var bankType in bankTypes?.Where(value => AvailableBankTypes.Contains(value)).Distinct() ?? Enumerable.Empty<string>())
                SelectedBankTypes.Add(bankType);

            CurrentPage = 1;
            ApplyFilters();
        }

        public void GoToNextPage()
        {
            if (CurrentPage < TotalPages)
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

        public void GoToPage(int pageNumber)
        {
            if (pageNumber <= 0)
                return;

            CurrentPage = Math.Min(pageNumber, TotalPages);
            ApplyFilters();
        }

        public void ChangePageSize(int pageSize)
        {
            if (pageSize <= 0)
                return;

            PageSize = pageSize;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void SelectOnly(BankItem bank)
        {
            foreach (var item in AllBanks.Where(item => !item.IsEmpty))
                item.IsSelected = ReferenceEquals(item, bank);

            SelectedBank = bank;
            UpdateSummary();
        }

        private void RequestEdit(BankItem? bank)
        {
            if (bank != null && !bank.IsEmpty)
                BankEditRequested?.Invoke(bank);
        }

        public void ClearSelection()
        {
            foreach (var item in AllBanks.Where(item => !item.IsEmpty))
                item.IsSelected = false;

            SelectedBank = null;
            UpdateSummary();
        }

        public void SyncSelection()
        {
            SelectedBank = AllBanks.FirstOrDefault(item => item.IsSelected && !item.IsEmpty);
            UpdateSummary();
        }

        public List<BankItem> GetSelectedItems() => AllBanks
            .Where(item => item.IsSelected && !item.IsEmpty)
            .ToList();

        public async Task<(int DeletedCount, List<string> Errors)> DeleteSelectedAsync()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0)
                return (0, new List<string>());

            IsLoading = true;
            var errors = new List<string>();
            var deletedCount = 0;

            try
            {
                await Task.Run(() =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var application = scope.ServiceProvider.GetRequiredService<IBankApplication>();

                    foreach (var item in selectedItems)
                    {
                        try
                        {
                            var result = application.Remove(item.Id);
                            if (result.IsSucceeded)
                                Interlocked.Increment(ref deletedCount);
                            else
                                lock (errors) errors.Add(result.Message ?? "حذف بانک انجام نشد.");
                        }
                        catch (Exception exception)
                        {
                            lock (errors) errors.Add(GetFriendlyDeleteError(exception));
                        }
                    }
                });

                CurrentPage = 1;
                await LoadDataAsync();
                return (deletedCount, errors);
            }
            finally
            {
                if (!IsDisposed)
                    IsLoading = false;
            }
        }

        public void CancelPendingLoads()
        {
            var current = Interlocked.Exchange(ref _loadCts, null);
            CancelAndDispose(current);
            Interlocked.Increment(ref _loadRequestVersion);
        }

        private string BuildFilterKey() =>
            string.Join("|", CurrentPage, PageSize, SearchText,
                string.Join(",", SelectedCountries.OrderBy(value => value)),
                string.Join(",", SelectedBankTypes.OrderBy(value => value)));

        private void ApplyFilters()
        {
            var filterKey = BuildFilterKey();
            if (string.Equals(filterKey, _lastAppliedFilterKey, StringComparison.Ordinal))
                return;

            var selectedIds = AllBanks
                .Where(item => item.IsSelected && !item.IsEmpty)
                .Select(item => item.Id)
                .ToHashSet();

            var filterOnlyKey = string.Join("|", SearchText,
                string.Join(",", SelectedCountries.OrderBy(value => value)),
                string.Join(",", SelectedBankTypes.OrderBy(value => value)));
            List<BankItem> filteredItems;
            if (string.Equals(filterOnlyKey, _cachedFilterKey, StringComparison.Ordinal) && _cachedFilteredItems != null)
            {
                filteredItems = _cachedFilteredItems;
            }
            else
            {
                var query = AllBanks.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(bank =>
                        (bank.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (bank.Country?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (bank.BankType?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                if (SelectedCountries.Count > 0)
                    query = query.Where(bank => SelectedCountries.Contains(bank.Country));

                if (SelectedBankTypes.Count > 0)
                    query = query.Where(bank => SelectedBankTypes.Contains(bank.BankType));

                filteredItems = query.ToList();
                _cachedFilterKey = filterOnlyKey;
                _cachedFilteredItems = filteredItems;
            }

            _filteredListCount = filteredItems.Count;
            OnPropertyChanged(nameof(TotalCount));
            TotalPages = Math.Max(1, (int)Math.Ceiling(filteredItems.Count / (double)PageSize));
            CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

            var pageItems = filteredItems
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            for (var index = 0; index < pageItems.Count; index++)
            {
                pageItems[index].IsSelected = selectedIds.Contains(pageItems[index].Id);
                pageItems[index].RowNumber = ((CurrentPage - 1) * PageSize) + index + 1;
            }

            FilteredBanks.ReplaceAll(pageItems);
            BuildPagination();
            UpdateSummary();
            _lastAppliedFilterKey = filterKey;
        }

        private void BuildPagination()
        {
            Pages.Clear();

            if (TotalPages <= 5)
            {
                for (var page = 1; page <= TotalPages; page++)
                    AddPage(page);
            }
            else
            {
                AddPage(1);
                AddEllipsis();

                var start = CurrentPage <= 3 ? 2 : CurrentPage >= TotalPages - 2 ? TotalPages - 3 : CurrentPage - 1;
                var end = CurrentPage <= 3 ? 4 : CurrentPage >= TotalPages - 2 ? TotalPages - 1 : CurrentPage + 1;
                for (var page = start; page <= end; page++)
                {
                    if (page > 1 && page < TotalPages)
                        AddPage(page);
                }

                AddEllipsis();
                AddPage(TotalPages);
            }

            OnPropertyChanged(nameof(Pages));
            OnPropertyChanged(nameof(PageInfoText));
        }

        private void AddPage(int page)
        {
            Pages.Add(new BankPageItem
            {
                PageNumber = page,
                PageNumberDisplay = ToPersianNumber(page),
                IsCurrent = page == CurrentPage
            });
        }

        private void AddEllipsis()
        {
            Pages.Add(new BankPageItem
            {
                PageNumber = 0,
                PageNumberDisplay = "...",
                IsCurrent = false
            });
        }

        public string PageInfoText =>
            $"نمایش {ToPersianNumber(FilteredBanks.Count)} از {ToPersianNumber(_filteredListCount)} مورد";

        private void UpdateSummary()
        {
            var selectedCount = AllBanks.Count(item => item.IsSelected && !item.IsEmpty);
            SelectedSummaryText = $"بانک انتخاب شده ({ToPersianNumber(selectedCount)})";
            TotalCountText = $"{ToPersianNumber(_filteredListCount)} مورد";
            OnPropertyChanged(nameof(PageInfoText));
        }

        private static string ToPersianDate(string? dateValue)
        {
            if (string.IsNullOrWhiteSpace(dateValue) || !DateTime.TryParse(dateValue, out var date))
                return "—";

            var calendar = new PersianCalendar();
            return $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}";
        }

        private static string ToPersianNumber(int number)
        {
            var digits = new[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            var result = string.Empty;
            foreach (var character in number.ToString())
                result += digits[int.Parse(character.ToString())];
            return result;
        }

        private static string GetFriendlyDeleteError(Exception exception)
        {
            var text = exception.ToString();
            if (text.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("SqlException", StringComparison.OrdinalIgnoreCase))
                return "خطا در اتصال به دیتابیس";

            return "حذف بانک انجام نشد.";
        }

        private static void ShowErrorToast(string message)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                Taadol.Controls.ToastManager.Error(message);
                return;
            }

            dispatcher.BeginInvoke(new Action(() => Taadol.Controls.ToastManager.Error(message)));
        }

        private static void CancelAndDispose(CancellationTokenSource? cts)
        {
            if (cts == null)
                return;

            try { cts.Cancel(); }
            catch (ObjectDisposedException) { }
            finally { cts.Dispose(); }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeState, 1) != 0)
                return;

            CancelPendingLoads();
        }
    }
}
