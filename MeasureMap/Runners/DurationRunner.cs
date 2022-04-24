using System;
using MeasureMap.Diagnostics;

namespace MeasureMap.Runners
{
    /// <summary>
    /// Runner that runs the task for a given duration
    /// </summary>
    public class DurationRunner : ITaskRunner
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the worker
        /// </summary>
        public DurationRunner()
        {
            _logger = Logger.Setup();
        }

        /// <summary>
        /// Runs the task for the given duration that is defined in the <see cref="ProfilerSettings"/>
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public void Run(ProfilerSettings settings, ExecutionContext context, Spectre.Console.ProgressTask progressTask, Action action)
        {
            _logger.Write($"Running Task for {settings.Duration} for Perfomance Analysis Benchmark");
            var time = DateTime.Now + settings.Duration;
            long ticks = settings.Duration.Ticks;
            progressTask.MaxValue = ticks;

            var execution = settings.Execution;
            int lastPercentage = 0;

            var iteration = 1;
            while(true)
            {
                var current = DateTime.Now;
                if (current >= time) break;
                _logger.Write($"Running Task for iteration {iteration}");
                context.Set(ContextKeys.Iteration, iteration);

                execution.Execute(action);

                iteration++;
                long i = settings.Duration.Ticks - (time.Ticks - current.Ticks);
                int percentage = (int)(i / progressTask.MaxValue * 100.0);
                if (lastPercentage != percentage)
                {
                    lastPercentage = percentage;
                    progressTask.Value = i;
                }
            }
            progressTask.Value = progressTask.MaxValue;
        }
    }
}
