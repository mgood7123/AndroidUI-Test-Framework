using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AndroidUITestFramework
{
    public abstract class XMarkTest : Test
    {
        public class XSession
        {
            internal string name;
            internal Action task;
            internal TimeSpan duration;
            internal long iterations;
            internal ushort threads = 1;
            internal bool hasDuration;

            internal class per_thread
            {
                internal ProgressTask task;
                internal long iterations = 0;
            }

            internal List<per_thread> per_threads_stats = new();

            internal long stats_start;
            internal long stats_end;

            public XSession(string name, Action task) : this(name, task, 1, 1)
            {
            }

            public XSession(string name, Action task, long iterations) : this(name, task, iterations, 1)
            {
            }

            public XSession(string name, Action task, long iterations, ushort threads)
            {
                this.name = name ?? throw new ArgumentNullException(nameof(name));
                this.task = task ?? throw new ArgumentNullException(nameof(task));
                this.iterations = iterations;
                this.threads = threads == 0 ? (ushort)1 : threads;
                hasDuration = false;
            }

            public XSession(string name, Action task, TimeSpan duration)
            {
                this.name = name ?? throw new ArgumentNullException(nameof(name));
                this.task = task ?? throw new ArgumentNullException(nameof(task));
                this.duration = duration;
                this.threads = 1;
                hasDuration = true;
            }
        }

        public class XManager
        {
            class Pair<T1, T2>
            {
                public T1 first;
                public T2 second;

                public Pair(T1 first, T2 second)
                {
                    this.first = first;
                    this.second = second;
                }
            }
            List<XSession> sessions = new();

            public void AddSession(XSession session)
            {
                sessions.Add(session);
            }

            public void Run()
            {
                var s = new AnsiConsoleSettings();
                var console = AnsiConsole.Create(s);
                var p = new Progress(console);
                p.RefreshRate = TimeSpan.FromTicks(0);
                p.Start(benchmark);
                printResults();
            }

            private void benchmark(ProgressContext context)
            {
                CollectTasks(context);
                RunBenchMark();
            }

            private void CollectTasks(ProgressContext context)
            {
                foreach (var session in sessions)
                {
                    var x = new XSession.per_thread();
                    x.task = context.AddTask(Markup.Escape(session.name + " [THREAD " + 1 + "]"));
                    session.per_threads_stats.Add(x);
                    for (ushort i = 2; i <= session.threads; i++)
                    {
                        var x2 = new XSession.per_thread();
                        x2.task = context.AddTask(Markup.Escape(session.name + " [THREAD " + i + "]"));
                        session.per_threads_stats.Add(x2);
                    }
                }
            }

            class startend {
                internal long start;
                internal long end;
            };

            private void RunBenchMark()
            {
                foreach (var session in sessions)
                {
                    if (session.per_threads_stats.Count == 1)
                    {
                        session.per_threads_stats[0].task.Value = 0;
                        session.per_threads_stats[0].task.MaxValue = 100;

                        long start;
                        long end = 0;
                        int lastPercentage = 0;
                        if (session.hasDuration)
                        {
                            long t = session.duration.Ticks / TimeSpan.TicksPerMillisecond;
                            session.per_threads_stats[0].iterations = 0;
                            start = Environment.TickCount64;
                            var target = start + t;
                            while (true)
                            {
                                var current = Environment.TickCount64;
                                if (current >= target) break;

                                session.task.Invoke();

                                long i = t - (target - current);
                                int percentage = (int)(i / (double)t * 100.0);
                                if (lastPercentage != percentage)
                                {
                                    lastPercentage = percentage;
                                    session.per_threads_stats[0].task.Value = percentage;
                                }
                                session.per_threads_stats[0].iterations++;
                            }
                            end = Environment.TickCount64;
                        }
                        else
                        {
                            session.per_threads_stats[0].iterations = 0;
                            start = Environment.TickCount64;
                            for (long i = 0; i < session.iterations; i++)
                            {
                                session.task.Invoke();

                                int percentage = (int)(i / (double)session.iterations * 100.0);
                                if (lastPercentage != percentage)
                                {
                                    lastPercentage = percentage;
                                    session.per_threads_stats[0].task.Value = percentage;
                                }
                                session.per_threads_stats[0].iterations++;
                            }
                            end = Environment.TickCount64;
                        }
                        session.per_threads_stats[0].task.Value = 100;
                        session.stats_start = start;
                        session.stats_end = end;
                    }
                    else
                    {
                        List<System.Threading.Tasks.Task> threadList = new();
                        startend s = new();
                        s.start = 0;
                        s.end = 0;

                        for (ushort thread = 0; thread < session.threads; thread++)
                        {
                            threadList.Add(new System.Threading.Tasks.Task(ThreadLocalMethod(session, s, thread)));
                        }

                        s.start = Environment.TickCount64;

                        foreach (var task in threadList)
                        {
                            task.Start();
                        }

                        foreach (var task in threadList)
                        {
                            task.Wait();
                        }

                        s.end = Environment.TickCount64;
                        session.stats_start = s.start;
                        session.stats_end = s.end;
                    }
                }
            }

            private static Action ThreadLocalMethod(XSession session, startend s, ushort thread)
            {
                return () =>
                {
                    session.per_threads_stats[thread].task.Value = 0;
                    session.per_threads_stats[thread].task.MaxValue = 100;
                    int tid = Thread.CurrentThread.ManagedThreadId;
                    var tid_thread_stats_tid = thread;
                    int lastPercentage = 0;
                    if (session.hasDuration)
                    {
                        long t = session.duration.Ticks / TimeSpan.TicksPerMillisecond;
                        session.per_threads_stats[thread].iterations = 0;
                        var target = s.start + t;
                        while (true)
                        {
                            var current = Environment.TickCount64;
                            if (current >= target) break;

                            session.task.Invoke();

                            long i = t - (target - current);
                            int percentage = (int)(i / (double)t * 100.0);
                            if (lastPercentage != percentage)
                            {
                                lastPercentage = percentage;
                                session.per_threads_stats[thread].task.Value = percentage;
                            }
                            session.per_threads_stats[thread].iterations++;
                        }
                    }
                    else
                    {
                        session.per_threads_stats[thread].iterations = 0;
                        for (long i = 0; i < session.iterations; i++)
                        {
                            session.task.Invoke();

                            int percentage = (int)(i / (double)session.iterations * 100.0);
                            if (lastPercentage != percentage)
                            {
                                lastPercentage = percentage;
                                session.per_threads_stats[thread].task.Value = percentage;
                            }
                            session.per_threads_stats[thread].iterations++;
                        }
                    }
                    session.per_threads_stats[thread].task.Value = 100;
                };
            }

            private void printResults()
            {
                var table = new Table();

                TableExtensions.AddColumns(
                    table,
                    ColumnExtensions.NoWrap(
                        AlignableExtensions.LeftAligned(
                            new TableColumn("Name")
                        )
                    ),
                    ColumnExtensions.NoWrap(
                        AlignableExtensions.RightAligned(
                            new TableColumn("Iterations")
                        )
                    ),
                    ColumnExtensions.NoWrap(
                        AlignableExtensions.RightAligned(
                            new TableColumn("Threads")
                        )
                    ),
                    ColumnExtensions.NoWrap(
                        AlignableExtensions.RightAligned(
                            new TableColumn("Start")
                        )
                    ),
                    ColumnExtensions.NoWrap(
                        AlignableExtensions.RightAligned(
                            new TableColumn("End")
                        )
                    ),
                    ColumnExtensions.NoWrap(
                        AlignableExtensions.RightAligned(
                            new TableColumn("Duration")
                        )
                    )
                );

                foreach (var session in sessions)
                {
                    long iterations = 0;

                    session.per_threads_stats.ForEach(p => iterations += p.iterations);

                    TableExtensions.AddRow(
                        table,
                        session.name.ToString(),
                        iterations.ToString(),
                        session.threads.ToString(),
                        session.stats_start.ToString(),
                        session.stats_end.ToString(),
                        (session.stats_end - session.stats_start).ToString()
                    );
                }

                AnsiConsoleSettings ansiConsoleSettings = new();
                var console2 = AnsiConsole.Create(ansiConsoleSettings);
                console2.Write(table);
            }
        }

        protected abstract void prepareBenchmark(XManager manager);

        sealed public override void Run(TestGroup nullableInstance)
        {
            XManager manager = new();
            prepareBenchmark(manager);
            manager.Run();
        }
    }
}
