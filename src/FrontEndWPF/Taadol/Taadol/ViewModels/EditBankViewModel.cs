using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BankManagement.Application.Contracts.Bank;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;

namespace Taadol.ViewModels
{
    public sealed class EditBankViewModel : ObservableObject, IDisposable, IUnsavedChangesAware
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource? _loadCts = new();
        private CancellationTokenSource? _saveCts = new();
        private Task? _loadTask;
        private int _disposeState;
        private bool _isInitializing = true;
        private bool _loadFailed;
        private EditBankFormSnapshot _initialSnapshot;

        private string _uniqueCode = string.Empty;
        private string _title = string.Empty;
        private string _country = "ایران";
        private string _description = string.Empty;
        private string _logo = string.Empty;
        private long _selectedBankTypeId;
        private bool _isLoading;
        private bool _isSaving;
        private string? _loadErrorText;

        public EditBankViewModel(IServiceProvider serviceProvider, long bankId)
        {
            _serviceProvider = serviceProvider;
            BankId = bankId;

            SaveCommand = new SafeAsyncCommand(SaveBankSafeAsync, CanSave);
            SelectBankTypeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<BankTypeOption>(SelectBankType);

            SelectBankType(BankTypes[0]);
            _isInitializing = false;
            _initialSnapshot = CaptureSnapshot();

            // بارگذاری از همان ابتدای ساخت ViewModel شروع می‌شود؛ فراخوانی مجدد LoadAsync
            // در رویداد Loaded همان Task را استفاده می‌کند و بارگذاری تکراری انجام نمی‌شود.
            _ = LoadAsync(_loadCts!.Token);
        }

        public long BankId { get; }

        // انواع بانک با شناسه‌های Seed بک‌اند هماهنگ هستند.
        public ObservableCollection<BankTypeOption> BankTypes { get; } = new()
        {
            new BankTypeOption(1, "دولتی"),
            new BankTypeOption(2, "خصوصی"),
            new BankTypeOption(3, "قرض‌الحسنه")
        };

        public BankTypeOption GovernmentBankType => BankTypes[0];
        public BankTypeOption PrivateBankType => BankTypes[1];
        public BankTypeOption QarzAlHasanehBankType => BankTypes[2];
        public ObservableCollection<string> Countries { get; } = new() { "ایران", "سایر کشورها" };

        public ICommand SaveCommand { get; }
        public ICommand SelectBankTypeCommand { get; }

        public event Action? BankUpdated;
        public event Action? LoadFailed;

        // قرارداد فعلی بک‌اند این مقدار را برنمی‌گرداند؛ فقط برای نمایش خواندنی فرم نگه داشته می‌شود.
        public string UniqueCode
        {
            get => _uniqueCode;
            private set => SetProperty(ref _uniqueCode, value);
        }

        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                {
                    MarkChanged();
                    RaiseCanExecuteChanged();
                }
            }
        }

        public string Country
        {
            get => _country;
            set
            {
                if (SetProperty(ref _country, value))
                    MarkChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (SetProperty(ref _description, value))
                    MarkChanged();
            }
        }

        public string Logo
        {
            get => _logo;
            set
            {
                if (SetProperty(ref _logo, value))
                    MarkChanged();
            }
        }

        public long SelectedBankTypeId
        {
            get => _selectedBankTypeId;
            private set
            {
                if (SetProperty(ref _selectedBankTypeId, value))
                {
                    MarkChanged();
                    RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    OnPropertyChanged(nameof(IsFormInteractive));
                    RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            private set
            {
                if (SetProperty(ref _isSaving, value))
                {
                    OnPropertyChanged(nameof(IsFormInteractive));
                    OnPropertyChanged(nameof(SaveButtonText));
                    RaiseCanExecuteChanged();
                }
            }
        }

        public string? LoadErrorText
        {
            get => _loadErrorText;
            private set => SetProperty(ref _loadErrorText, value);
        }

        public bool IsFormInteractive => !IsLoading && !IsSaving && !IsDisposed;
        public string SaveButtonText => IsSaving ? "در حال ذخیره..." : "ذخیره تغییرات";
        public bool HasUnsavedChanges => !IsSnapshotEqual(CaptureSnapshot(), _initialSnapshot);
        public bool IsDisposed => Volatile.Read(ref _disposeState) != 0;

        private bool CanSave() => !IsLoading && !IsSaving && !IsDisposed;

        private void SelectBankType(BankTypeOption? option)
        {
            if (option == null || IsDisposed)
                return;

            foreach (var type in BankTypes)
                type.IsSelected = type.Id == option.Id;

            SelectedBankTypeId = option.Id;
        }

        public Task LoadAsync(CancellationToken cancellationToken)
        {
            if (IsDisposed)
                return Task.CompletedTask;

            if (_loadTask != null)
            {
                // اگر خطا قبل از اتصال View رخ داده باشد، در فراخوانی Loaded دوباره رویداد را اعلام می‌کنیم.
                if (_loadFailed)
                    LoadFailed?.Invoke();

                return _loadTask;
            }

            _loadTask = LoadCoreAsync(cancellationToken);
            return _loadTask;
        }

        private async Task LoadCoreAsync(CancellationToken cancellationToken)
        {
            IsLoading = true;
            LoadErrorText = null;

            try
            {
                var details = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using var scope = _serviceProvider.CreateScope();
                    var application = scope.ServiceProvider.GetRequiredService<IBankApplication>();
                    var result = application.GetDetails(BankId);
                    cancellationToken.ThrowIfCancellationRequested();
                    return result;
                }, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();
                if (IsDisposed)
                    return;

                if (details == null)
                    throw new InvalidOperationException("بانک پیدا نشد");

                _isInitializing = true;
                UniqueCode = string.Empty;
                Title = details.Title ?? string.Empty;
                Country = string.IsNullOrWhiteSpace(details.Country) ? "ایران" : details.Country;
                Description = details.Description ?? string.Empty;
                Logo = details.Logo ?? string.Empty;
                SelectBankType(BankTypesFind(details.BankTypeId));
                _initialSnapshot = CaptureSnapshot();
                _isInitializing = false;
                OnPropertyChanged(nameof(HasUnsavedChanges));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // لغو بارگذاری هنگام خروج از فرم رفتار عادی است.
            }
            catch (Exception exception)
            {
                _loadFailed = true;
                _isInitializing = false;
                System.Diagnostics.Debug.WriteLine($"[EditBankViewModel] خطا در بارگذاری بانک: {exception}");

                if (!IsDisposed)
                {
                    LoadErrorText = "خطا در بارگذاری اطلاعات بانک";
                    ShowErrorToast("خطا در بارگذاری اطلاعات بانک");
                    LoadFailed?.Invoke();
                }
            }
            finally
            {
                if (!IsDisposed)
                    IsLoading = false;
            }
        }

        private BankTypeOption BankTypesFind(long bankTypeId)
        {
            foreach (var type in BankTypes)
            {
                if (type.Id == bankTypeId)
                    return type;
            }

            return BankTypes[0];
        }

        public Task<bool> SaveAsync() => SaveBankAsync();

        private async Task SaveBankSafeAsync()
        {
            try
            {
                await SaveBankAsync();
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[EditBankViewModel] ویرایش بانک لغو شد");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[EditBankViewModel] خطا در ویرایش بانک: {exception}");
                ShowErrorToast(GetFriendlyErrorMessage(exception));
            }
        }

        private async Task<bool> SaveBankAsync()
        {
            if (!CanSave())
                return false;

            if (string.IsNullOrWhiteSpace(Title))
            {
                ShowWarningToast("نام بانک را وارد کنید.");
                return false;
            }

            if (Title.Trim().Length > 200)
            {
                ShowWarningToast("نام بانک نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.");
                return false;
            }

            if ((Description ?? string.Empty).Trim().Length > 300)
            {
                ShowWarningToast("توضیحات نمی‌تواند بیشتر از ۳۰۰ کاراکتر باشد.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Country))
            {
                ShowWarningToast("کشور را انتخاب کنید.");
                return false;
            }

            if (!TryValidateLogo(Logo, out var logoError))
            {
                ShowWarningToast(logoError);
                return false;
            }

            if (SelectedBankTypeId <= 0)
            {
                ShowWarningToast("نوع بانک را انتخاب کنید.");
                return false;
            }

            var title = Title.Trim();
            var country = Country.Trim();
            var description = (Description ?? string.Empty).Trim();
            var logo = string.IsNullOrWhiteSpace(Logo) ? null : Logo;
            var bankTypeId = SelectedBankTypeId;
            var token = _saveCts?.Token ?? CancellationToken.None;

            IsSaving = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = _serviceProvider.CreateScope();
                    var application = scope.ServiceProvider.GetRequiredService<IBankApplication>();
                    var operation = application.Edit(new EditBank
                    {
                        Id = BankId,
                        Title = title,
                        Country = country,
                        Description = description,
                        Logo = logo,
                        BankTypeId = bankTypeId
                    });
                    token.ThrowIfCancellationRequested();
                    return operation;
                }, token);

                token.ThrowIfCancellationRequested();
                if (IsDisposed)
                    return false;

                if (!result.IsSucceeded)
                {
                    ShowWarningToast(string.IsNullOrWhiteSpace(result.Message)
                        ? "ویرایش بانک انجام نشد."
                        : result.Message);
                    return false;
                }

                ShowSuccessToast("بانک با موفقیت ویرایش شد.");
                BankUpdated?.Invoke();
                return true;
            }
            finally
            {
                if (!IsDisposed)
                    IsSaving = false;
            }
        }

        private static bool TryValidateLogo(string? path, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
                return true;

            if (!File.Exists(path))
            {
                errorMessage = "فایل لوگو قابل خواندن نیست یا وجود ندارد.";
                return false;
            }

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp"
            };

            if (!allowedExtensions.Contains(Path.GetExtension(path)))
            {
                errorMessage = "فرمت لوگو مجاز نیست. فقط PNG، JPG، JPEG، BMP، GIF و WEBP مجاز هستند.";
                return false;
            }

            if (new FileInfo(path).Length > 2 * 1024 * 1024)
            {
                errorMessage = "حجم فایل لوگو نباید بیشتر از ۲ مگابایت باشد.";
                return false;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return true;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[EditBankViewModel] خطا در خواندن لوگو: {exception}");
                errorMessage = "فایل لوگو قابل خواندن نیست.";
                return false;
            }
        }

        private static string GetFriendlyErrorMessage(Exception exception)
        {
            var text = exception.ToString();
            if (text.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("تکراری", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("IX_", StringComparison.OrdinalIgnoreCase))
                return "این نام بانک قبلاً ثبت شده";

            if (text.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("FK_", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("BankType", StringComparison.OrdinalIgnoreCase))
                return "نوع بانک انتخاب‌شده نامعتبر است";

            if (text.Contains("truncated", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("maximum", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("max length", StringComparison.OrdinalIgnoreCase))
                return "متن وارد شده بیش از حد مجاز است";

            if (text.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("network", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("server", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("SqlException", StringComparison.OrdinalIgnoreCase))
                return "خطا در اتصال به دیتابیس";

            return "خطا در ویرایش بانک";
        }

        private void MarkChanged()
        {
            if (_isInitializing || IsDisposed)
                return;

            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private EditBankFormSnapshot CaptureSnapshot() => new(
            UniqueCode,
            Title,
            Country,
            Description,
            Logo,
            SelectedBankTypeId);

        private static bool IsSnapshotEqual(EditBankFormSnapshot left, EditBankFormSnapshot right) =>
            string.Equals(left.UniqueCode, right.UniqueCode, StringComparison.Ordinal) &&
            string.Equals(left.Title, right.Title, StringComparison.Ordinal) &&
            string.Equals(left.Country, right.Country, StringComparison.Ordinal) &&
            string.Equals(left.Description, right.Description, StringComparison.Ordinal) &&
            string.Equals(left.Logo, right.Logo, StringComparison.Ordinal) &&
            left.SelectedBankTypeId == right.SelectedBankTypeId;

        private void RaiseCanExecuteChanged() => (SaveCommand as SafeAsyncCommand)?.RaiseCanExecuteChanged();

        private static void ShowWarningToast(string message) => ShowToast(() => ToastManager.Warning(message));
        private static void ShowSuccessToast(string message) => ShowToast(() => ToastManager.Success(message));
        private static void ShowErrorToast(string message) => ShowToast(() => ToastManager.Error(message));

        private static void ShowToast(Action showAction)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                showAction();
                return;
            }

            dispatcher.BeginInvoke(new Action(showAction));
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeState, 1) != 0)
                return;

            CancelAndDispose(ref _loadCts);
            CancelAndDispose(ref _saveCts);
            OnPropertyChanged(nameof(IsFormInteractive));
            RaiseCanExecuteChanged();
        }

        private static void CancelAndDispose(ref CancellationTokenSource? cts)
        {
            var current = Interlocked.Exchange(ref cts, null);
            if (current == null)
                return;

            try { current.Cancel(); }
            catch (ObjectDisposedException) { }
            finally { current.Dispose(); }
        }
    }

    internal sealed class EditBankFormSnapshot
    {
        public EditBankFormSnapshot(
            string? uniqueCode,
            string? title,
            string? country,
            string? description,
            string? logo,
            long selectedBankTypeId)
        {
            UniqueCode = uniqueCode ?? string.Empty;
            Title = title ?? string.Empty;
            Country = country ?? string.Empty;
            Description = description ?? string.Empty;
            Logo = logo ?? string.Empty;
            SelectedBankTypeId = selectedBankTypeId;
        }

        public string UniqueCode { get; }
        public string Title { get; }
        public string Country { get; }
        public string Description { get; }
        public string Logo { get; }
        public long SelectedBankTypeId { get; }
    }
}
