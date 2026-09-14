namespace Taadol.Models
{

    public interface IListRowItem
    {
        bool IsSelected { get; set; }
        bool IsEmpty { get; }
    }
}