using System;
using MeasureMap.Runners;

namespace MeasureMap
{
    /// <summary>
    /// Extension class for ProfilerSession
    /// </summary>
    public static class ProfilerSessionExtensions
    {
        /// <summary>
        /// Sets the Task that will be profiled
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="task">The Task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession Task(this ProfilerSession session, Action task)
        {
            session.Task(new Task(task));

            return session;
        }

        /// <summary>
        /// Sets the Task that will be profiled
        /// </summary>
        /// <param name="session">The current session</param>
        /// <typeparam name="T">The return and parameter value</typeparam>
        /// <param name="task">The task to execute</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession Task<T>(this ProfilerSession session, Func<T, T> task)
        {
            session.Task(new Task<T>(task));

            return session;
        }

        /// <summary>
        /// Sets the Task that will be profiled
        /// </summary>
        /// <param name="session">The current session</param>
        /// <typeparam name="T">The return and parameter value</typeparam>
        /// <param name="task">The task to execute</param>
        /// <param name="parameter">The parameter that is passed to the task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession Task<T>(this ProfilerSession session, Func<T, T> task, T parameter)
        {
            session.Task(new Task<T>(task, parameter));

            return session;
        }

        /// <summary>
        /// Sets the Task that will be profiled passing the current ExecutionContext as parameter
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="task">The task to execute</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession Task(this ProfilerSession session, Action<IExecutionContext> task)
        {
            session.Task(new ContextTask(task));

            return session;
        }

        /// <summary>
        /// Sets the Task that will be profiled passing the current ExecutionContext as parameter
        /// </summary>
        /// <typeparam name="T">The expected task output</typeparam>
        /// <param name="session">The current session</param>
        /// <param name="task">The task to execute</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession Task<T>(this ProfilerSession session, Func<IExecutionContext, T> task)
        {
            session.Task(new OutputTask<T>(task));

            return session;
        }

        /// <summary>
        /// Sets the amount of iterations that the profileing session should run the task
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="iterations">The iterations to run the task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession SetIterations(this ProfilerSession session, int iterations)
        {
            session.Settings.Iterations = iterations;

            return session;
        }

        /// <summary>
        /// Sets the duration that the profileing session should run the task for
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="duration">The iterations to run the task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession SetDuration(this ProfilerSession session, TimeSpan duration)
        {
            session.Settings.Duration = duration;

            return session;
        }

        /// <summary>
        /// The task will be executed at the given interval.
        /// To ensure the execution interval, the task is executed in a new thread
        /// </summary>
        /// <param name="session"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static ProfilerSession SetInterval(this ProfilerSession session, TimeSpan interval)
        {
            session.Settings.Execution = new TimedTaskExecution(interval);

            return session;
        }

        public static ProfilerSession RunWarmup(this ProfilerSession session, bool run)
        {
            session.Settings.RunWarmup = run;

            return session;
        }

        /// <summary>
        /// Sets the settings that the profiler should use
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="settings">The settings for thr profiler</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession SetSettings(this ProfilerSession session, ProfilerSettings settings)
        {
            settings.MergeChangesTo(session.Settings);

            return session;
        }

		/// <summary>
		/// Set values in the settings
		/// </summary>
		/// <param name="session">The current session</param>
		/// <param name="settings">The settings for thr profiler</param>
		/// <returns></returns>
		public static ProfilerSession Settings(this ProfilerSession session, Action<ProfilerSettings> settings)
        {
	        settings(session.Settings);

	        return session;
        }

		/// <summary>
		/// Add the middleware to the processing pipeline
		/// </summary>
		/// <param name="session">The current session</param>
		/// <param name="middleware">The middleware to add</param>
		/// <returns></returns>
		public static ProfilerSession AddMiddleware(this ProfilerSession session, ITaskMiddleware middleware)
        {
            session.ProcessingPipeline.SetNext(middleware);
            return session;
        }

        /// <summary>
        /// Add the middleware to the session pipeline
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="middleware">The middleware to add</param>
        /// <returns></returns>
        public static ProfilerSession AddMiddleware(this ProfilerSession session, ISessionHandler middleware)
        {
            session.SessionPipeline.SetNext(middleware);
            return session;
        }

        /// <summary>
        /// Sets a Task that will be executed before each profiling task execution
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="task">The task to execute before each profiling task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession PreExecute(this ProfilerSession session, Action task)
        {
            return session.AddMiddleware(new PreExecutionTaskHandler(task));
        }

        /// <summary>
        /// Sets a Task that will be executed before each profiling task execution
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="task">The task to execute before each profiling task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession PreExecute(this ProfilerSession session, Action<IExecutionContext> task)
        {
            return session.AddMiddleware(new PreExecutionTaskHandler(task));
        }

        /// <summary>
        /// Sets a Task that will be executed after each profiling task execution
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="task">The task to execute after each profiling task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession PostExecute(this ProfilerSession session, Action task)
        {
            return session.AddMiddleware(new PostExecutionTaskHandler(task));
        }

        /// <summary>
        /// Sets a Task that will be executed after each profiling task execution
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="task">The task to execute after each profiling task</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession PostExecute(this ProfilerSession session, Action<IExecutionContext> task)
        {
            return session.AddMiddleware(new PostExecutionTaskHandler(task));
        }

        /// <summary>
        /// Add a delay before each task gets executed. The delay is not countet to the execution time of the task
        /// </summary>
        /// <param name="session"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static ProfilerSession AddDelay(this ProfilerSession session, TimeSpan duration)
        {
            return session.AddMiddleware(new DelayTaskHandler(duration));
        }

        /// <summary>
        /// Adds a setup task to the sessionpipeline
        /// </summary>
        /// <param name="session">The current session</param>
        /// <param name="setup">The setuptask</param>
        /// <returns>The current profiling session</returns>
        public static ProfilerSession Setup(this ProfilerSession session, Action setup)
        {
            return session.AddMiddleware(new PreExecutionSessionHandler(setup));
        }
    }
}
