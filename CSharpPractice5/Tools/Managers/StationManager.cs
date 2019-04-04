using System;
using System.Windows;

namespace CSharpPractice5.Tools.Managers
{
    class StationManager
    {
        public static event Action StopThreads;

        internal static void CloseApp()
        {
            MessageBox.Show("ShutDown");
            StopThreads?.Invoke();
            Environment.Exit(1);
        }
    }
}
