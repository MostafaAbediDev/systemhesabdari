using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Taadol.Models
{
    public class TransactionItem : INotifyPropertyChanged
    {
        private string _date;
        private string _amount;

        public string Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public string Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
