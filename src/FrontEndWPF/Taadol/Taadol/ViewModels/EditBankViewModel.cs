using System;
using System.Threading;
using System.Threading.Tasks;
using BankManagement.Application.Contracts.Bank;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Taadol.ViewModels
{

    public sealed class EditBankViewModel : BankFormViewModelBase<BankEditFormSnapshot>
    {
        private readonly IServiceProvider _serviceProvider;

        private CancellationTokenSource? _loadCts = new();
        private Task? _loadTask;
        private bool _loadFailed;

        private string _uniqueCode = string.Empty;
        private bool _isLoading;
        private string? _loadErrorText;

        public EditBankViewModel(IServiceProvider serviceProvider, long bankId)
        {
            _serviceProvider = serviceProvider;
            BankId = bankId;

            _ = LoadAsync(_loadCts!.Token);
        }

        public long BankId { get; }

        public event Action? BankUpdated;
        public event Action? LoadFailed;

        public string UniqueCode
        {
            get => _uniqueCode;
            private set => SetProperty(ref _uniqueCode, value);
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

        public string? LoadErrorText
        {
            get => _loadErrorText;
            private set => SetProperty(ref _loadErrorText, value);
        }

        public override bool IsFormInteractive => base.IsFormInteractive && !IsLoading;
        public override string SaveButtonText => IsSaving ? "در حال ذخیره..." : "ذخیره تغییرات";
        protected override string SuccessMessage => "بانک با موفقیت ویرایش شد.";
        protected override string DefaultSaveErrorMessage => "خطا در ویرایش بانک";

        protected override bool CanSave() => base.CanSave() && !IsLoading;

        protected override BankEditFormSnapshot BuildSnapshot() => new(
            UniqueCode,
            Title,
            Country,
            Description,
            Logo,
            SelectedBankTypeId);

        public Task LoadAsync(CancellationToken cancellationToken)
        {
            if (IsDisposed)
                return Task.CompletedTask;

            if (_loadTask != null)
            {

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
                _isInitializing = false;
                ResetInitialSnapshot();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {

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

        protected override async Task<bool> ExecuteSaveCoreAsync(CancellationToken token)
        {
            var title = Title.Trim();
            var country = Country.Trim();
            var description = (Description ?? string.Empty).Trim();
            var logo = string.IsNullOrWhiteSpace(Logo) ? null : Logo;
            var bankTypeId = SelectedBankTypeId;

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

            return true;
        }

        protected override void OnSaved() => BankUpdated?.Invoke();

        protected override void DisposeCore()
        {
            CancelAndDispose(ref _loadCts);
        }
    }
}
