using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using FinancialManagement.Application.Contracts.Fund;
using System;
using GeneralInfoManagement.Application.Contract.Branches;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.ViewModels
{
    public sealed class NewFundViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBranchApplication _branchApplication;
        private string _title = string.Empty;
        private long _selectedBranchId;
        private long _selectedAccountId;
        private bool _isSaving;
        private bool _isLoading;
        private CancellationTokenSource _loadCts = new();

        public NewFundViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _branchApplication = serviceProvider.GetRequiredService<IBranchApplication>();
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => !_isSaving);
            CancelCommand = new RelayCommand(() => CancelRequested?.Invoke());
        }

        public ObservableCollection<BranchOption> Branches { get; } = new();
        public ObservableCollection<AccountOption> Accounts { get; } = new();
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action CancelRequested;
        public event Action Saved;

        public string Title { get => _title; set => Set(ref _title, value); }
        public long SelectedBranchId { get => _selectedBranchId; set => Set(ref _selectedBranchId, value); }
        public long SelectedAccountId { get => _selectedAccountId; set => Set(ref _selectedAccountId, value); }
        public bool IsLoading { get => _isLoading; private set => Set(ref _isLoading, value); }

        public async Task LoadAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                var branches = await Task.Run(() =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IBranchApplication>().GetBranches()
                        .Where(x => x.IsActive)
                        .Select(x => new BranchOption { Id = x.Id, Title = x.Title })
                        .ToList();
                }, _loadCts.Token);

                Branches.Clear();
                foreach (var branch in branches) Branches.Add(branch);
                if (SelectedBranchId <= 0 && Branches.Count > 0) SelectedBranchId = Branches[0].Id;
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFundViewModel] Load error: {ex}");
                ToastManager.Error("خطا در بارگذاری شعبه‌ها");
            }
            finally { IsLoading = false; }
        }

        private async Task SaveAsync()
        {
            if (_isSaving) return;
            if (string.IsNullOrWhiteSpace(Title)) { ToastManager.Warning("عنوان صندوق را وارد کنید."); return; }
            if (SelectedBranchId <= 0) { ToastManager.Warning("لطفاً شعبه را انتخاب کنید."); return; }
            if (SelectedAccountId <= 0) { ToastManager.Warning("لطفاً حساب صندوق را انتخاب کنید."); return; }

            _isSaving = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IFundApplication>().Create(new CreateFunds
                    {
                        Title = Title.Trim(), BranchId = SelectedBranchId, AccountId = SelectedAccountId
                    });
                });
                if (!result.IsSucceeded) { ToastManager.Warning(result.Message ?? "ثبت صندوق انجام نشد."); return; }
                ToastManager.Success("صندوق با موفقیت ثبت شد.");
                Saved?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFundViewModel] Save error: {ex}");
                ToastManager.Error("خطا در ثبت صندوق");
            }
            finally { _isSaving = false; }
        }

        public void Dispose()
        {
            try { _loadCts.Cancel(); } catch { }
            _loadCts.Dispose();
        }

        private void Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public sealed class BranchOption { public long Id { get; set; } public string Title { get; set; } }
    public sealed class AccountOption { public long Id { get; set; } public string Title { get; set; } }

    internal sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute; private readonly Func<bool> _canExecute;
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute) { _execute = execute; _canExecute = canExecute; }
        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public async void Execute(object parameter) => await _execute();
        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
