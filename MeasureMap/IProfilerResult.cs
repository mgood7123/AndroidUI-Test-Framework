using System;
using System.Collections.Generic;

namespace MeasureMap
{
    /// <summary>
    /// The result
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Collection of all return values
        /// </summary>
        IDictionary<string, object> ResultValues { get; }

        /// <summary>
        /// Gets the number of Threads that where used to run the task
        /// </summary>
        int ThreadCount { get; }

        /// <summary>
        /// Gets the average Milliseconds that all iterations took to run the task
        /// </summary>
        long AverageMilliseconds { get; }

        /// <summary>
        /// Gets the average Ticks that all iterations took to run the task
        /// </summary>
        long AverageTicks { get; }

        /// <summary>
        /// Gets the average time each iteration took
        /// </summary>
        TimeSpan AverageTime { get; }

        /// <summary>
        /// Gets the fastest iterations
        /// </summary>
        IIterationResult Fastest { get; }

        /// <summary>
        /// The increase in memory size
        /// </summary>
        long Increase { get; }

        /// <summary>
        /// The initial memory size
        /// </summary>
        long InitialSize { get; }

        /// <summary>
        /// The memory size after measure
        /// </summary>
        long EndSize { get; }

        /// <summary>
        /// The iterations that were run
        /// </summary>
        IEnumerable<IIterationResult> Iterations { get; }

        /// <summary>
        /// Gets the slowest iterations
        /// </summary>
        IIterationResult Slowest { get; }

        /// <summary>
        /// Gets the total number of iterations
        /// </summary>
        public long TotalIterations { get; }

        /// <summary>
        /// Gets the total time for all iterations
        /// </summary>
        TimeSpan TotalTime { get; }
    }

    public interface IProfilerResult : IResult, IEnumerable<IResult>
    {
    }
}