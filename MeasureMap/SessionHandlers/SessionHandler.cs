
namespace MeasureMap
{
    /// <summary>
    /// Baseclass for Chain of responsibility for executing tasks
    /// </summary>
    public abstract class SessionHandler : ISessionHandler
    {
        private ISessionHandler _next;

        /// <summary>
        /// Set the next execution item
        /// </summary>
        /// <param name="next">The next handler for the session</param>
        public void SetNext(ISessionHandler next)
        {
            _next = next;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="task">The task to run</param>
        /// <param name="settings">The settings for the profiler</param>
        /// <returns>The resulting collection of the executions</returns>
        public virtual IProfilerResult Execute(ITask task, ProfilerSettings settings)
        {
            if(_next == null)
            {
                return new ProfilerResult();
            }

            return _next.Execute(task, settings);
        }
    }
}
