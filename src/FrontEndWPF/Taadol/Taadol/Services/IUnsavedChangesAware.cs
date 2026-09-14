namespace Taadol
{

    public interface IUnsavedChangesAware
    {
        bool HasUnsavedChanges { get; }
    }
}
