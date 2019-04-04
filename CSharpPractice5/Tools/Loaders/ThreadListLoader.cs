using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSharpPractice5.Model;

namespace CSharpPractice5.Tools.Loaders
{
    static class ThreadListLoader
    {
        internal static List<ThreadInfo> GetThreadsInfos(int processId)
        {
            ProcessThreadCollection threadCollection = GetThreads(processId);
            return GetThreadsInfos(threadCollection);
        }

        private static ProcessThreadCollection GetThreads(int processId)
        {
            return TaskListLoader.GetProcessById(processId).Threads;
        }

        private static List<ThreadInfo> GetThreadsInfos(ProcessThreadCollection threadCollection)
        {
            List<ThreadInfo> res = new List<ThreadInfo>();
            foreach (ProcessThread thread in threadCollection)
            {
                res.Add(GetThreadInfo(thread));
            }
            return res;
        }

        private static ThreadInfo GetThreadInfo(ProcessThread thread)
        {
            return new ThreadInfo(thread.Id, thread.ThreadState.ToString(), TryToGetDateTime(thread).ToShortDateString());
        }

        private static DateTime TryToGetDateTime(ProcessThread thread)
        {
            try
            {
                return thread.StartTime;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return default(DateTime);
        }
    }
}
