using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BankManagement.Application.Contracts.Bank;
using BankManagement.Application.Contracts.BankType;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;

namespace Taadol.ViewModels
{
    public sealed class NewBankViewModel : ObservableObject, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _loadCts = new();
        private string _title = string.Empty;
        private string _country = "ایران";
        private string _description = string.Empty;
        private string _logo = string.Empty;
        private long _selectedBankTypeId;
        private bool _isLoading;
        private bool _isSaving;

        public NewBankViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            SaveCommand = new LocalAsyncCommand(SaveAsync, CanSave);
            CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => CancelRequested?.Invoke());
            SelectBankTypeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<BankTypeOption>(SelectBankType);
        }

        public ObservableCollection<BankTypeOption> BankTypes { get; } = new();
        public ObservableCollection<string> Countries { get; } = new() { "ایران", "سایر کشورها" };
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectBankTypeCommand { get; }
        public event Action? CancelRequested;
        public event Action? Saved;

        public string Title { get => _title; set { if (SetProperty(ref _title, value)) RaiseCanExecuteChanged(); } }
        public string Country { get => _country; set => SetProperty(ref _country, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public string Logo { get => _logo; set => SetProperty(ref _logo, value); }
        public long SelectedBankTypeId { get => _selectedBankTypeId; private set { if (SetProperty(ref _selectedBankTypeId, value)) RaiseCanExecuteChanged(); } }
        public bool IsLoading { get => _isLoading; private set { if (SetProperty(ref _isLoading, value)) RaiseCanExecuteChanged(); } }
        public bool IsSaving { get => _isSaving; private set { if (SetProperty(ref _isSaving, value)) RaiseCanExecuteChanged(); } }

        public async Task LoadAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                var token = _loadCts.Token;
                var types = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = _serviceProvider.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IBankTypeApplication>().GetBankTypes() ?? new();
                }, token);

                BankTypes.Clear();
                foreach (var type in types.Where(x => x.Id > 0))
                    BankTypes.Add(new BankTypeOption(type.Id, type.Title));
                if (BankTypes.Count > 0) SelectBankType(BankTypes[0]);
            }
            catch (OperationCanceledException) when (_loadCts.IsCancellationRequested) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewBankViewModel] Load bank types error: {ex}");
                ToastManager.Error("خطا در بارگذاری انواع بانک");
            }
            finally { IsLoading = false; }
        }

        private bool CanSave() => !IsSaving && !IsLoading;

        private void SelectBankType(BankTypeOption? option)
        {
            if (option == null) return;
            foreach (var type in BankTypes) type.IsSelected = type.Id == option.Id;
            SelectedBankTypeId = option.Id;
        }

        private async Task SaveAsync()
        {
            if (IsSaving) return;
            if (string.IsNullOrWhiteSpace(Title)) { ToastManager.Warning("نام بانک را وارد کنید."); return; }
            if (string.IsNullOrWhiteSpace(Country)) { ToastManager.Warning("کشور را وارد کنید."); return; }
            if (SelectedBankTypeId <= 0) { ToastManager.Warning("نوع بانک را انتخاب کنید."); return; }

            IsSaving = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IBankApplication>().Create(new CreateBank
                    {
                        Title = Title.Trim(),
                        Country = Country.Trim(),
                        Description = Description?.Trim() ?? string.Empty,
                        Logo = string.IsNullOrWhiteSpace(Logo) ? null : Logo,
                        BankTypeId = SelectedBankTypeId
                    });
                });

                if (!result.IsSucceeded) { ToastManager.Warning(result.Message ?? "ثبت بانک انجام نشد."); return; }
                ToastManager.Success(result.Message ?? "بانک با موفقیت ایجاد شد.");
                Saved?.Invoke();
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewBankViewModel] Save bank error: {ex}");
                ToastManager.Error("خطا در ثبت بانک");
            }
            finally { IsSaving = false; }
        }

        private void ClearForm()
        {
            Title = string.Empty;
            Country = "ایران";
            Description = string.Empty;
            Logo = string.Empty;
            if (BankTypes.Count > 0) SelectBankType(BankTypes[0]);
        }

        private void RaiseCanExecuteChanged() => (SaveCommand as LocalAsyncCommand)?.RaiseCanExecuteChanged();

        public void Dispose()
        {
            try { _loadCts.Cancel(); } catch (ObjectDisposedException) { }
            _loadCts.Dispose();
        }
    }

    internal sealed class LocalAsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        public LocalAsyncCommand(Func<Task> execute, Func<bool> canExecute) { _execute = execute; _canExecute = canExecute; }
        public bool CanExecute(object? parameter) => _canExecute();
        public async void Execute(object? parameter) => await _execute();
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public sealed class BankTypeOption : ObservableObject
    {
        private bool _isSelected;
        public BankTypeOption(long id, string title) { Id = id; Title = title ?? string.Empty; }
        public long Id { get; }
        public string Title { get; }
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    }
}
