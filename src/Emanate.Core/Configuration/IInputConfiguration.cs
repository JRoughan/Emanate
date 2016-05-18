namespace Emanate.Core.Configuration
{
    public interface IInputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }
        string Name { get; }
    }
}