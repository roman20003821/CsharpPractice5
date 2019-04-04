using System.Windows.Controls;
using CSharpPractice5.Tools;
using CSharpPractice5.Tools.Navigation;
using CSharpPractice5.ViewModel;

namespace CSharpPractice5.View
{
    /// <summary>
    /// Interaction logic for ModuleListView.xaml
    /// </summary>
    public partial class ModuleListView : UserControl, INavigatable
    {
        public ModuleListView()
        {
            InitializeComponent();
            DataContext = new ModuleListViewModel();
        }

        public void OnReOpen()
        {
            ((BaseViewModel)DataContext).OnReOpen();
        }
    }
}
