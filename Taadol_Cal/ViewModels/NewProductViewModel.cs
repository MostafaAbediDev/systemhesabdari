using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Taadol_Cal.Models;

namespace Taadol_Cal.ViewModels
{
    public class NewProductViewModel : INotifyPropertyChanged
    {
        private Product _currentProduct;

        public NewProductViewModel()
        {
            CurrentProduct = new Product
            {
                AccountCode = "1234",
                IsAutoCode = true,
                IsActive = true
            };

            Categories = new ObservableCollection<string>
            {
                "دسته‌بندی ۱",
                "دسته‌بندی ۲",
                "دسته‌بندی ۳"
            };

            Brands = new ObservableCollection<string>
            {
                "برند ۱",
                "برند ۲",
                "برند ۳"
            };

            SaveCommand = new RelayCommand(o => { /* ذخیره */ });
            SaveAndContinueCommand = new RelayCommand(o => { /* ثبت موقت */ });
            SelectImageCommand = new RelayCommand(o =>
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
                };
                if (dialog.ShowDialog() == true)
                    CurrentProduct.ImagePath = dialog.FileName;
            });
            DeleteImageCommand = new RelayCommand(o => CurrentProduct.ImagePath = null);
            AddCategoryCommand = new RelayCommand(o => { /* دسته جدید */ });
            AddBrandCommand = new RelayCommand(o => { /* برند جدید */ });
        }

        public Product CurrentProduct
        {
            get => _currentProduct;
            set { _currentProduct = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> Brands { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand SaveAndContinueCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand DeleteImageCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand AddBrandCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly System.Action<object> _execute;
        private readonly System.Func<object, bool> _canExecute;

        public RelayCommand(System.Action<object> execute, System.Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);

        public event System.EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
