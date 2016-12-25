using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RavenTrafficGeneratingTool.Services;

namespace RavenTrafficGeneratingTool.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IConfigurationService, ConfigurationService>();
            SimpleIoc.Default.Register<ITrafficService, TrafficService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
            if (SimpleIoc.Default.IsRegistered<MainViewModel>())
            {
                SimpleIoc.Default.Unregister<MainViewModel>();
            }
        }
    }
}