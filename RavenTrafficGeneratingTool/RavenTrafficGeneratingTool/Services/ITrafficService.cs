namespace RavenTrafficGeneratingTool.Services
{
    public interface ITrafficService
    {
        void Start(string databaseUrl, int timesPerMinute);
        void Stop();
    }
}
