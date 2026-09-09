using CommunityToolkit.Mvvm.ComponentModel;

namespace Taadol.ViewModels
{
    public sealed class BankTypeOption : ObservableObject
    {
        private bool _isSelected;

        public BankTypeOption(long id, string title)
        {
            Id = id;
            Title = title ?? string.Empty;
        }

        public long Id { get; }
        public string Title { get; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
