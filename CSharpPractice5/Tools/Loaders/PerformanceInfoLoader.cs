using System;
using System.Runtime.InteropServices;

namespace CSharpPractice5.Tools.Loaders
{
   static  class PerformanceInfoLoader
   {
       private static long _physicalMemory = Int64.MinValue;
    
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemory()
        {
            if (_physicalMemory != Int64.MinValue)
            {
                return _physicalMemory;
            }
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                _physicalMemory = Convert.ToInt64((pi.PhysicalAvailable.ToInt64()*pi.PageSize.ToInt64()));
            }
            else
            {
                _physicalMemory = - 1;
            }
            return _physicalMemory;
        }
    }
}
