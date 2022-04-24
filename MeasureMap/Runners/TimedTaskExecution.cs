using System;

namespace MeasureMap.Runners
{
    /// <summary>
    /// Executes the task at a given interval. To ensure the interval can be met the Task is executed in a own thread.
    /// </summary>
    public class TimedTaskExecution : ITaskExecution
    {
        private readonly TimeSpan _interval;
        private DateTime _nextExecute = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public TimedTaskExecution(TimeSpan interval)
        {
            _interval = interval;
        }

        /// <summary>
        /// Execute the task in a interval
        /// </summary>
        /// <param name="execution"></param>
        public void Execute(Action execution)
        {
            // calculate the wait depending on the current time and the next execution
            var wait = _nextExecute.Subtract(DateTime.Now);
            if(wait > TimeSpan.Zero)
            {
                System.Threading.Tasks.Task.Delay(wait).Wait();
            }

            System.Threading.Tasks.Task.Factory.StartNew(execution);

            // set the time for the next execution
            _nextExecute = DateTime.Now.Add(_interval);
        }
    }
}
