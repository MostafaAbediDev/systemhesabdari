using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Taadol.Models
{
    public class BankAccountItem : INotifyPropertyChanged
    {
        private string _bankName;
        private string _branchName;
        private string _cardNumber;
        private string _shebaNumber;
        private string _accountNumber;
        private bool _isDefault;

        public string BankName
        {
            get => _bankName;
            set { _bankName = value; OnPropertyChanged(); }
        }

        public string BranchName
        {
            get => _branchName;
            set { _branchName = value; OnPropertyChanged(); }
        }

        public string CardNumber
        {
            get => _cardNumber;
            set { _cardNumber = value; OnPropertyChanged(); }
        }

        public string ShebaNumber
        {
            get => _shebaNumber;
            set { _shebaNumber = value; OnPropertyChanged(); }
        }

        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        public bool IsDefault
        {
            get => _isDefault;
            set { _isDefault = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}