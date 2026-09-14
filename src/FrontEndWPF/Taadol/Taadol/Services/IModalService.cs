using System.Windows.Controls;

namespace Taadol.Services
{

    public interface IModalService
    {

        void Open(UserControl view);

        void Close();

        bool IsOpen { get; }

        UserControl? Current { get; }
    }
}