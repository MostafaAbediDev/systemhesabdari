using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.ViewModels
{

    public abstract class BankFormViewModelBase<TSnapshot> : ObservableObject, IDisposable, IUnsavedChangesAware
        where TSnapshot : FormSnapshotBase
    {
        private const string DefaultCountry = "ایران";

        private string _title = string.Empty;
        private string _country = DefaultCountry;
        private string _description = string.Empty;
        private string _logo = string.Empty;
        private long _selectedBankTypeId;
        private bool _isSaving;
        private CancellationTokenSource? _saveCts;
        private TSnapshot? _initialSnapshot;

        protected bool _isInitializing;

        protected int _disposeState;

        protected BankFormViewModelBase()
        {
            SaveCommand = new SafeAsyncCommand(SaveBankSafeAsync, CanSave);
            SelectBankTypeCommand = new RelayCommand<BankTypeOption>(SelectBankType);

            SelectBankType(BankTypes[0]);
        }

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

        public bool IsDisposed => Volatile.Read(ref _disposeState) != 0;

        public virtual bool IsFormInteractive => !IsSaving && !IsDisposed;

        public virtual bool HasUnsavedChanges
        {
            get
            {
                if (IsDisposed || _initialSnapshot == null)
                    return false;

                var current = BuildSnapshot();
                return !current.Equals(_initialSnapshot);
            }
        }

        public abstract string SaveButtonText { get; }
        protected abstract string SuccessMessage { get; }
        protected virtual string DefaultSaveErrorMessage => "خطا در ذخیره اطلاعات";

        protected virtual bool CanSave() => !IsSaving && !IsDisposed;

        protected void SelectBankType(BankTypeOption? option)
        {
            if (option == null || IsDisposed)
                return;

            foreach (var type in BankTypes)
                type.IsSelected = type.Id == option.Id;

            SelectedBankTypeId = option.Id;
        }

        public virtual async Task<bool> SaveAsync()
        {
            if (!CanSave())
                return false;

            if (!ValidateForm(out var validationError))
            {
                ShowWarningToast(validationError);
                return false;
            }

            CancelAndDispose(ref _saveCts);
            _saveCts = new CancellationTokenSource();
            var token = _saveCts.Token;

            IsSaving = true;
            OnPropertyChanged(nameof(IsFormInteractive));

            try
            {
                var success = await ExecuteSaveCoreAsync(token);
                if (!success)
                    return false;

                ShowSuccessToast(SuccessMessage);
                ResetInitialSnapshot();
                OnSaved();
                ResetFormIfNew();
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BankFormViewModelBase] خطا در ذخیره‌سازی: {ex}");
                ShowErrorToast(GetFriendlyErrorMessage(ex, DefaultSaveErrorMessage));
                return false;
            }
            finally
            {
                if (!IsDisposed)
                {
                    IsSaving = false;
                    OnPropertyChanged(nameof(IsFormInteractive));
                    RaiseCanExecuteChanged();
                }
            }
        }

        private async Task SaveBankSafeAsync() => await SaveAsync();

        protected virtual bool ValidateForm(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title))
            {
                errorMessage = "نام بانک را وارد کنید.";
                return false;
            }

            if (Title.Trim().Length > 200)
            {
                errorMessage = "نام بانک نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.";
                return false;
            }

            if ((Description ?? string.Empty).Trim().Length > 300)
            {
                errorMessage = "توضیحات نمی‌تواند بیشتر از ۳۰۰ کاراکتر باشد.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Country))
            {
                errorMessage = "کشور را انتخاب کنید.";
                return false;
            }

            if (!TryValidateLogo(Logo, out var logoError))
            {
                errorMessage = logoError;
                return false;
            }

            if (SelectedBankTypeId <= 0)
            {
                errorMessage = "نوع بانک را انتخاب کنید.";
                return false;
            }

            return true;
        }

        public void ResetInitialSnapshot()
        {
            _initialSnapshot = BuildSnapshot();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        protected void MarkChanged()
        {
            if (_isInitializing || IsDisposed)
                return;

            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        protected static bool TryValidateLogo(string? path, out string errorMessage)
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
                System.Diagnostics.Debug.WriteLine($"[BankFormViewModelBase] خطا در خواندن لوگو: {exception}");
                errorMessage = "فایل لوگو قابل خواندن نیست.";
                return false;
            }
        }

        protected static string GetFriendlyErrorMessage(Exception exception, string fallbackMessage)
        {
            var messages = new System.Text.StringBuilder();
            for (var current = exception; current != null; current = current.InnerException)
            {
                messages.Append(' ');
                messages.Append(current.Message);
                messages.Append(' ');
                messages.Append(current.GetType().Name);
            }

            var text = messages.ToString();
            if (text.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("تکراری", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("IX_", StringComparison.OrdinalIgnoreCase))
                return "این نام بانک قبلاً ثبت شده";

            if (text.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("FK_", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("BankType", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("نوع بانک", StringComparison.OrdinalIgnoreCase))
                return "نوع بانک انتخاب‌شده نامعتبر است";

            if (text.Contains("truncated", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("maximum", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("max length", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("طول", StringComparison.OrdinalIgnoreCase))
                return "متن وارد شده بیش از حد مجاز است";

            if (text.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("network", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("server", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("pool", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("SqlException", StringComparison.OrdinalIgnoreCase))
                return "خطا در اتصال به دیتابیس";

            return fallbackMessage;
        }

        protected static void ShowWarningToast(string message) => ShowToast(() => ToastManager.Warning(message));
        protected static void ShowSuccessToast(string message) => ShowToast(() => ToastManager.Success(message));
        protected static void ShowErrorToast(string message) => ShowToast(() => ToastManager.Error(message));

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

        protected void RaiseCanExecuteChanged() => (SaveCommand as SafeAsyncCommand)?.RaiseCanExecuteChanged();

        protected abstract void OnSaved();

        protected abstract Task<bool> ExecuteSaveCoreAsync(CancellationToken token);

        protected virtual void ResetFormIfNew()
        {
        }

        protected abstract TSnapshot BuildSnapshot();

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeState, 1) != 0)
                return;

            CancelAndDispose(ref _saveCts);
            OnPropertyChanged(nameof(IsFormInteractive));
            RaiseCanExecuteChanged();
            DisposeCore();
        }

        protected virtual void DisposeCore()
        {
        }

        protected static void CancelAndDispose(ref CancellationTokenSource? cts)
        {
            var current = Interlocked.Exchange(ref cts, null);
            if (current == null)
                return;

            try { current.Cancel(); }
            catch (ObjectDisposedException) { }
            finally { current.Dispose(); }
        }
    }
}
