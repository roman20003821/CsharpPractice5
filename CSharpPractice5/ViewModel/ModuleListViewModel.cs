using CSharpPractice5.Model;
using CSharpPractice5.Tools;
using CSharpPractice5.Tools.Managers;
using System.Collections.ObjectModel;
using System.Threading;
using CSharpPractice5.Tools.Loaders;
using BackgroundTask = System.Threading.Tasks.Task;
using System;
using System.Collections.Generic;
using System.Windows;
using CSharpPractice5.Tools.Navigation;

namespace CSharpPractice5.ViewModel
{
    class ModuleListViewModel:BaseViewModel
    {
        #region fields

        private ObservableCollection<Module> _modules;
        private BackgroundTask _backgroundTask;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;

        private RelayCommand<object> _backCommand;

        #endregion

        #region properties

        public ObservableCollection<Module> Modules
        {
            get
            {
                return _modules;
            }

            set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<object> BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand<object>(
                           Back, o => true));
            }
        }

        #endregion

        internal ModuleListViewModel()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartBarckgroundTask();
            StationManager.StopThreads += StopBackgroundTask;
        }

        private void StartBarckgroundTask()
        {
            _backgroundTask = BackgroundTask.Factory.StartNew(BackgroundTaskProcess, System.Threading.Tasks.TaskCreationOptions.LongRunning);
        }

        private async void BackgroundTaskProcess()
        {
            int i = 0;
            while (!_token.IsCancellationRequested)
            {
                await BackgroundTask.Run(() => LoadNewModules());
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }

                if (_token.IsCancellationRequested)
                    break;
                i++;
            }
        }

        private List<Module> TryToGetModules()
        {
            try
            {
                int processId = TaskListManager.GetSelected().ProcessId;
                return ModuleListLoader.GetModuleList(processId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                StopBackgroundTask();
            }
            return new List<Module>();
        }

        private void LoadNewModules()
        {
           Modules = new ObservableCollection<Module>(TryToGetModules());
        }

        internal void StopBackgroundTask()
        {
            _tokenSource.Cancel();
            if (_backgroundTask == null)
            {
                return;
            }
            _backgroundTask.Wait(2000);
            _backgroundTask.Dispose();
            _backgroundTask = null;
        }

        private void Back(object obj)
        {
            StopBackgroundTask();
            NavigationManager.Instance.Navigate(ViewType.TaskManager);
        }

        public override void OnReOpen()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartBarckgroundTask();
        }
    }
}
