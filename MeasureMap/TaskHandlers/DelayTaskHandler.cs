using System;
using System.Threading;

namespace MeasureMap
{
    /// <summary>
    /// TaskHandler that delays the execution of the task
    /// </summary>
    public class DelayTaskHandler : TaskHandler
    {
        private readonly TimeSpan _duration;
        private readonly WaitHandle _waitHandle;

        /// <summary>
        /// Creates a new taskhandler for delaying executions
        /// </summary>
        /// <param name="duration">The duration with which the task is paused by</param>
        public DelayTaskHandler(TimeSpan duration)
        {
            _duration = duration;
            _waitHandle = new ManualResetEvent(false);
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IIterationResult Run(IExecutionContext context)
        {
            _waitHandle.WaitOne(_duration, false);

            var result = base.Run(context);

            return result;
        }
    }
}
