using CSharpPractice5.Tools.Managers;
using CSharpPractice5.Tools.Navigation;
using CSharpPractice5.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CSharpPractice5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IContentOwner
    {
        public ContentControl ContentControl
        {
            get { return _contentControl; }
        }

        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            NavigationManager.Instance.Initialize(new InitializationNavigationModel(this));
            NavigationManager.Instance.Navigate(ViewType.TaskManager);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            StationManager.CloseApp();
        }
    }
}
