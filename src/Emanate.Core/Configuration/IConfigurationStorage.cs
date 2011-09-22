namespace Emanate.Core.Configuration
{
    public interface IConfigurationStorage
    {
        string GetString(string key);
        bool GetBool(string key);
        int GetInt(string key);
    }
}