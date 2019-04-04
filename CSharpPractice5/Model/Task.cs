using System;

namespace CSharpPractice5.Model
{
    internal class Task
    {
        #region fields

        private string _processName;
        private int _processId;
        private bool _isActive;
        private string _cpuMeasure;
        private string _ramPercent;
        private string _ramVolume;
        private int _threadsNum;
        private string _user;
        private string _startDate;

        #endregion

        #region properties

        public string ProcessName
        {
            get
            {
                return _processName;
            }
        }

        public int ProcessId
        {
            get
            {
                return _processId;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
        }

        public string CpuMeasure
        {
            get
            {
                return _cpuMeasure;
            }
        }

        public string RamPercent
        {
            get
            {
                return _ramPercent;
            }
        }

        public string RamVolume
        {
            get
            {
                return _ramVolume;
            }
        }

        public int ThreadsNum
        {
            get
            {
                return _threadsNum;
            }
        }

        public string User
        {
            get
            {
                return _user;
            }
        }

        public string StartDate
        {
            get
            {
                return _startDate;
            }
        }

        #endregion

        internal Task(string processName, int processId, bool isActive, string cpuMeasure, string ramPercent, string ramVolume, int threadsNum, string user, string startDate)
        {
            _processName = processName;
            _processId = processId;
            _isActive = isActive;
            _cpuMeasure = cpuMeasure;
            _ramPercent = ramPercent;
            _ramVolume = ramVolume;
            _threadsNum = threadsNum;
            _user = user;
            _startDate = startDate == default(DateTime).ToShortDateString()?"-":startDate;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType()) return false;

            Task task = (Task)obj;
            return ProcessId == task.ProcessId;
        }

    }
}
