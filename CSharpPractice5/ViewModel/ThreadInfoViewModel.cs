using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CSharpPractice5.Model;
using CSharpPractice5.Tools;
using BackgroundTask = System.Threading.Tasks.Task;
using System.Threading;
using System.Windows;
using CSharpPractice5.Tools.Loaders;
using CSharpPractice5.Tools.Managers;
using CSharpPractice5.Tools.Navigation;

namespace CSharpPractice5.ViewModel
{
    class ThreadInfoViewModel:BaseViewModel
    {
        #region fields

        private ObservableCollection<ThreadInfo> _threadInfos;
        private BackgroundTask _backgroundTask;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;

        private RelayCommand<object> _backCommand;

        #endregion

        #region properties

        public ObservableCollection<ThreadInfo> ThreadInfos
        {
            get
            {
                return _threadInfos;
            }

            set
            {
                _threadInfos = value;
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

        internal ThreadInfoViewModel()
        {
            _threadInfos = new ObservableCollection<ThreadInfo>(TryToGetThreads());
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
                if (ThreadInfos == null)
                {
                    LoaderManager.Instance.ShowLoader();
                }
                await  BackgroundTask.Run(() => LoadNewThreads());
                LoaderManager.Instance.HideLoader();
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(100);
                    if (_token.IsCancellationRequested)
                        break;
                }

                if (_token.IsCancellationRequested)
                    break;
                i++;
            }
        }

        private void LoadNewThreads()
        {
            ThreadInfos = new ObservableCollection<ThreadInfo>(TryToGetThreads());
        }

        private static List<ThreadInfo> TryToGetThreads()
        {
            Task selected = TaskListManager.GetSelected();
            int id = selected.ProcessId;
            try
            {
               return ThreadListLoader.GetThreadsInfos(id);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return new List<ThreadInfo>();
        }

        internal void StopBackgroundTask()
        {
            if (_backgroundTask == null)
            {
                return;
            }
            _tokenSource.Cancel();
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
            ThreadInfos = null;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartBarckgroundTask();
        }
    }
}
