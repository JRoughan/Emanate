namespace Emanate.Core.Configuration
{
    public interface IInputConfiguration : IOriginator
    {
        string Key { get; }
        string Name { get; }
    }
}