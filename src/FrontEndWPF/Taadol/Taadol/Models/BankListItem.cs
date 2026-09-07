using System.ComponentModel;
using Taadol.Controls;

namespace Taadol.Models
{
    public sealed class BankItem : IListRowItem, INotifyPropertyChanged
    {
        private int _rowNumber;
        private bool _isSelected;

        public long Id { get; set; }
        public string UniqueId { get; set; } = string.Empty;

        public int RowNumber
        {
            get => _rowNumber;
            set
            {
                if (_rowNumber == value)
                    return;

                _rowNumber = value;
                OnPropertyChanged(nameof(RowNumber));
                OnPropertyChanged(nameof(RowNumberDisplay));
            }
        }

        public string Title { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string BankType { get; set; } = string.Empty;
        public string RegisterDate { get; set; } = string.Empty;
        public bool IsEmpty { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string RowNumberDisplay => RowNumber > 0 && !IsEmpty
            ? ToPersianNumber(RowNumber)
            : string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static string ToPersianNumber(int number)
        {
            var digits = new[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            var result = string.Empty;

            foreach (var character in number.ToString())
                result += digits[int.Parse(character.ToString())];

            return result;
        }
    }

    public sealed class BankPageItem
    {
        public int PageNumber { get; set; }
        public string PageNumberDisplay { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
    }
}
