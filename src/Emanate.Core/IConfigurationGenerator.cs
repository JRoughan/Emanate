namespace Emanate.Core
{
    public interface IConfigurationGenerator
    {
        T Generate<T>();
    }

    public interface IConfigurationStorage
    {
        string GetString(string key);
        bool GetBool(string key);
        int GetInt(string key);
    }
}
