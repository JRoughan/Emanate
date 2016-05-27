namespace Emanate.Core.Configuration
{
    public interface IOriginator
    {
        Memento CreateMemento();
        void SetMemento(Memento memento);
    }
}