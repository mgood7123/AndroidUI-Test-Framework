
namespace MeasureMap.Diagnostics
{
    /// <summary>
    /// Logwritert that writes all messages to the diagnostics console
    /// </summary>
    public class TraceLogWriter : ILogWriter
    {
        /// <inheritdoc />
        public void Write(string message, LogLevel level, string source)
        {
            System.Diagnostics.Trace.WriteLine($"[{source}] [{level}] {message}");
        }
    }
}
