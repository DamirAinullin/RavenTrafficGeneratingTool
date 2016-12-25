using GalaSoft.MvvmLight.Threading;

namespace RavenTrafficGeneratingTool
{
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
