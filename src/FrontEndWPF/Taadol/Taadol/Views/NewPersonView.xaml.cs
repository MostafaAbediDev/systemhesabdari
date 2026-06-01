using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewPersonView : UserControl

    {

        private int _bankAccountCounter = 0;
        private ObservableCollection<object> _bankAccounts = new ObservableCollection<object>();
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
            AdditionalBankAccountsPanel.ItemsSource = _bankAccounts;
            cmbCategory.ItemsSource = BuildTree();
            cmbCategory.AddRootCommand = new RelayCommand(_ => AddRoot());
            cmbCategory.AddChildCommand = new RelayCommand(item => AddChild((TreeComboItem)item));
            cmbCategory.EditCommand = new RelayCommand(item => Edit((TreeComboItem)item));
            cmbCategory.DeleteCommand = new RelayCommand(item => Delete((TreeComboItem)item));
                                                                                                                                                                                                                                                            
                                }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
        private void AddBankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var newAccount = new { Id = _bankAccountCounter++ };
            _bankAccounts.Add(newAccount);
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

        private void ToggleSwitchControl_SelectionChanged(object sender, bool e)
        {

        }
    }
}