using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CSharpPractice5.Model;
using CSharpPractice5.Tools;
using BackgroundTask = System.Threading.Tasks.Task;
using CSharpPractice5.Tools.Managers;
using CSharpPractice5.Tools.Loaders;
using System.Threading;
using System.Windows;
using CSharpPractice5.Tools.Navigation;

namespace CSharpPractice5.ViewModel
{
    internal class TaskListViewModel:BaseViewModel, ITaskListOwner
    {
        #region fields

        private ObservableCollection<Task> _taskList;
        private BackgroundTask _backgroundTask;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;

        private bool _ascendingSort;
        private bool _descendingSort;
        private bool _nameSort;
        private bool _idSort;
        private bool _activeSort;
        private bool _cpuSort;
        private bool _ramPercentSort;
        private bool _ramVolumeSort;
        private bool _threadsNumSort;
        private bool _userSort;
        private bool _startDateSort;

        private bool _sortPressed;

        private Task _selected;

        private RelayCommand<object> _sortCommand;
        private RelayCommand<object> _resetCommand;
        private RelayCommand<object> _showThreadsCommand;
        private RelayCommand<object> _showModulesCommand;
        private RelayCommand<object> _stopProcessCommand;
        private RelayCommand<object> _openFolderCommand;

        #endregion

        #region properties

        public ObservableCollection<Task> TaskList
        {
            get
            {
                return _taskList;
            }
            set
            {
                _taskList = value;
                OnPropertyChanged();
            }
        }

        public bool DescendingSort
        {
            get
            {
                return _descendingSort;
            }

            set
            {
                _descendingSort = value;
                OnPropertyChanged();
            }
        }

        public bool AscendingSort
        {
            get
            {
                return _ascendingSort;
            }

            set
            {
                _ascendingSort = value;
                OnPropertyChanged();
            }
        }

        public bool NameSort
        {
            get
            {
                return _nameSort;
            }

            set
            {
                _nameSort = value;
                OnPropertyChanged();
            }
        }

        public bool IdSort
        {
            get
            {
                return _idSort;
            }

            set
            {
                _idSort = value;
                OnPropertyChanged();
            }
        }

        public bool ActiveSort
        {
            get
            {
                return _activeSort;
            }

            set
            {
                _activeSort = value;
                OnPropertyChanged();
            }
        }

        public bool CpuSort
        {
            get
            {
                return _cpuSort;
            }

            set
            {
                _cpuSort = value;
                OnPropertyChanged();
            }
        }

        public bool RamPercentSort
        {
            get
            {
                return _ramPercentSort;
            }

            set
            {
                _ramPercentSort = value;
                OnPropertyChanged();
            }
        }

        public bool RamVolumeSort
        {
            get
            {
                return _ramVolumeSort;
            }

            set
            {
                _ramVolumeSort = value;
                OnPropertyChanged();
            }
        }

        public bool ThreadsNumSort
        {
            get
            {
                return _threadsNumSort;
            }

            set
            {
                _threadsNumSort = value;
                OnPropertyChanged();
            }
        }

        public bool UserSort
        {
            get
            {
                return _userSort;
            }

            set
            {
                _userSort = value;
                OnPropertyChanged();
            }
        }

        public bool StartDateSort
        {
            get
            {
                return _startDateSort;
            }

            set
            {
                _startDateSort = value;
                OnPropertyChanged();
            }
        }

        public Task Selected
        {
            get
            {
             return _selected;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                _selected = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<object> SortCommand
        {
            get
            {
                return _sortCommand ?? (_sortCommand = new RelayCommand<object>(
                           Sort, CanBeSorted));
            }
        }

        public RelayCommand<object> ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new RelayCommand<object>(
                           Reset, o => true));
            }
        }

        public RelayCommand<object> ShowThreadsCommand
        {
            get
            {
                return _showThreadsCommand ?? (_showThreadsCommand = new RelayCommand<object>(
                           ShowThreads,CanExecuteOptionWithProcess));
            }
        }


        public RelayCommand<object> ShowModulesCommand
        {
            get
            {
                return _showModulesCommand ?? (_showModulesCommand = new RelayCommand<object>(
                           ShowModules, CanExecuteOptionWithProcess));
            }
        }

        public RelayCommand<object> StopProcessCommand
        {
            get
            {
                return _stopProcessCommand ?? (_stopProcessCommand = new RelayCommand<object>(
                           Stop, CanExecuteOptionWithProcess));
            }
        }

        public RelayCommand<object> OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand = new RelayCommand<object>(
                           OpenFolder, CanExecuteOptionWithProcess));
            }
        }

        #endregion


        internal TaskListViewModel()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartBarckgroundTask();
            StationManager.StopThreads += StopBackgroundTask;
            TaskListManager.Initialize(this);
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
                if (_taskList == null)
                {
                    LoaderManager.Instance.ShowLoader();
                }
                await BackgroundTask.Run(() => LoadNewTasks());
                LoaderManager.Instance.HideLoader();
                if (_token.IsCancellationRequested)
                    break;
                i++;
            }
        }

        private void LoadNewTasks()
        {
            Task selected = _selected;
            var tasks = TaskListLoader.GetCurrentTasksAndCheckForBreak(_token);
            if (_sortPressed) tasks = GetSorted(tasks);
            TaskList = new ObservableCollection<Task>(tasks);
            Selected = TaskList.ToList().FirstOrDefault(task => task.Equals(selected));
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

        private void Sort(object obj)
        {
            _sortPressed = true;
            var tasks = _taskList.ToList();
            TaskList = new ObservableCollection<Task>(GetSorted(tasks));
        }

        private bool CanBeSorted(object obj)
        {
            if (!DescendingSort && !AscendingSort) return false;
            return (IdSort || NameSort || CpuSort || RamPercentSort || RamVolumeSort || ThreadsNumSort || UserSort ||
                    ActiveSort || StartDateSort);

        }

        private List<Task> GetSorted(List<Task> taskList)
        {
            if (AscendingSort)
            {
                if (NameSort)
                {
                    taskList.Sort((task, task1) => task.ProcessName.CompareTo(task1.ProcessName));
                }
                else if(IdSort)
                {
                    taskList.Sort((task, task1) => task.ProcessId.CompareTo(task1.ProcessId));
                }
                else if (ActiveSort)
                {
                    taskList.Sort((task, task1) => task.IsActive.CompareTo(task1.IsActive));
                }
                else if (CpuSort)
                {
                    taskList.Sort((task, task1) => task.CpuMeasure.CompareTo(task1.CpuMeasure));
                }
                else if (RamPercentSort)
                {
                    taskList.Sort((task, task1) => task.RamPercent.CompareTo(task1.RamPercent));
                }
                else if (RamVolumeSort)
                {
                    taskList.Sort((task, task1) => task.RamVolume.CompareTo(task1.RamVolume));
                }
                else if (ThreadsNumSort)
                {
                    taskList.Sort((task, task1) => task.ThreadsNum.CompareTo(task1.ThreadsNum));
                }
                else if (UserSort)
                {
                    taskList.Sort((task, task1) => task.User.CompareTo(task1.User));
                }
                else if (StartDateSort)
                {
                    taskList.Sort((task, task1) => task.StartDate.CompareTo(task1.StartDate));
                }
            }
            else if(DescendingSort)
            {
                if (NameSort)
                {
                    taskList.Sort((task, task1) => task1.ProcessName.CompareTo(task.ProcessName));
                }
                else if (IdSort)
                {
                    taskList.Sort((task, task1) => task1.ProcessId.CompareTo(task.ProcessId));
                }
                else if (ActiveSort)
                {
                    taskList.Sort((task, task1) => task1.IsActive.CompareTo(task.IsActive));
                }
                else if (CpuSort)
                {
                    taskList.Sort((task, task1) => task1.CpuMeasure.CompareTo(task.CpuMeasure));
                }
                else if (RamPercentSort)
                {
                    taskList.Sort((task, task1) => task1.RamPercent.CompareTo(task.RamPercent));
                }
                else if (RamVolumeSort)
                {
                    taskList.Sort((task, task1) => task1.RamVolume.CompareTo(task.RamVolume));
                }
                else if (ThreadsNumSort)
                {
                    taskList.Sort((task, task1) => task1.ThreadsNum.CompareTo(task.ThreadsNum));
                }
                else if (UserSort)
                {
                    taskList.Sort((task, task1) => task1.User.CompareTo(task.User));
                }
                else if (StartDateSort)
                {
                    taskList.Sort((task, task1) => task1.StartDate.CompareTo(task.StartDate));
                }

            }   
            return taskList;
        }

        private async void Reset(object obj)
        {
            _sortPressed = false;
            AscendingSort = false;
            DescendingSort = false;
            NameSort = false;
            IdSort = false;
            ActiveSort = false;
            CpuSort = false;
            RamPercentSort = false;
            RamVolumeSort = false;
            ThreadsNumSort = false;
            UserSort = false;
            StartDateSort = false;
            LoaderManager.Instance.ShowLoader();
            await BackgroundTask.Run(() => LoadNewTasks());
            LoaderManager.Instance.HideLoader();
        }

        private void ShowThreads(object obj)
        {
            StopBackgroundTask();
            NavigationManager.Instance.Navigate(ViewType.ThreadInfo);
        }

        private void ShowModules(object obj)
        {
            StopBackgroundTask();
            NavigationManager.Instance.Navigate(ViewType.Modules);
        }

        private async void Stop(object obj)
        {
            try
            {
                TaskListLoader.GetProcessById(_selected.ProcessId).Kill();
                LoaderManager.Instance.ShowLoader();
                await BackgroundTask.Run(() => LoadNewTasks());
                LoaderManager.Instance.HideLoader();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OpenFolder(object obj)
        {
            try
            {
                Process process = TaskListLoader.GetProcessById(_selected.ProcessId);
                string fileName =  process.StartInfo.FileName;
                Process.Start(fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool CanExecuteOptionWithProcess(object obj)
        {
            return _selected != null;
        }

        public override void OnReOpen()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartBarckgroundTask();
        }
    }
}
