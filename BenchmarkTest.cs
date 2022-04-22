using MeasureMap;
using Spectre.Console;
using System;

namespace AndroidUITestFramework
{
    //
    // Summary:
    //     Extension class for ProfilerSession
    public static class ProfilerSessionExtensions
    {
        //
        // Summary:
        //     Sets the Task that will be profiled
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The Task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession Task(this BenchmarkTest.ProfilerSession session, Action task)
        {
            session.Task(new Task(task));
            return session;
        }

        //
        // Summary:
        //     Sets the Task that will be profiled
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute
        //
        // Type parameters:
        //   T:
        //     The return and parameter value
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession Task<T>(this BenchmarkTest.ProfilerSession session, Func<T, T> task)
        {
            session.Task(new Task<T>(task));
            return session;
        }

        //
        // Summary:
        //     Sets the Task that will be profiled
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute
        //
        //   parameter:
        //     The parameter that is passed to the task
        //
        // Type parameters:
        //   T:
        //     The return and parameter value
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession Task<T>(this BenchmarkTest.ProfilerSession session, Func<T, T> task, T parameter)
        {
            session.Task(new Task<T>(task, parameter));
            return session;
        }

        //
        // Summary:
        //     Sets the Task that will be profiled passing the current ExecutionContext as parameter
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession Task(this BenchmarkTest.ProfilerSession session, Action<IExecutionContext> task)
        {
            session.Task(new ContextTask(task));
            return session;
        }

        //
        // Summary:
        //     Sets the Task that will be profiled passing the current ExecutionContext as parameter
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute
        //
        // Type parameters:
        //   T:
        //     The expected task output
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession Task<T>(this BenchmarkTest.ProfilerSession session, Func<IExecutionContext, T> task)
        {
            session.Task(new OutputTask<T>(task));
            return session;
        }

        //
        // Summary:
        //     Sets the amount of iterations that the profileing session should run the task
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   iterations:
        //     The iterations to run the task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession SetIterations(this BenchmarkTest.ProfilerSession session, int iterations)
        {
            session.Settings.Iterations = iterations;
            return session;
        }

        //
        // Summary:
        //     Sets the duration that the profileing session should run the task for
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   duration:
        //     The iterations to run the task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession SetDuration(this BenchmarkTest.ProfilerSession session, TimeSpan duration)
        {
            session.Settings.Duration = duration;
            return session;
        }

        //
        // Summary:
        //     The task will be executed at the given interval. To ensure the execution interval,
        //     the task is executed in a new thread
        //
        // Parameters:
        //   session:
        //
        //   interval:
        public static BenchmarkTest.ProfilerSession SetInterval(this BenchmarkTest.ProfilerSession session, TimeSpan interval)
        {
            session.s.SetInterval(interval);
            return session;
        }

        public static BenchmarkTest.ProfilerSession RunWarmup(this BenchmarkTest.ProfilerSession session, bool run)
        {
            session.Settings.RunWarmup = run;
            return session;
        }

        //
        // Summary:
        //     Sets the settings that the profiler should use
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   settings:
        //     The settings for thr profiler
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession SetSettings(this BenchmarkTest.ProfilerSession session, ProfilerSettings settings)
        {
            session.SetSettings(settings);
            return session;
        }

        //
        // Summary:
        //     Set values in the settings
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   settings:
        //     The settings for thr profiler
        public static BenchmarkTest.ProfilerSession Settings(this BenchmarkTest.ProfilerSession session, Action<ProfilerSettings> settings)
        {
            settings(session.Settings);
            return session;
        }

        //
        // Summary:
        //     Add the middleware to the processing pipeline
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   middleware:
        //     The middleware to add
        public static BenchmarkTest.ProfilerSession AddMiddleware(this BenchmarkTest.ProfilerSession session, ITaskMiddleware middleware)
        {
            session.s.AddMiddleware(middleware);
            return session;
        }

        //
        // Summary:
        //     Add the middleware to the session pipeline
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   middleware:
        //     The middleware to add
        public static BenchmarkTest.ProfilerSession AddMiddleware(this BenchmarkTest.ProfilerSession session, ISessionHandler middleware)
        {
            session.s.AddMiddleware(middleware);
            return session;
        }

        //
        // Summary:
        //     Sets a Task that will be executed before each profiling task execution
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute before each profiling task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession PreExecute(this BenchmarkTest.ProfilerSession session, Action task)
        {
            session.s.PreExecute(task);
            return session;
        }

        //
        // Summary:
        //     Sets a Task that will be executed before each profiling task execution
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute before each profiling task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession PreExecute(this BenchmarkTest.ProfilerSession session, Action<IExecutionContext> task)
        {
            session.s.PreExecute(task);
            return session;
        }

        //
        // Summary:
        //     Sets a Task that will be executed after each profiling task execution
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute after each profiling task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession PostExecute(this BenchmarkTest.ProfilerSession session, Action task)
        {
            session.s.PostExecute(task);
            return session;
        }

        //
        // Summary:
        //     Sets a Task that will be executed after each profiling task execution
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   task:
        //     The task to execute after each profiling task
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession PostExecute(this BenchmarkTest.ProfilerSession session, Action<IExecutionContext> task)
        {
            session.s.PostExecute(task);
            return session;
        }

        //
        // Summary:
        //     Add a delay before each task gets executed. The delay is not countet to the execution
        //     time of the task
        //
        // Parameters:
        //   session:
        //
        //   duration:
        public static BenchmarkTest.ProfilerSession AddDelay(this BenchmarkTest.ProfilerSession session, TimeSpan duration)
        {
            session.s.AddDelay(duration);
            return session;
        }

        //
        // Summary:
        //     Adds a setup task to the sessionpipeline
        //
        // Parameters:
        //   session:
        //     The current session
        //
        //   setup:
        //     The setuptask
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.ProfilerSession Setup(this BenchmarkTest.ProfilerSession session, Action setup)
        {
            session.s.Setup(setup);
            return session;
        }
    }

    public static class BenchmarkRunnerExtensions
    {
        //
        // Summary:
        //     Set the amount of iterations that the benchmarks run
        //
        // Parameters:
        //   runner:
        //     The benchmark runner
        //
        //   iterations:
        //     the amount of itterations
        public static BenchmarkTest.BenchmarkRunner SetIterations(this BenchmarkTest.BenchmarkRunner runner, int iterations)
        {
            runner.b.SetIterations(iterations);
            return runner;
        }

        //
        // Summary:
        //     Sets the amount of iterations that the profileing session should run the task
        //
        // Parameters:
        //   runner:
        //     The benchmark runner
        //
        //   settings:
        //     The settings for thr profiler
        //
        // Returns:
        //     The current profiling session
        public static BenchmarkTest.BenchmarkRunner SetSettings(this BenchmarkTest.BenchmarkRunner runner, ProfilerSettings settings)
        {
            runner.b.SetSettings(settings);
            return runner;
        }

        //
        // Summary:
        //     Set values in the settings
        //
        // Parameters:
        //   runner:
        //     The benchmark runner
        //
        //   settings:
        //     The settings for thr profiler
        public static BenchmarkTest.BenchmarkRunner Settings(this BenchmarkTest.BenchmarkRunner runner, Action<ProfilerSettings> settings)
        {
            runner.b.Settings(settings);
            return runner;
        }
    }

    public abstract class BenchmarkTest : Test
    {
        private class CustomMiddleware : ITaskMiddleware
        {
            private ITask _next;
            internal System.Threading.ThreadLocal<int> current_iteration;
            internal int max_iterations;
            internal int current_global_iteration;
            internal int max_global_iterations;
            string name;
            static object LOCK = new object();
            BenchmarkRunner benchmarkRunner;

            public CustomMiddleware(BenchmarkRunner benchmarkRunner, string name, int max_iterations, int threadCount, bool has_warmup)
            {
                this.benchmarkRunner = benchmarkRunner;
                int tmp = has_warmup ? -1 : 0;
                current_global_iteration = tmp;
                current_iteration = new System.Threading.ThreadLocal<int>();
                current_iteration.Value = tmp;
                this.max_iterations = max_iterations;
                max_global_iterations = max_iterations * threadCount;
                this.name = name;
            }
            public IIterationResult Run(IExecutionContext context) {
                if (benchmarkRunner.Logging)
                {
                    lock (LOCK)
                    {
                        if (current_iteration.Value == max_iterations)
                        {
                            current_iteration.Value = 1;
                        }
                        else
                        {
                            current_iteration.Value++;
                        }
                        current_global_iteration++;
                        if (current_iteration.Value != 0)
                        {
                            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
                            Console.WriteLine("[ Thread id: " + id + " ] : " + name + ": iteration " + current_iteration.Value + " of " + max_iterations + ", global iteration " + current_global_iteration + " of " + max_global_iterations);
                        }
                    }
                }
                return _next.Run(context);
            }

            public void SetNext(ITask next)
            {
                _next = next;
            }
        }

        //
        // Summary:
        //     The main entry for a profiling session
        public class ProfilerSession : IDisposable
        {
            internal MeasureMap.ProfilerSession s;
            public ProfilerSettings Settings => s.Settings;

            [Obsolete("Use SessionPipeline")]
            public ISessionHandler SessionHandler => s.SessionHandler;

            public ISessionHandler SessionPipeline => s.SessionPipeline;

            [Obsolete("Use ProcessingPipeline")]
            public ITaskMiddleware TaskHandler => s.TaskHandler;

            //
            // Summary:
            //     Gets the processing pipeline containing the middleware that get executed for
            //     every itteration. The task is executed at the top of the executionchain.
            public ITaskMiddleware ProcessingPipeline => s.ProcessingPipeline;

            private ProfilerSession()
            {
                s = MeasureMap.ProfilerSession.StartSession();
            }

            //
            // Summary:
            //     Creates a new Session for profiling performance
            //
            // Returns:
            //     A profiler session
            public static ProfilerSession StartSession()
            {
                return new ProfilerSession();
            }

            internal int threadCount = 1;

            //
            // Summary:
            //     Sets the amount of threads that the profiling sessions should run in. All iterations
            //     are run on every thread.
            //
            // Parameters:
            //   thredCount:
            //     The amount of threads that the task is run on
            //
            // Returns:
            //     The current profiling session
            public ProfilerSession SetThreads(int thredCount)
            {
                threadCount = thredCount;
                s.SetThreads(thredCount);
                return this;
            }

            //
            // Summary:
            //     Sets the Taskrunner that will be profiled
            //
            // Parameters:
            //   runner:
            //     The runner containig the task
            //
            // Returns:
            //     The current profiling session
            public ProfilerSession Task(ITask runner)
            {
                s.Task(runner);
                return this;
            }

            //
            // Summary:
            //     Adds a condition to the profiling session
            //
            // Parameters:
            //   condition:
            //     The condition that will be checked
            //
            // Returns:
            //     The current profiling session
            [Obsolete("Use Assert", false)]
            public ProfilerSession AddCondition(Func<IResult, bool> condition)
            {
                return Assert(condition);
            }

            //
            // Summary:
            //     Adds a condition to the profiling session
            //
            // Parameters:
            //   assertion:
            //     The condition that will be checked
            //
            // Returns:
            //     The current profiling session
            public ProfilerSession Assert(Func<IResult, bool> assertion)
            {
                s.Assert(assertion);
                return this;
            }

            //
            // Summary:
            //     Starts the profiling session
            //
            // Returns:
            //     The resulting profile
            public IProfilerResult RunSession()
            {
                return s.RunSession();
            }

            public void Dispose()
            {
                s.Dispose();
            }
        }


        public class BenchmarkRunner
        {
            internal MeasureMap.BenchmarkRunner b;
            internal readonly System.Collections.Generic.Dictionary<string, ProfilerSession> _sessions;
            public ProfilerSettings Settings => b.Settings;

            public bool Logging = false;

            internal BenchmarkRunner()
            {
                b = new MeasureMap.BenchmarkRunner();
                _sessions = new System.Collections.Generic.Dictionary<string, ProfilerSession>();
            }

            public BenchmarkRunner AddSession(string name, ProfilerSession session)
            {
                session.AddMiddleware(new CustomMiddleware(this, name, Settings.Iterations, session.threadCount, session.Settings.RunWarmup));
                b.AddSession(name, session.s);
                _sessions.Add(name, session);
                return this;
            }

            public IProfilerResultCollection RunSessions()
            {
                return b.RunSessions();
            }
        }

        public abstract void prepareBenchmark(BenchmarkRunner runner);
        public sealed override void Run(TestGroup nullableInstance)
        {
            BenchmarkRunner benchmarkRunner = new();
            prepareBenchmark(benchmarkRunner);
            WriteToConsole(benchmarkRunner.RunSessions(), benchmarkRunner._sessions);
        }

        private static void WriteToConsole(IProfilerResultCollection results, System.Collections.Generic.Dictionary<string, ProfilerSession> _sessions)
        {
            Console.WriteLine("Iterations: " + results.Iterations);

            var table = new Table();

            table.AddColumns(
                new TableColumn("Name").LeftAligned().NoWrap(),
                new TableColumn("Threads").RightAligned(),
                new TableColumn("Avg\nTime").RightAligned().NoWrap(),
                new TableColumn("Avg\nTicks").RightAligned().NoWrap(),
                new TableColumn("Total").RightAligned().NoWrap(),
                new TableColumn("Fastest").RightAligned().NoWrap(),
                new TableColumn("Slowest").RightAligned().NoWrap(),
                new TableColumn("Total\nUsed Memory").RightAligned()
            );

            foreach (string key in results.Keys)
            {
                IProfilerResult result = results[key];
                table.AddRow(
                    key,
                    _sessions[key].threadCount.ToString(),
                    result.AverageTime.ToString(),
                    result.AverageTicks.ToString(),
                    result.TotalTime.ToString(),
                    result.Fastest.Ticks.ToString(),
                    result.Slowest.Ticks.ToString(),
                    result.Increase.ToString()
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
