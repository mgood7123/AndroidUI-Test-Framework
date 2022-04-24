using System;

namespace MeasureMap
{
    /// <summary>
    /// Taskhandler that measures the memory before and after each profiling task execution
    /// </summary>
    public class MemoryCollectionTaskHandler : TaskHandler
    {
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IIterationResult Run(IExecutionContext context)
        {
            var initial = GC.GetTotalMemory(true);

            var result = base.Run(context);

            result.InitialSize = initial;
            result.AfterExecution = GC.GetTotalMemory(false);
            ForceGarbageCollector();
            result.AfterGarbageCollection = GC.GetTotalMemory(true);

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
