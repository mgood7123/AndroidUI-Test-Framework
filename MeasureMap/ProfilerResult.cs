using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MeasureMap
{
    /// <summary>
    /// The result
    /// </summary>
    public class ProfilerResult : IEnumerable<IResult>, IProfilerResult
    {
        private readonly List<IResult> _results = new List<IResult>();

        /// <summary>
        /// Creates a profiler result
        /// </summary>
        public ProfilerResult()
        {
            ResultValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// Collection of all retun values
        /// </summary>
        public IDictionary<string, object> ResultValues { get; }

        /// <summary>
        /// Gets the total timespan
        /// </summary>
        public TimeSpan Elapsed => ResultValues.ContainsKey(ResultValueType.Elapsed) ? (TimeSpan)ResultValues[ResultValueType.Elapsed] : TimeSpan.Zero;

        /// <summary>
        /// The iterations that were run
        /// </summary>
        public IEnumerable<IIterationResult> Iterations => _results.SelectMany(r => r.Iterations);

        /// <summary>
        /// Gets the fastest iterations
        /// </summary>
        public IIterationResult Fastest
        {
            get
            {
                return Iterations.OrderBy(i => i.Ticks).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the slowest iterations
        /// </summary>
        public IIterationResult Slowest
        {
            get
            {
                return Iterations.OrderByDescending(i => i.Ticks).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the average Milliseconds that all iterations took to run the task
        /// </summary>
        public long AverageMilliseconds
        {
            get
            {
                return Iterations.Select(i => (int)i.Duration.TotalMilliseconds).Sum() / Iterations.Count();
            }
        }

        /// <summary>
        /// Gets the average Ticks that all iterations took to run the task
        /// </summary>
        public long AverageTicks
        {
            get
            {
                return Iterations.Select(i => i.Ticks).Sum() / Iterations.Count();
            }
        }

        /// <summary>
        /// Gets the average time each iteration took
        /// </summary>
        public TimeSpan AverageTime => TimeSpan.FromTicks(AverageTicks);

        /// <summary>
        /// Gets the total time for all iterations
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return TimeSpan.FromTicks(Iterations.Select(i => i.Ticks).Sum());
            }
        }
        
        /// <summary>
        /// The initial memory size
        /// </summary>
        public long InitialSize => _results.Select(r => r.InitialSize).Min();

        /// <summary>
        /// The memory size after measure
        /// </summary>
        public long EndSize => _results.Select(r => r.EndSize).Max();

        /// <summary>
        /// The increase in memory size
        /// </summary>
        public long Increase => EndSize - InitialSize;

        /// <summary>
        /// Duration of the warmup
        /// </summary>
        public TimeSpan Warmup => ResultValues.ContainsKey(ResultValueType.Warmup) ? (TimeSpan)ResultValues[ResultValueType.Warmup] : TimeSpan.Zero;

        /// <summary>
        /// Gets the last <see cref="Result"/> for fast access that is needed during executions
        /// </summary>
        public Result Last { get; private set; }

        /// <summary>
        /// Gets the number of Threads that where used to run the task
        /// </summary>
        public int ThreadCount => _results.Select(r => r.ThreadCount).First();

        /// <summary>
        /// Gets the total number of iterations
        /// </summary>
        public long TotalIterations
        {
            get
            {
                return _results.Select(r => r.Iterations.Count()).Sum();
            }
        }

        /// <summary>
        /// Add a new result
        /// </summary>
        /// <param name="result"></param>
        internal void Add(Result result)
        {
            _results.Add(result);
            Last = result;
        }

        /// <summary>
        /// The enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IResult> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }
    }
}
