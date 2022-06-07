using System;

namespace MeasureMap
{
    /// <summary>
    /// Marks a iteration of a run Task
    /// </summary>
    public interface IIterationResult
    {
        /// <summary>
        /// The memory size after execution
        /// </summary>
        long AfterExecution { get; set; }

        /// <summary>
        /// The memory size after GC
        /// </summary>
        long AfterGarbageCollection { get; set; }

        /// <summary>
        /// Gets or sets the data that is returned by the Task. If no data is returned, the iteration is contained
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Gets the Milliseconds that the iteration took to run the Task
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// The initial memory size
        /// </summary>
        long InitialSize { get; set; }

        /// <summary>
        /// Gets the Ticks that the iteration took to run the Task
        /// </summary>
        long Ticks { get; set; }

        /// <summary>
        /// The timestamp of when the iteration was run
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets the current iteration
        /// </summary>
        int Iteration { get; set; }

        /// <summary>
        /// Gets the current thread
        /// </summary>
        int ThreadId { get; set; }

        /// <summary>
        /// Gets the current process
        /// </summary>
        int ProcessId { get; set; }
    }

    /// <summary>
    /// Marks a iteration of a run Task
    /// </summary>
    public class IterationResult : IIterationResult
    {
        /// <summary>
        /// Creates a object containing information on the iteration
        /// </summary>
        public IterationResult()
        {
            TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Gets the Ticks that the iteration took to run the Task
        /// </summary>
        public long Ticks { get; set; }

        /// <summary>
        /// Gets the Milliseconds that the iteration took to run the Task
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The timestamp of when the iteration was run
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The memory size after execution
        /// </summary>
        public long AfterExecution { get; set; }

        /// <summary>
        /// The memory size after GC
        /// </summary>
        public long AfterGarbageCollection { get; set; }

        /// <summary>
        /// The initial memory size
        /// </summary>
        public long InitialSize { get; set; }

        /// <summary>
        /// Gets or sets the data that is returned by the Task. If no data is returned, the iteration is contained
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets the current iteration
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Gets the current thread
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets the current process
        /// </summary>
        public int ProcessId { get; set; }

        public override string ToString()
        {
            return "Ticks: " + Ticks + " ms: " + Duration;
        }
    }
}
