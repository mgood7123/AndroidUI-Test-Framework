using System;
using MeasureMap.Diagnostics;

namespace MeasureMap
{
    /// <summary>
    /// A worker that runs the provided tasks
    /// </summary>
    public class Worker
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the worker
        /// </summary>
        public Worker()
        {
            _logger = Logger.Setup();
        }

        /// <summary>
        /// Runs the provided task for the iteration count
        /// </summary>
        /// <param name="task">The task that has to be run</param>
        /// <param name="settings">The settings for the profiler</param>
        /// <returns></returns>
        public Result Run(ITask task, ProfilerSettings settings, Spectre.Console.ProgressTask progressTask)
        {
            var result = new Result();

            ForceGarbageCollector();
            
            result.InitialSize = GC.GetTotalMemory(true);
            var context = new ExecutionContext();

            var runner = settings.Runner;
            runner.Run(settings, context, progressTask, () =>
            {
                var iteration = task.Run(context);

                result.Add(iteration);
            });

            ForceGarbageCollector();
            result.EndSize = GC.GetTotalMemory(true);

            return result;
        }
        
        /// <summary>
        /// Forces the GC to run
        /// </summary>
        protected void ForceGarbageCollector()
        {
            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
