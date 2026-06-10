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

            SaveCommand = new CompanySaveCommand(SaveCompany);
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
        private void SaveCompany()
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                MessageBox.Show("نام شرکت / کسب‌وکار را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(OfficialName))
            {
                MessageBox.Show("نام رسمی ثبتی را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!FoundingDate.HasValue)
            {
                MessageBox.Show("تاریخ تاسیس را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var command = new CreateCompanies
            {
                Title = CompanyName.Trim(),
                LegalName = OfficialName.Trim(),
                EstablishedDate = FoundingDate.Value,
                Logo = string.IsNullOrWhiteSpace(ProductImage) ? "" : ProductImage
            };

            var operation = _companyApplication.Create(command);

            var message = GetOperationMessage(operation);

            if (!IsOperationSucceeded(operation))
            {
                MessageBox.Show(
                    string.IsNullOrWhiteSpace(message) ? "ثبت شرکت انجام نشد." : message,
                    "خطا",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                string.IsNullOrWhiteSpace(message) ? "شرکت با موفقیت ثبت شد." : message,
                "موفق",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ClearForm();
        }

        private void ClearForm()
        {
            UniqueId = "";
            CompanyName = "";
            OfficialName = "";
            FoundingDate = null;
            ProductImage = "";
            IsBranchActive = true;
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
            if (picker == null || !picker.SelectedDate.HasValue) return;

            FoundingDate = picker.SelectedDate.Value;
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