using System;
using System.Threading;
using System.Threading.Tasks;
using BankManagement.Application.Contracts.Bank;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Taadol.ViewModels
{
    /// <summary>
    /// ViewModel فرم ثبت بانک. فقط اعضای ویژهٔ «ثبت» (شناسهٔ یکتا، وضعیت فعال، رویداد ذخیره)
    /// را نگه می‌دارد؛ وضعیت و منطق مشترک از BankFormViewModelBase ارث برده می‌شود.
    /// </summary>
    public sealed class NewBankViewModel : BankFormViewModelBase<BankNewFormSnapshot>
    {
        private readonly IServiceProvider _serviceProvider;

        private string _uniqueCode = string.Empty;
        private bool _isUniqueCodeAutomatic = true;
        private bool _isActive = true;
        private string _successMessage = "بانک با موفقیت ایجاد شد.";

        public NewBankViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ResetInitialSnapshot();
        }

        public event Action? BankSaved;

        // این فیلد فعلاً فقط برای تکمیل ظاهر فرم است و تا آماده‌شدن قرارداد بک‌اند ارسال نمی‌شود.
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
                if (SetProperty(ref _isUniqueCodeAutomatic, value))
                {
                    OnPropertyChanged(nameof(IsUniqueCodeManual));
                    MarkChanged();
                }
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

        public override string SaveButtonText => IsSaving ? "در حال ذخیره..." : "ثبت";
        protected override string SuccessMessage => _successMessage;
        protected override string DefaultSaveErrorMessage => "خطا در ثبت بانک";

        protected override BankNewFormSnapshot BuildSnapshot() => new(
            UniqueCode,
            IsUniqueCodeAutomatic,
            Title,
            Country,
            Description,
            Logo,
            SelectedBankTypeId,
            IsActive);

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
            if (IsDisposed)
                return false;

            if (!result.IsSucceeded)
            {
                ShowWarningToast(MapResultMessage(result.Message));
                return false;
            }

            _successMessage = result.Message ?? "بانک با موفقیت ایجاد شد.";
            return true;
        }

        protected override void OnSaved() => BankSaved?.Invoke();

        protected override void ResetFormIfNew() => ClearForm();

        private void ClearForm()
        {
            _isInitializing = true;
            UniqueCode = string.Empty;
            IsUniqueCodeAutomatic = true;
            Title = string.Empty;
            Country = "ایران";
            Description = string.Empty;
            Logo = string.Empty;
            IsActive = true;
            if (BankTypes.Count > 0)
                SelectBankType(BankTypes[0]);
            _isInitializing = false;
            ResetInitialSnapshot();
        }

        private string MapResultMessage(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return DefaultSaveErrorMessage;

            return GetFriendlyErrorMessage(new InvalidOperationException(message), DefaultSaveErrorMessage);
        }
    }
}
