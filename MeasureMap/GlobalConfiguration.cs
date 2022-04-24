using System.Collections.Generic;
using MeasureMap.Diagnostics;

namespace MeasureMap
{
    /// <summary>
    /// The configuration
    /// </summary>
    public static class GlobalConfiguration
    {
        /// <summary>
        /// Add a logwriter to log to
        /// </summary>
        /// <param name="writer"></param>
        public static void AddLogWriter(ILogWriter writer)
        {
            LogWriters.Add(writer);
        }

        internal static List<ILogWriter> LogWriters { get; } = new List<ILogWriter>();
    }
}
