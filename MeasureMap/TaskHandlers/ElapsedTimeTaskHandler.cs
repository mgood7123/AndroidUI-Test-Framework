using System.Diagnostics;

namespace MeasureMap
{
    /// <summary>
    /// Taskhandler that measures the elapsed time for each profiling task execution
    /// </summary>
    public class ElapsedTimeTaskHandler : TaskHandler
    {
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IIterationResult Run(IExecutionContext context)
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = base.Run(context);

            sw.Stop();

            result.Ticks = sw.ElapsedTicks;
            result.Duration = sw.Elapsed;

            return result;
        }
    }
}
