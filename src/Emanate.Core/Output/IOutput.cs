using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutput
    {
        void UpdateStatus(BuildState state);
    }
}