using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;using System.Globalization;

namespace Taadol.Views
{

    public partial class NewCompanyView : UserControl, INotifyPropertyChanged
    {
        private readonly ICompanyApplication _companyApplication;

        private bool _isSaving;
        private string _uniqueId;
        private string _companyName;
        private string _officialName;
        private DateTime? _foundingDate;
        private string _productImage;
        private bool _isBranchActive = true;

        public ICommand SaveCommand { get; }

        public string UniqueId
        {
            get => _uniqueId;
            set
            {
                _uniqueId = value;
                OnPropertyChanged(nameof(UniqueId));
            }
        }

        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        public string OfficialName
        {
            get => _officialName;
            set
            {
                _officialName = value;
                OnPropertyChanged(nameof(OfficialName));
            }
        }

        public DateTime? FoundingDate
        {
            get => _foundingDate;
            set
            {
                _foundingDate = value;
                OnPropertyChanged(nameof(FoundingDate));
            }
        }

        public string ProductImage
        {
            get => _productImage;
            set
            {
                _productImage = value;
                OnPropertyChanged(nameof(ProductImage));
            }
        }

        public bool IsBranchActive
        {
            get => _isBranchActive;
            set
            {
                _isBranchActive = value;
                OnPropertyChanged(nameof(IsBranchActive));
            }
        }

        public NewCompanyView()
        {
            InitializeComponent();

            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            SaveCommand = new CompanySaveCommand(async () => await SaveCompanyAsync());
            DataContext = this;
        }
        public class CompanySaveCommand : ICommand
        {
            private readonly Action _execute;

            public CompanySaveCommand(Action execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                _execute();
            }

            public event EventHandler CanExecuteChanged;
        }
        private async Task SaveCompanyAsync()
        {
            if (_isSaving) return;

            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                ToastManager.Warning("نام شرکت / کسب‌وکار را وارد کنید.");
                return;
            }

            if (string.IsNullOrWhiteSpace(OfficialName))
            {
                ToastManager.Warning("نام رسمی ثبتی را وارد کنید.");
                return;
            }

            if (FoundingDate.HasValue && FoundingDate.Value.Date > DateTime.Today)
            {
                ToastManager.Warning("تاریخ تاسیس نمیتواند در آینده باشد.");
                return;
            }

            if (!FoundingDate.HasValue)
            {
                ToastManager.Warning("تاریخ تاسیس را انتخاب کنید.");
                return;
            }

            _isSaving = true;
            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
                SaveButton.ButtonText = "در حال ذخیره...";
            }

            try
            {
                var command = new CreateCompanies
                {
                    Title = CompanyName.Trim(),
                    LegalName = OfficialName.Trim(),
                    EstablishedDate = FoundingDate.Value,
                    Logo = string.IsNullOrWhiteSpace(ProductImage) ? "" : ProductImage
                };

                var operation = await Task.Run(() => _companyApplication.Create(command));

                var message = GetOperationMessage(operation);

                if (!IsOperationSucceeded(operation))
                {
                    ToastManager.Warning(
                        string.IsNullOrWhiteSpace(message) ? "ثبت شرکت انجام نشد." : message);

                    return;
                }

                ToastManager.Success(
                    string.IsNullOrWhiteSpace(message) ? "شرکت با موفقیت ثبت شد." : message);

                ClearForm();

                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewCompanyView] Save company error: {ex}");
                ToastManager.Error("خطا در ثبت شرکت");
            }
            finally
            {
                _isSaving = false;
                if (SaveButton != null)
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.ButtonText = "ثبت شرکت";
                }
            }
        }

        private void Cancel_Click(object sender, MouseButtonEventArgs e)
        {

            if (HasContent())
            {
                var result = MessageBox.Show(
                    "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                    "ذخیره تغییرات",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _ = SaveCompanyAsync();
                    return;
                }
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            CloseForm();
        }

        private bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(CompanyName)
                || !string.IsNullOrWhiteSpace(OfficialName)
                || FoundingDate.HasValue
                || !string.IsNullOrWhiteSpace(ProductImage);
        }

        private void CloseForm()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;
            if (mainWindow.ModalContent.Content == this)
                mainWindow.CloseModal();
            else
                mainWindow.CloseCurrentForm();
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {

            CloseForm();
        }

        private void ClearForm()
        {
            UniqueId = "";
            CompanyName = "";
            OfficialName = "";
            FoundingDate = null;
            ProductImage = "";
            IsBranchActive = true;

            FoundingDatePicker?.Clear();
        }

        private bool IsOperationSucceeded(object operation)
        {
            if (operation == null) return false;

            var type = operation.GetType();

            foreach (var name in new[] { "IsSucceeded", "Succeeded", "Success", "IsSuccess" })
            {
                var prop = type.GetProperty(name);

                if (prop != null && prop.PropertyType == typeof(bool))
                    return (bool)prop.GetValue(operation);
            }

            var message = GetOperationMessage(operation);

            if (!string.IsNullOrWhiteSpace(message) &&
                (message.Contains("تکراری") || message.Contains("یافت نشد") || message.Contains("خطا")))
                return false;

            return true;
        }

        private string GetOperationMessage(object operation)
        {
            if (operation == null) return "";

            var prop = operation.GetType().GetProperty("Message");

            return prop?.GetValue(operation)?.ToString() ?? "";
        }

        private void UniqueIdToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
        }

        private void BranchActive_Changed(object sender, RoutedEventArgs e)
        {
        }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null) return;

            FoundingDate = picker.SelectedDate;
        }

        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
            ProductImage = "";
        }

        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}