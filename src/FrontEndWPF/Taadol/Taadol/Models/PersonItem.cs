using System.Windows;

public class PersonItem : System.ComponentModel.INotifyPropertyChanged
{
    private int _rowNumber;

    public int RowNumber
    {
        get => _rowNumber;
        set
        {
            _rowNumber = value;
            PropertyChanged?.Invoke(this,
                new System.ComponentModel.PropertyChangedEventArgs(nameof(RowNumber)));
        }
    }

    public string Category { get; set; }   
    public string FullName { get; set; }
    public string Company { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string Mobile { get; set; }
    public string Phone { get; set; }
    public string NationalCode { get; set; }
    public string EconomicCode { get; set; }
    public string AccountingStatus { get; set; }
    public bool IsEmpty { get; set; }

    public Visibility AccountingStatusVisibility =>
        IsEmpty || string.IsNullOrEmpty(AccountingStatus)
            ? Visibility.Collapsed
            : Visibility.Visible;

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
}