using System;

namespace MeasureMap
{
    /// <summary>
    /// Taskhandler that executes a task before each profiling task execution
    /// </summary>
    public class PreExecutionTaskHandler : TaskHandler
    {
        private readonly ITask _task;

        /// <summary>
        /// Creates a new taskhandler
        /// </summary>
        /// <param name="task">Task to execute before each profiling task execution</param>
        public PreExecutionTaskHandler(Action task)
        {
            _task = new Task(task);
        }

        /// <summary>
        /// Creates a new taskhandler
        /// </summary>
        /// <param name="task">Task to execute before each profiling task execution</param>
        public PreExecutionTaskHandler(Action<IExecutionContext> task)
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
            _task.Run(context);

            var result = base.Run(context);

            return result;
        }
    }
}
