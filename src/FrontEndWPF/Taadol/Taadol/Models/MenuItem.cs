// Models/MenuItem.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaadolAccounting.Models
{
    public class MenuItem : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isSelected;

        public string Title { get; set; }
        public string IconPath { get; set; }  
        public ObservableCollection<SubMenuItem> SubItems { get; set; } = new ObservableCollection<SubMenuItem>();

        public bool HasSubItems => SubItems?.Count > 0;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SubMenuItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isBold;

        public string Title { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsBold
        {
            get => _isBold;
            set
            {
                _isBold = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}