namespace Emanate.Core
{
    public interface IModule
    {
        string Key { get; }
        string Name { get; }

        Direction Direction { get; }
    }
}