
namespace MeasureMap
{
    /// <summary>
    /// Chain of responnsibility Manager for running tasks
    /// </summary>
    public class ProcessingPipeline : ITaskMiddleware
    {
        private ITask _root;
        private ITask _last;

        /// <summary>
        /// Set the next execution item
        /// </summary>
        /// <param name="next">The next executor</param>
        public void SetNext(ITask next)
        {
            if (_root == null)
            {
                _root = next;
                _last = next;
            }
            else
            {
                if(_last is ITaskMiddleware last)
                {
                    last.SetNext(next);
                    _last = next;
                }
            }
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public IIterationResult Run(IExecutionContext context)
        {
            return _root.Run(context);
        }
    }
}
