
namespace MeasureMap.Diagnostics
{
    /// <summary>
    /// The LogWriter interface
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Writes the message to the logdestination
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="source"></param>
        void Write(string message, LogLevel level, string source = null);
    }
}
