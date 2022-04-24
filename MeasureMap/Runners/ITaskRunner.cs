using System;

namespace MeasureMap.Runners
{
    /// <summary>
    /// Runner for the Tasks. The implementation defines the way a task is run
    /// </summary>
    public interface ITaskRunner
    {
        /// <summary>
        /// Runs the task
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="context"></param>
        /// <param name="action"></param>
        void Run(ProfilerSettings settings, ExecutionContext context, Spectre.Console.ProgressTask progressTask, Action action);
    }
}
