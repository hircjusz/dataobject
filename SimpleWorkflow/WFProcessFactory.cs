using System.Collections.Generic;
using System.IO;
using SoftwareMind.SimpleWorkflow.Misc;
using System.Threading;
using System;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public static class WFProcessFactory
    {
        private static ReaderWriterLockSlim readWriterLock = new ReaderWriterLockSlim();
        internal static IDictionary<string, WFProcess> processes = new Dictionary<string, WFProcess>();
        private static object lockobj = new object();

        public static WFProcess GetProcess(string name, string ns = null)
        {
            var fullName = (ns == null ? "" : ns + "::") + name;
            var fullPath = ns == null ? name : Path.Combine(ns, name);

            lock (lockobj)
            {
                WFProcess process = null;

                if (processes.TryGetValue(fullName, out process))
                {
                    return process;
                }

                process = LoadProcess(ProcessTemplate.GetTemplatePathByName(fullPath));
                processes.Add(fullName, process);
                return process;
            }
        }

        public static WFProcess GetProcessFromXml(string xml, string ns = null)
        {
            try
            {
                readWriterLock.EnterUpgradeableReadLock();

                WFProcess process = null;
                if (processes.TryGetValue(xml, out process))
                    return process;

                try
                {
                    readWriterLock.EnterWriteLock();

                    process = LoadProcessFromXml(xml);

                    string fullName = (ns == null ? "" : ns + "::") + process.Name;
                    if (!processes.ContainsKey(fullName))
                    {
                        processes.Add(fullName, process);
                    }
                    return process;
                }
                finally
                {
                    readWriterLock.ExitWriteLock();
                }
            }
            finally
            {
                readWriterLock.ExitUpgradeableReadLock();
            }
        }

        public static WFProcess LoadProcess(string filename, bool substituteTemplate = true)
        {
            return ProcessTemplate.Load(filename, substituteTemplate);
        }

        public static WFProcess LoadProcessFromXml(string xml, bool substituteTemplate = true)
        {
            return ProcessTemplate.LoadFromXml(xml, substituteTemplate);
        }

        // template -> disk :)
        public static void SaveProcess(string filename, WFProcess process)
        {
            ProcessTemplate.Save(filename, process);
        }

        public static void ClearCache()
        {
            readWriterLock.EnterWriteLock();
            processes.Clear();
            readWriterLock.ExitWriteLock();
        }
    }
}
