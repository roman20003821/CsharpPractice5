using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSharpPractice5.Model;

namespace CSharpPractice5.Tools.Loaders
{
    internal static class ModuleListLoader
    {
        internal static List<Module> GetModuleList(int processId)
        {
            Process process = Process.GetProcessById(processId);
            return GetModuleList(process);
        }

        private static List<Module> GetModuleList(Process process)
        {
            List<Module> res = new List<Module>();
            ProcessModuleCollection collection = TryToGetModules(process);
            foreach (ProcessModule processModule in collection)
            {
                res.Add(GetModule(processModule));
            }
            return res;
        }

        private static ProcessModuleCollection TryToGetModules(Process process)
        {
            try
            {
                return process.Modules;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
         }

        private static Module GetModule(ProcessModule processModule)
        {
            return new Module(processModule.ModuleName, processModule.FileName);
        }
    }
}
