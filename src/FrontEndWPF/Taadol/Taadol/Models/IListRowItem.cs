namespace Taadol.Models
{
    /// <summary>
    /// قراردادی برای ردیف‌های گرید مشترک (UnifiedListView).
    /// هر مدل ردیف در فرم‌های لیست باید این واسط را پیاده‌سازی کند
    /// تا انتخاب با چک‌باکس و شماره ردیف به یک شکل کار کند.
    /// </summary>
    public interface IListRowItem
    {
        bool IsSelected { get; set; }
        bool IsEmpty { get; }
    }
}