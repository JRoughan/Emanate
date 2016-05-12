using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Emanate.Extensibility
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Task<InitializationResult> Initialize() { return Task.FromResult(InitializationResult.NoneRequired); }
    }

    public enum InitializationResult
    {
        Unknown = 0,
        NoneRequired,
        Failed,
        Succeeded
    }
}