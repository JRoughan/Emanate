namespace Emanate.Core.Configuration
{
    public interface IConfigurationGenerator
    {
        T Generate<T>();
    }
}
