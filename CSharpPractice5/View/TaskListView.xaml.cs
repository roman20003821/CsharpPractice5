using System.Windows.Controls;
using CSharpPractice5.Tools.Navigation;
using CSharpPractice5.ViewModel;
using CSharpPractice5.Tools;

namespace CSharpPractice5.View
{
    /// <summary>
    /// Interaction logic for TaskListView.xaml
    /// </summary>
    public partial class TaskListView : UserControl, INavigatable
    {
        public TaskListView()
        {
            InitializeComponent();
            DataContext = new TaskListViewModel();
        }

        public void OnReOpen()
        {
            ((BaseViewModel)DataContext).OnReOpen();
        }
    }
}
