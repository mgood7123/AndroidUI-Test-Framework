using System;
using MeasureMap.Diagnostics;

namespace MeasureMap.Runners
{
    /// <summary>
    /// Runner that runs the task for a given amount of iterations
    /// </summary>
    public class IterationRunner : ITaskRunner
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the worker
        /// </summary>
        public IterationRunner()
        {
            _logger = Logger.Setup();
        }

        /// <summary>
        /// Runs the task for the given amount of iterations that are defined in the <see cref="ProfilerSettings"/>
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public void Run(ProfilerSettings settings, ExecutionContext context, Spectre.Console.ProgressTask progressTask, Action action)
        {
            int iterations = settings.Iterations;
            progressTask.MaxValue = iterations;

            var execution = settings.Execution;
            int lastPercentage = 0;

            for (var i = 0; i < iterations; i++)
            {
                context.Set(ContextKeys.Iteration, i + 1);
                execution.Execute(action);
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
