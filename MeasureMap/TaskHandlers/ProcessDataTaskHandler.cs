using System.Diagnostics;

namespace MeasureMap
{
    /// <summary>
    /// Taskhandler that reads the current process and thread for each profiling task execution
    /// </summary>
    public class ProcessDataTaskHandler : TaskHandler
    {
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IIterationResult Run(IExecutionContext context)
        {
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var processId = Process.GetCurrentProcess().Id;

            context.Set(ContextKeys.ThreadId, threadId);
            context.Set(ContextKeys.ProcessId, processId);

            var result = base.Run(context);

            result.ThreadId = threadId;
            result.ProcessId = processId;
            result.Iteration = context.Get<int>(ContextKeys.Iteration);

            return result;
        }
    }
}
