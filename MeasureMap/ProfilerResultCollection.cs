using System.Collections;
using System.Collections.Generic;

namespace MeasureMap
{
    /// <summary>
    /// Defines the redults of benchmarktests
    /// </summary>
    public interface IProfilerResultCollection : IEnumerable<IProfilerResult>
    {
        /// <summary>
        /// Add a result of benchmarktest
        /// </summary>
        /// <param name="name">The name of the result</param>
        /// <param name="result">The result</param>
        void Add(string name, IProfilerResult result);

        /// <summary>
        /// Gets the amount of iterations that the benchmarktests were run
        /// </summary>
        int Iterations { get; }
        
        /// <summary>
        /// Gets the keys collection
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Indexer for benchmarkresults 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IProfilerResult this[string key] { get; }
    }

    /// <summary>
    /// Defines the redults of benchmarktests
    /// </summary>
    public class ProfilerResultCollection : IProfilerResultCollection
    {
        private readonly Dictionary<string, IProfilerResult> _results;

        /// <summary>
        /// Creates an instance of a ProfilerResultCollection
        /// </summary>
        /// <param name="iterations"></param>
        public ProfilerResultCollection(int iterations)
        {
            _results = new Dictionary<string, IProfilerResult>();
            Iterations = iterations;
        }

        /// <summary>
        /// Gets the amount of iterations that the benchmarktests were run
        /// </summary>
        public int Iterations { get; }

        /// <summary>
        /// Gets the keys collection
        /// </summary>
        public IEnumerable<string> Keys => _results.Keys;

        /// <summary>
        /// Indexer for benchmarkresults 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IProfilerResult this[string key] => _results[key];

        /// <summary>
        /// Add a result of benchmarktest
        /// </summary>
        /// <param name="name">The name of the result</param>
        /// <param name="result">The result</param>
        public void Add(string name, IProfilerResult result)
        {
            _results.Add(name, result);
        }

        /// <summary>
        /// The enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IProfilerResult> GetEnumerator()
        {
            return _results.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
