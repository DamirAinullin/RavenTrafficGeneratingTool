using RavenTrafficGeneratingTool.ViewModel;

namespace RavenTrafficGeneratingTool
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}