using System;

namespace MeasureMap
{
    public static class BenchmarkRunnerExtensions
    {
        /// <summary>
        /// Set the amount of iterations that the benchmarks run
        /// </summary>
        /// <param name="runner">The benchmark runner</param>
        /// <param name="iterations">the amount of itterations</param>
        /// <returns></returns>
        public static BenchmarkRunner SetIterations(this BenchmarkRunner runner, int iterations)
        {
            if (iterations < 1)
            {
                throw new InvalidOperationException("Invalid amount of iterations. There have to be at lease 1 iteration");
            }

            runner.Settings.Iterations = iterations;

            return runner;
        }

        /// <summary>
        /// Sets the amount of iterations that the profileing session should run the task
        /// </summary>
        /// <param name="runner">The benchmark runner</param>
        /// <param name="settings">The settings for thr profiler</param>
        /// <returns>The current profiling session</returns>
        public static BenchmarkRunner SetSettings(this BenchmarkRunner runner, ProfilerSettings settings)
        {
            settings.MergeChangesTo(runner.Settings);

            return runner;
        }

		/// <summary>
		/// Set values in the settings
		/// </summary>
		/// <param name="runner">The benchmark runner</param>
		/// <param name="settings">The settings for thr profiler</param>
		/// <returns></returns>
		public static BenchmarkRunner Settings(this BenchmarkRunner runner, Action<ProfilerSettings> settings)
        {
	        settings(runner.Settings);

	        return runner;
        }
	}
}
