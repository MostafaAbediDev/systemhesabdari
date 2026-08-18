namespace Taadol
{
    /// <summary>
    /// فرم‌هایی که تغییرات ذخیره‌نشده را ردیابی می‌کنند تا قبل از ناوبری/بستن
    /// بتوان به کاربر هشدار داد (جلوگیری از از دست رفتن بی‌صدا اطلاعات).
    /// </summary>
    public interface IUnsavedChangesAware
    {
        bool HasUnsavedChanges { get; }
    }
}
