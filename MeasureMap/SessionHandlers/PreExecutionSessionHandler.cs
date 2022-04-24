using System;

namespace MeasureMap
{
    /// <summary>
    /// SessionHandler that executes a task before the TaskExecution
    /// </summary>
    public class PreExecutionSessionHandler : SessionHandler
    {
        private readonly Action _action;

        /// <summary>
        /// Creates a SessionHandler that executes a task before the TaskExecution
        /// </summary>
        /// <param name="action"></param>
        public PreExecutionSessionHandler(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="task">The task to run</param>
        /// <param name="settings">The settings for the profiler</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IProfilerResult Execute(ITask task, ProfilerSettings settings)
        {
            _action.Invoke();

            return base.Execute(task, settings);
        }
    }
}
