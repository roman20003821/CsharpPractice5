using System;
using System.Windows.Controls;
using CSharpPractice5.Tools.Navigation;
using CSharpPractice5.ViewModel;
using CSharpPractice5.Tools;

namespace CSharpPractice5.View
{
    /// <summary>
    /// Interaction logic for TaskInfoView.xaml
    /// </summary>
    public partial class ThreadInfoView : UserControl,INavigatable
    {
        public ThreadInfoView()
        {
            InitializeComponent();
            DataContext = new ThreadInfoViewModel();
        }

        public void OnReOpen()
        {
            ((BaseViewModel)DataContext).OnReOpen();
        }
    }
}
