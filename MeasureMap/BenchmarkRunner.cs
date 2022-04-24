using System;
using System.Collections.Generic;

namespace MeasureMap
{
    /// <summary>
    /// Runs multiple sessions to create benchmarks
    /// </summary>
    public class BenchmarkRunner
    {
        private readonly ProfilerSettings _settings;
        private readonly Dictionary<string, ProfilerSession> _sessions;

        /// <summary>
        /// Creates an instance of the BenchmaekRunner
        /// </summary>
        public BenchmarkRunner()
        {
            _settings = new ProfilerSettings();
            _sessions = new Dictionary<string, ProfilerSession>();
        }

        /// <summary>
        /// Gets the settings for the benchmarks
        /// </summary>
        public ProfilerSettings Settings => _settings;

        /// <summary>
        /// Add a new session to run benchmarks against
        /// </summary>
        /// <param name="name"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public BenchmarkRunner AddSession(string name, ProfilerSession session)
        {
            _sessions.Add(name, session);

            return this;
        }

        /// <summary>
        /// Run all sessions and benchmarks
        /// </summary>
        /// <returns></returns>
        public IProfilerResultCollection RunSessions()
        {
            var results = new ProfilerResultCollection(_settings.Iterations);

            var s = new Spectre.Console.AnsiConsoleSettings();
            var console = Spectre.Console.AnsiConsole.Create(s);
            var p = new Spectre.Console.Progress(console);
            p.RefreshRate = TimeSpan.FromTicks(0);
            p.HideCompleted = true;
            p.AutoClear = true;
            p.Start(ctx =>
            {
                Dictionary<string, List<Spectre.Console.ProgressTask>> tasks = new();
                // collect tasks
                foreach (var key in _sessions.Keys)
                {
                    List<Spectre.Console.ProgressTask> progressTasks = new();
                    progressTasks.Add(ctx.AddTask(Spectre.Console.Markup.Escape(key + " [THREAD " + 1 + "]"), true, 1));
                    int t = _sessions[key].thread_count;
                    for (int i = 2; i <= t; i++)
                    {
                        progressTasks.Add(ctx.AddTask(Spectre.Console.Markup.Escape(key + " [THREAD " + i + "]"), true, 1));
                    }
                    tasks[key] = progressTasks;
                }
                foreach (var key in _sessions.Keys)
                {
                    var session = _sessions[key];
                    session.SetSettings(_settings);
                    var result = session.RunSession(key, tasks);
                    results.Add(key, result);
                }
            });
            return results;
        }
    }
}
