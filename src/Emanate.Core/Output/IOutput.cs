using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutput
    {
        void UpdateStatus(object sender, StatusChangedEventArgs e);
    }
}