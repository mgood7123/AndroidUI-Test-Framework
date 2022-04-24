using System;
using System.Diagnostics;

namespace MeasureMap
{
    /// <summary>
    /// TaskExecutor that measures the total elapsed time
    /// </summary>
    public class ElapsedTimeSessionHandler : SessionHandler
    {
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="task">The task to run</param>
        /// <param name="settings">The settings for the profiler</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IProfilerResult Execute(ITask task, ProfilerSettings settings)
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = base.Execute(task, settings);

            sw.Stop();
            result.ResultValues.Add("Elapsed", sw.Elapsed);

            return result;
        }
    }
}
