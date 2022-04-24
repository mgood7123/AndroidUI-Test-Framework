using System;
using System.Diagnostics;

namespace MeasureMap
{
    /// <summary>
    /// Defines a task that will be run
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        IIterationResult Run(IExecutionContext context);
    }

    /// <summary>
    /// Defines a task that will be run
    /// </summary>
    public class Task : ITask
    {
        private readonly Action _task;

        /// <summary>
        /// Defines a task that will be run
        /// </summary>
        public Task(Action task)
        {
            _task = task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public IIterationResult Run(IExecutionContext context)
        {
            _task();

            var result = new IterationResult()
            {
                //Data = iteration
            };

            return result;
        }
    }
    /// <summary>
    /// Defines a task receiving a ExecutionContext that will be run
    /// </summary>
    public class ContextTask : ITask
    {
        private readonly Action<IExecutionContext> _task;

        /// <summary>
        /// Defines a task that will be run
        /// </summary>
        public ContextTask(Action<IExecutionContext> task)
        {
            _task = task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public IIterationResult Run(IExecutionContext context)
        {
            _task(context);

            return new IterationResult();
        }
    }

    /// <summary>
    /// Defines a task that will be run
    /// </summary>
    public class OutputTask<T> : ITask
    {
        private readonly Func<IExecutionContext, T> _task;

        /// <summary>
        /// Defines a task that will be run
        /// </summary>
        public OutputTask(Func<IExecutionContext, T> task)
        {
            _task = task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public IIterationResult Run(IExecutionContext context)
        {
            var output = _task(context);

            var result = new IterationResult()
            {
                Data = output
            };

            return result;
        }
    }

    /// <summary>
    /// Defines a task that will be run
    /// </summary>
    public class Task<T> : ITask
    {
        private readonly Func<T, T> _task;
        private T _parameter;

        /// <summary>
        /// Defines a task that will be run
        /// </summary>
        public Task(Func<T, T> task)
        {
            _task = task;
            _parameter = GetObject();
        }

        /// <summary>
        /// Defines a task that will be run
        /// </summary>
        public Task(Func<T, T> task, T parameter)
        {
            _task = task;
            _parameter = parameter;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="context">The current execution context</param>
        /// <returns>The resulting collection of the executions</returns>
        public IIterationResult Run(IExecutionContext context)
        {
            // ATENTION
            // TODO: Check if using _parameter is threadsafe

            _parameter = _task(_parameter);

            var result = new IterationResult()
            {
                Data = _parameter
            };

            return result;
        }

        private T GetObject()
        {
            try
            {
                if (typeof(T).IsValueType || typeof(T) == typeof(string))
                {
                    return default(T);
                }

                return (T) Activator.CreateInstance(typeof(T));
            }
            catch (MissingMethodException e)
            {
                throw new InvalidOperationException($"The object of Type {typeof(T).Name} does not contain a parameterless constructor", e);
            }
        }
    }
}
