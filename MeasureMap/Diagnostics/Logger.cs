using System.Collections.Generic;

namespace MeasureMap.Diagnostics
{
    /// <inheritdoc />
    public class Logger : ILogger
    {
        private readonly List<ILogWriter> _writers;

        /// <inheritdoc />
        public Logger()
        {
            _writers = new List<ILogWriter>();
        }

        /// <inheritdoc />
        public void Write(string message, LogLevel level = LogLevel.Info, string source = null)
        {
            foreach(var writer in _writers)
            {
                writer.Write(message, level, source);
            }
        }

        /// <summary>
        /// Create a new logger
        /// </summary>
        /// <returns></returns>
        public static Logger Setup()
        {
            var logger = new Logger();
            foreach(var writer in GlobalConfiguration.LogWriters)
            {
                logger._writers.Add(writer);
            }

            return logger;
        }
    }
}
