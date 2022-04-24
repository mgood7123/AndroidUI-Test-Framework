using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using MeasureMap.Diagnostics;

namespace MeasureMap
{
    internal static class ThreadHelper
    {
        private static readonly bool IsRunningOnMono;

        static ThreadHelper()
        {
            IsRunningOnMono = Type.GetType("Mono.Runtime") != null;
        }

        public static System.Threading.Tasks.Task<Result> QueueTask(int index, Func<int, Result> action)
        {
            var task = new System.Threading.Tasks.Task<Result>(() =>
            {
                SetThreadAffinity(index);
                SetThreadPriority();

                var result = action.Invoke(index);

                EndThreadAffinity();

                return result;
            });

            return task;
        }

        private static void SetThreadAffinity(int index)
        {
            if (IsRunningOnMono)
            {
                return;
            }

            try
            {
                Thread.BeginThreadAffinity();

                var affinity = GetAffinity(index + 1, Environment.ProcessorCount);
                var thread = GetCurrentThread();

                thread.ProcessorAffinity = new IntPtr(1 << affinity);
                //thread.ProcessorAffinity = new IntPtr(2);
            }
            catch (Exception)
            {
                var logger = Logger.Setup();
                logger.Write($"Could not set Task to run on second Core or Processor", LogLevel.Error, "ThreadHelper");
            }

            // Prevents "Normal" processes from interrupting Threads
            var process = Process.GetCurrentProcess();
            process.PriorityClass = ProcessPriorityClass.High;
        }

        private static void EndThreadAffinity()
        {
            if (IsRunningOnMono)
            {
                return;
            }

            Thread.EndThreadAffinity();
        }

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        public static ProcessThread GetCurrentThread()
        {
            var id = GetCurrentThreadId();
            var thread = (from ProcessThread th in Process.GetCurrentProcess().Threads
                           where th.Id == id
                           select th).Single();

            return thread;
        }

        private static int GetAffinity(int index, int cores)
        {
            var affinity = index * 2 % cores;

            if (index % cores >= cores / 2)
            {
                affinity++;
            }

            return affinity;
        }

        /// <summary>
        /// Sets the process to run on second core with high priority
        /// </summary>
        public static void SetProcessor()
        {
            if (!IsRunningOnMono)
            {
                var process = Process.GetCurrentProcess();

                try
                {
                    // Uses the second Core or Processor for the Test
                    process.ProcessorAffinity = new IntPtr(2);
                }
                catch (Exception)
                {
                    var logger = Logger.Setup();
                    logger.Write($"Could not set Task to run on second Core or Processor", LogLevel.Error, "ThreadHelper");
                }

                // Prevents "Normal" processes from interrupting Threads
                process.PriorityClass = ProcessPriorityClass.High;
            }
        }

        /// <summary>
        /// Sets the thread priority to highest
        /// </summary>
        public static void SetThreadPriority()
        {
            // Prevents "Normal" Threads from interrupting this thread
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
        }
    }
}
