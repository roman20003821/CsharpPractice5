using System;

namespace CSharpPractice5.Model
{
  internal  class ThreadInfo
  {
        #region fields

        private int _threadId;
        private string _state;
        private string _startDate;

        #endregion

        #region properties

        public int ThreadId
        {
            get
            {
                return _threadId;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
        }

        public string StartDate
        {
            get { return _startDate; }
        }

        internal ThreadInfo(int threadId, string state, string startDate)
        {
            _threadId = threadId;
            _state = state;
            _startDate = startDate == default(DateTime).ToShortDateString()?"-":startDate;
        }

        #endregion

    }
}
