namespace RavenTrafficGeneratingTool.Services
{
    public interface IConfigurationService
    {
        string GetValue(string key);
        void SetValue(string key, string value);
    }
}
