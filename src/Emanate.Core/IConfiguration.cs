namespace Emanate.Core
{
    public interface IConfiguration
    {
        string GetString(string key);
        bool GetBool(string key);
        int GetInt(string key);
    }
}
