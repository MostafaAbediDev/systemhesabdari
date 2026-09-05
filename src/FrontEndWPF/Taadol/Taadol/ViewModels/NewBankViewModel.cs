using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BankManagement.Application.Contracts.Bank;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;

namespace Taadol.ViewModels
{
    public sealed class NewBankViewModel : ObservableObject, IDisposable, IUnsavedChangesAware
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _loadCts = new();
        private CancellationTokenSource _saveCts = new();
        private int _disposeState;
        private bool _isInitializing = true;
        private bool _hasUnsavedChanges;

        private string _title = string.Empty;
        private string _country = "ایران";
        private string _description = string.Empty;
        private string _logo = string.Empty;
        private string _uniqueCode = string.Empty;
        private long _selectedBankTypeId;
        private bool _isUniqueCodeAutomatic = true;
        private bool _isActive = true;
        private bool _isLoading;
        private bool _isSaving;

        public NewBankViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            SaveCommand = new SafeAsyncCommand(SaveBankSafeAsync, CanSave);
            CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => CancelRequested?.Invoke());
            SelectBankTypeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<BankTypeOption>(SelectBankType);
            SelectBankType(BankTypes[0]);
            _isInitializing = false;
        }

        // انواع بانک ثابت هستند و شناسه آن‌ها با داده‌های Seed بک‌اند هماهنگ است.
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
        public ICommand CancelCommand { get; }
        public ICommand SelectBankTypeCommand { get; }
        public event Action? CancelRequested;
        public event Action? BankSaved;

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

        public string UniqueCode
        {
            get => _uniqueCode;
            set
            {
                if (SetProperty(ref _uniqueCode, value))
                    MarkChanged();
            }
        }

        public bool IsUniqueCodeAutomatic
        {
            get => _isUniqueCodeAutomatic;
            set
            {
                if (!SetProperty(ref _isUniqueCodeAutomatic, value)) return;
                OnPropertyChanged(nameof(IsUniqueCodeManual));
                MarkChanged();
            }
        }

        public bool IsUniqueCodeManual => !IsUniqueCodeAutomatic;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (SetProperty(ref _isActive, value))
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

        public bool IsFormInteractive => !IsSaving && !IsLoading && !IsDisposed;
        public string SaveButtonText => IsSaving ? "در حال ذخیره..." : "ثبت";
        public bool HasUnsavedChanges => _hasUnsavedChanges;
        private bool IsDisposed => Volatile.Read(ref _disposeState) != 0;

        public Task LoadAsync()
        {
            if (IsDisposed)
                return Task.CompletedTask;

            return Task.CompletedTask;
        }

        private bool CanSave() => !IsSaving && !IsLoading && !IsDisposed;

        private void SelectBankType(BankTypeOption? option)
        {
            if (option == null || IsDisposed) return;

            foreach (var type in BankTypes)
                type.IsSelected = type.Id == option.Id;

            SelectedBankTypeId = option.Id;
        }

        private async Task SaveBankSafeAsync()
        {
            try
            {
                await SaveBankAsync();
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewBankViewModel] ثبت بانک لغو شد");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewBankViewModel] خطا در ثبت بانک: {ex}");
                ShowErrorToast("خطا در ثبت بانک");
            }
        }

        private async Task SaveBankAsync()
        {
            if (!CanSave()) return;

            if (string.IsNullOrWhiteSpace(Title))
            {
                ShowWarningToast("نام بانک را وارد کنید.");
                return;
            }

            if (Title.Trim().Length > 200)
            {
                ShowWarningToast("نام بانک نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.");
                return;
            }

            if (Description?.Trim().Length > 300)
            {
                ShowWarningToast("توضیحات نمی‌تواند بیشتر از ۳۰۰ کاراکتر باشد.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Country))
            {
                ShowWarningToast("کشور را انتخاب کنید.");
                return;
            }

            if (SelectedBankTypeId <= 0)
            {
                ShowWarningToast("نوع بانک را انتخاب کنید.");
                return;
            }

            if (!IsUniqueCodeAutomatic && string.IsNullOrWhiteSpace(UniqueCode))
            {
                ShowWarningToast("شناسه یکتا را وارد کنید.");
                return;
            }

            var title = Title.Trim();
            var country = Country.Trim();
            var description = Description?.Trim() ?? string.Empty;
            var logo = string.IsNullOrWhiteSpace(Logo) ? null : Logo;
            var bankTypeId = SelectedBankTypeId;
            var token = _saveCts.Token;

            IsSaving = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = _serviceProvider.CreateScope();
                    var application = scope.ServiceProvider.GetRequiredService<IBankApplication>();
                    var operation = application.Create(new CreateBank
                    {
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
                if (IsDisposed) return;

                if (!result.IsSucceeded)
                {
                    ShowWarningToast(result.Message ?? "ثبت بانک انجام نشد.");
                    return;
                }

                ShowSuccessToast(result.Message ?? "بانک با موفقیت ایجاد شد.");
                ClearForm();
                BankSaved?.Invoke();
            }
            finally
            {
                if (!IsDisposed)
                    IsSaving = false;
            }
        }

        private void ClearForm()
        {
            _isInitializing = true;
            Title = string.Empty;
            Country = "ایران";
            Description = string.Empty;
            Logo = string.Empty;
            UniqueCode = string.Empty;
            IsUniqueCodeAutomatic = true;
            IsActive = true;
            if (BankTypes.Count > 0)
                SelectBankType(BankTypes[0]);
            _hasUnsavedChanges = false;
            OnPropertyChanged(nameof(HasUnsavedChanges));
            _isInitializing = false;
        }

        private void MarkChanged()
        {
            if (_isInitializing || IsDisposed || _hasUnsavedChanges) return;
            _hasUnsavedChanges = true;
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

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
            if (current == null) return;

            try { current.Cancel(); }
            catch (ObjectDisposedException) { }
            finally { current.Dispose(); }
        }
    }

    internal sealed class SafeAsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public SafeAsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _ = ExecuteSafeAsync();

        private async Task ExecuteSafeAsync()
        {
            try
            {
                await _execute();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SafeAsyncCommand] خطای کنترل‌نشده: {ex}");
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                    ToastManager.Error("خطای غیرمنتظره در اجرای عملیات");
                else
                    dispatcher.BeginInvoke(new Action(() => ToastManager.Error("خطای غیرمنتظره در اجرای عملیات")));
            }
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public sealed class BankTypeOption : ObservableObject
    {
        private bool _isSelected;

        public BankTypeOption(long id, string title)
        {
            Id = id;
            Title = title ?? string.Empty;
        }

        public long Id { get; }
        public string Title { get; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
