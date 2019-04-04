using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CSharpPractice5.Model;
using DateTime = System.DateTime;

namespace CSharpPractice5.Tools.Loaders
{
    internal static class TaskListLoader
    {
        internal static List<Task> GetCurrentTasksAndCheckForBreak(CancellationToken cancellationToken)
        {
            List<Task> res = new List<Task>();
            return GetCurrentTasks(Process.GetProcesses(), cancellationToken);
        }

        private static List<Task> GetCurrentTasks(Process[] processes, CancellationToken cancellationToken)
        {
            List<Task> res = new List<Task>();
            for (int i = 0; i < processes.Length; i++)
            {
                res.Add(GetTask(processes[i]));
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
            return res;
        }

        private static Task GetTask(Process process)
        {
            string cpuMeasure = TryToGetCpuMeasure(process).ToString();
            dynamic extraProcessInfo = ProcessExtraInfoLoader.GetProcessExtraInformation(process.Id);
            long ramBytes = process.PagedMemorySize64;
            string ramPercentage = TryToGetRamPercentage(ramBytes);
            string ramMeasure = TryToGetRamMeasure(ramBytes);
            string processOwner = TryToGetProcessOwner(extraProcessInfo);
            DateTime startDateTime = TryToGetDateTime(process);
            Task task = new Task(process.ProcessName, process.Id, process.Responding, cpuMeasure, ramPercentage, ramMeasure, process.Threads.Count,processOwner, startDateTime.ToShortDateString());
            return task;
        }

        private static double TryToGetCpuMeasure(Process process)
        {
            PerformanceCounter counter = null;
            double res = 0;
            try
            {
                counter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
                res = counter.NextValue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (counter != null)
            {
                counter.Dispose();
            }
            return res;
        }

        private static string TryToGetRamMeasure(long bytes)
        {
           
            string res = " - ";
            try
            {
                res = BytesToReadableValue(bytes);
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            return res;
        }

        private static string TryToGetRamPercentage(long proccessRamMeasure)
        {
            string res = " - ";
            try
            {
                long total = TotalMachineMemory();
                res = (Math.Round((proccessRamMeasure / (total+.0)) * 100, 3)).ToString();
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            return res;
        }

        private static Int64 TotalMachineMemory()
        {
           return PerformanceInfoLoader.GetPhysicalAvailableMemory();
        }

        private static string TryToGetProcessOwner(dynamic extraProccessInfo)
        {
            try
            {
                return extraProccessInfo.Username;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return " - ";
        }

        private static DateTime TryToGetDateTime(Process process)
        {
            try
            {
                return process.StartTime;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return default(DateTime);
        }

        private static string BytesToReadableValue(long number)
        {
            List<string> suffixes = new List<string> { " B", " KB", " MB", " GB", " TB", " PB" };
            for (int i = 0; i < suffixes.Count; i++)
            {
                long temp = number / (int)Math.Pow(1024, i + 1);
                if (temp == 0)
                {
                    return (number / (int)Math.Pow(1024, i)) + suffixes[i];
                }
            }
            return number.ToString();
        }

        internal static Process GetProcessById(int processId)
        {
            return Process.GetProcessById(processId);
        }
    }
}
