namespace MeasureMap
{
    /// <summary>
    /// Chain of responnsibility Manager for executing tasks
    /// </summary>
    public class TaskExecutionChain : ISessionHandler
    {
        private ISessionHandler _root;
        private ISessionHandler _last;
        
        /// <summary>
        /// Set the next execution item
        /// </summary>
        /// <param name="next">The next executor</param>
        public void SetNext(ISessionHandler next)
        {
            if(_root == null)
            {
                _root = next;
                _last = next;
            }
            else
            {
                _last.SetNext(next);
            }

            _last = next;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="task">The task to run</param>
        /// <param name="settings">The settings for the profiler</param>
        /// <returns>The resulting collection of the executions</returns>
        public IProfilerResult Execute(ITask task, ProfilerSettings settings)
        {
            return _root.Execute(task, settings);
        }
    }
}
