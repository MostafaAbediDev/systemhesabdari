using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application.Contract.Persons;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Controls;
namespace Taadol.Views
{
    public partial class NewPersonView : UserControl

    {
        public void LoadPerson(long id)
        {
            var person = _personApplication.GetDetails(id);

            //FullName = person.FullName;
            //NationalCode = person.NationalCode;

            EconomicCode = person.EconomicCode;
            RegistrationNumber = person.RegistrationNumber;

            isFirstSelected = !person.IsLegal;
        }
        public bool IsEditMode { get; set; }
        public long PersonId { get; set; }
        private int _bankAccountCounter = 0;
        private ObservableCollection<object> _bankAccounts; 
        private List<TreeComboItem> BuildTree() => new List<TreeComboItem>
        {
            new TreeComboItem("مشتری", 1) { IsRoot = true }
                .AddChild(new TreeComboItem("مشتری عمده", 11)
                    .AddChild("مشتری شیراز", 111)
                    .AddChild("مشتری تهران",  112)
                    .AddChild("مشتری یخچال",  113))
                .AddChild(new TreeComboItem("مشتری خرده", 12)
                    .AddChild("مشتری جهرم",  121)
                    .AddChild("مشتری یخچال", 122))
                .AddChild("مشتری و تامین کننده", 13)
                .AddChild("مشتری و پرسنل",       14),

            new TreeComboItem("تامین کننده", 2) { IsRoot = true }
                .AddChild(new TreeComboItem("تامین کننده عمده", 21)
                    .AddChild("تامین کارتن",    211)
                    .AddChild("تامین یخچال",    212)
                    .AddChild("تامین تلویزیون", 213))
                .AddChild("تامین کننده و پرسنل", 22),

            new TreeComboItem("پرسنل", 3) { IsRoot = true },
        };
        private void AddRoot()
          => MessageBox.Show("افزودن دسته ریشه جدید");
        private string FirstName;
        private string LastName;
        private string NationalId;
        private readonly IPersonApplication _personApplication;

        private string CompanyName;
        private string EconomicCode;
        private string RegistrationNumber;

        private bool isFirstSelected = true;
        private bool ValidatePerson()
        {
            if (isFirstSelected)
            {
                return !string.IsNullOrWhiteSpace(FirstName)
                    && !string.IsNullOrWhiteSpace(LastName)
                    && !string.IsNullOrWhiteSpace(NationalId);
            }
            else
            {
                return !string.IsNullOrWhiteSpace(CompanyName)
                    && !string.IsNullOrWhiteSpace(EconomicCode)
                    && !string.IsNullOrWhiteSpace(RegistrationNumber);
            }
        }
        private void AddChild(TreeComboItem parent)
            => MessageBox.Show($"افزودن زیرمجموعه برای: {parent.DisplayName}");

        private void Edit(TreeComboItem item)
            => MessageBox.Show($"ویرایش: {item.DisplayName}");

        private void Delete(TreeComboItem item)
            => MessageBox.Show($"حذف: {item.DisplayName}");

        private void cmbCategory_SelectionChanged(object sender, TreeComboItem selected)
            => MessageBox.Show($"انتخاب: {selected.DisplayName}  |  ID: {selected.Value}");

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object p) => _canExecute?.Invoke(p) ?? true;
            public void Execute(object p) => _execute(p);
            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
        public NewPersonView()
        {
            InitializeComponent();

            _personApplication = App.ServiceProvider
                .GetRequiredService<IPersonApplication>();
        }
        public ICommand SavePersonCommand { get; set; }

        private void SavePerson_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomConfirmDialog();

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SavePerson();
            }
        }

     
        private void SavePerson(object obj = null)
        {
            var dialog = new CustomConfirmDialog();

            if (dialog.ShowDialog() != true)
                return;

            var command = new CreatePerson
            {
                //FullName = FirstName + " " + LastName,

                //Add New
                FirstName = FirstName,
                LastName = LastName,

                //
                NationalCode = NationalId,

                EconomicCode = EconomicCode,
                RegistrationNumber = RegistrationNumber,

                IsLegal = !isFirstSelected
            };

            _personApplication.Create(command);
            MessageBox.Show("ثبت شخص با موفقیت انجام شد");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
        private void AddBankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var newAccount = new { Id = _bankAccountCounter++ };
            _bankAccounts.Add(new { Id = _bankAccountCounter++ });
        }
        private void RemoveBankAccount_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                _bankAccounts.Remove(button.Tag);
            }
        }
        private void Toggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            this.isFirstSelected = isFirstSelected;

            if (isFirstSelected)
            {
                FirstNamePanel.Visibility = Visibility.Visible;
                LastNamePanel.Visibility = Visibility.Visible;
                NationalCodePanel.Visibility = Visibility.Visible;

                CompanyNamePanel.Visibility = Visibility.Collapsed;
                EconomicCodePanel.Visibility = Visibility.Collapsed;
                RegistrationNumberPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                FirstNamePanel.Visibility = Visibility.Collapsed;
                LastNamePanel.Visibility = Visibility.Collapsed;
                NationalCodePanel.Visibility = Visibility.Collapsed;

                CompanyNamePanel.Visibility = Visibility.Visible;
                EconomicCodePanel.Visibility = Visibility.Visible;
                RegistrationNumberPanel.Visibility = Visibility.Visible;
            }
        }
        private void MyDatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null || !picker.SelectedDate.HasValue) return;

            DateTime selected = picker.SelectedDate.Value;
            Console.WriteLine(selected.ToString("yyyy/MM/dd"));
        }


        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as ToggleRadioControl;
            if (radio == null) return;

        }
        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
        }

        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabInventory == null) return;

            TabPricing.IsChecked = false;
            TabInventory.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Visible;
        }





        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabTax == null) return;

            TabPricing.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;
            TaxContent.Visibility = Visibility.Collapsed;
        }
        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabInventory == null || TabTax == null) return;

            TabInventory.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Collapsed;
        }


        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void CategoryToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (PersonnelToggle == null || TabTax == null) return;

            bool isPersonnel = PersonnelToggle.IsChecked == true;
            TabTax.Visibility = isPersonnel ? Visibility.Visible : Visibility.Collapsed;

            // اگه پرسنل غیرفعال شد ولی تب مالیات/پرسونل انتخاب شده بود، برگرد به تب پیش‌فرض
            if (!isPersonnel && TabTax.IsChecked == true)
            {
                TabPricing.IsChecked = true;
            }
        }
        private void ToggleSwitchControl_SelectionChanged(object sender, bool e)
        {

        }
    }
}