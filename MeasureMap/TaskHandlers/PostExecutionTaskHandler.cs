using System;

namespace MeasureMap
{
    /// <summary>
    /// Taskhandler that executes a task after each profiling task execution
    /// </summary>
    public class PostExecutionTaskHandler : TaskHandler
    {
        private readonly ITask _task;

        /// <summary>
        /// Creates a new taskhandler
        /// </summary>
        /// <param name="task">Task to execute after each profiling task execution</param>
        public PostExecutionTaskHandler(Action task)
        {
            _task = new Task(task);
        }

        /// <summary>
        /// Creates a new taskhandler
        /// </summary>
        /// <param name="task">Task to execute after each profiling task execution</param>
        public PostExecutionTaskHandler(Action<IExecutionContext> task)
        {
            _task = new ContextTask(task);
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public override IIterationResult Run(IExecutionContext context)
        {
            var result = base.Run(context);

            _task.Run(context);

            return result;
        }
    }
}
