using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BankManagement.Application.Contracts.Bank;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagement.Application.Contracts.Fund;
using GeneralInfoManagement.Application.Contract.Branches;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.ViewModels
{
    public sealed class NewFundViewModel : ObservableObject, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private string _title = string.Empty;
        private long _selectedBranchId;
        private long _selectedBankId;
        private long _selectedAccountId;
        private string _accountNumber = string.Empty;
        private string _shaba = string.Empty;
        private string _cardNumber = string.Empty;
        private DateTime? _accountRegistrationDate;
        private bool _isSaving;
        private bool _isLoading;
        private bool _isAccountServiceAvailable;
        private CancellationTokenSource _loadCts = new();

        public NewFundViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
            CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => CancelRequested?.Invoke());
        }

        public ObservableCollection<BranchOption> Branches { get; } = new();
        public ObservableCollection<BankOption> Banks { get; } = new();
        public ObservableCollection<AccountOption> Accounts { get; } = new();
        public AsyncRelayCommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action? CancelRequested;
        public event Action? Saved;

        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public long SelectedBranchId
        {
            get => _selectedBranchId;
            set
            {
                if (SetProperty(ref _selectedBranchId, value))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public long SelectedBankId { get => _selectedBankId; set => SetProperty(ref _selectedBankId, value); }
        public long SelectedAccountId { get => _selectedAccountId; set => SetProperty(ref _selectedAccountId, value); }
        public string AccountNumber { get => _accountNumber; set => SetProperty(ref _accountNumber, value); }
        public string Shaba { get => _shaba; set => SetProperty(ref _shaba, value); }
        public string CardNumber { get => _cardNumber; set => SetProperty(ref _cardNumber, value); }
        public DateTime? AccountRegistrationDate { get => _accountRegistrationDate; set => SetProperty(ref _accountRegistrationDate, value); }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (SetProperty(ref _isLoading, value))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            private set
            {
                if (SetProperty(ref _isSaving, value))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public bool IsAccountServiceAvailable
        {
            get => _isAccountServiceAvailable;
            private set
            {
                if (SetProperty(ref _isAccountServiceAvailable, value))
                {
                    OnPropertyChanged(nameof(AccountServiceHint));
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public string AccountServiceHint => IsAccountServiceAvailable
            ? string.Empty
            : "لیست حساب‌ها پس از آماده‌شدن Contract مربوطه در دسترس خواهد بود.";

        public async Task LoadAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            try
            {
                var token = _loadCts.Token;
                var data = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = _serviceProvider.CreateScope();

                    var branches = scope.ServiceProvider.GetRequiredService<IBranchApplication>()
                        .GetBranches()
                        .Where(x => x.IsActive)
                        .Select(x => new BranchOption { Id = x.Id, Title = x.Title })
                        .ToList();

                    var banks = scope.ServiceProvider.GetRequiredService<IBankApplication>()
                        .Search(new BankSearchModel())
                        .Select(x => new BankOption { Id = x.Id, Title = x.Title })
                        .ToList();

                    return (branches, banks);
                }, token);

                Branches.Clear();
                foreach (var branch in data.branches)
                    Branches.Add(branch);

                Banks.Clear();
                foreach (var bank in data.banks)
                    Banks.Add(bank);

                if (SelectedBranchId <= 0 && Branches.Count > 0)
                    SelectedBranchId = Branches[0].Id;

                Accounts.Clear();
                Accounts.Add(new AccountOption
                {
                    Id = 0,
                    Title = "حساب‌ها در انتظار سرویس بک‌اند هستند"
                });
                IsAccountServiceAvailable = false;
            }
            catch (OperationCanceledException) when (_loadCts.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFundViewModel] Load error: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات صندوق");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanSave() => !IsSaving && !IsLoading;

        private async Task SaveAsync()
        {
            if (IsSaving) return;

            if (string.IsNullOrWhiteSpace(Title))
            {
                ToastManager.Warning("عنوان صندوق را وارد کنید.");
                return;
            }

            if (SelectedBranchId <= 0)
            {
                ToastManager.Warning("لطفاً شعبه را انتخاب کنید.");
                return;
            }

            if (!IsAccountServiceAvailable || SelectedAccountId <= 0)
            {
                ToastManager.Warning("ثبت صندوق تا آماده‌شدن سرویس حساب‌ها امکان‌پذیر نیست.");
                return;
            }

            IsSaving = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IFundApplication>().Create(new CreateFunds
                    {
                        Title = Title.Trim(),
                        BranchId = SelectedBranchId,
                        AccountId = SelectedAccountId
                    });
                });

                if (!result.IsSucceeded)
                {
                    ToastManager.Warning(result.Message ?? "ثبت صندوق انجام نشد.");
                    return;
                }

                ToastManager.Success(result.Message ?? "صندوق با موفقیت ثبت شد.");
                Saved?.Invoke();
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFundViewModel] Save error: {ex}");
                ToastManager.Error("خطا در ثبت صندوق");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void ClearForm()
        {
            Title = string.Empty;
            SelectedBankId = 0;
            SelectedAccountId = 0;
            AccountNumber = string.Empty;
            Shaba = string.Empty;
            CardNumber = string.Empty;
            AccountRegistrationDate = null;
            if (Branches.Count > 0)
                SelectedBranchId = Branches[0].Id;
        }

        public void Dispose()
        {
            try { _loadCts.Cancel(); } catch (ObjectDisposedException) { }
            _loadCts.Dispose();
        }

        public sealed class BranchOption
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
        }

        public sealed class BankOption
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
        }

        public sealed class AccountOption
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
        }
    }
}
