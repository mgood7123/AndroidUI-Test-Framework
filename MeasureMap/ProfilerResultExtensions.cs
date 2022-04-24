using System;
using System.Linq;
using System.Text;

namespace MeasureMap
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProfilerResultExtensions
    {
        /// <summary>
        /// Trace the result to the Console
        /// </summary>
        public static string Trace(this IProfilerResult profilerResult, string header = "### MeasureMap - Profiler result for Profilesession")
            => Trace(profilerResult, false, header);

        /// <summary>
        /// Trace the result to the Console
        /// </summary>
        public static string Trace(this IProfilerResult profilerResult, bool fullTrace, string header = "### MeasureMap - Profiler result for Profilesession")
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(header))
            {
                sb.AppendLine(header);
            }

            sb.AppendLine($"##### Summary");
            sb.AppendLine($"\tWarmup ========================================");
            sb.AppendLine($"\t\tDuration Warmup:\t\t\t{profilerResult.Warmup().ToString()}");
            sb.AppendLine($"\tSetup ========================================");
            sb.AppendLine($"\t\tIterations:\t\t\t{profilerResult.Iterations.Count()}");
            sb.AppendLine($"\tDuration ========================================");
            sb.AppendLine($"\t\tDuration Total:\t\t\t{profilerResult.TotalTime.ToString()}");
            sb.AppendLine($"\t\tAverage Time:\t\t\t{profilerResult.AverageTime}");
            sb.AppendLine($"\t\tAverage Milliseconds:\t\t{profilerResult.AverageMilliseconds}");
            sb.AppendLine($"\t\tAverage Ticks:\t\t\t{profilerResult.AverageTicks}");
            sb.AppendLine($"\t\tFastest:\t\t\t{TimeSpan.FromTicks(profilerResult.Fastest.Ticks)}");
            sb.AppendLine($"\t\tSlowest:\t\t\t{TimeSpan.FromTicks(profilerResult.Slowest.Ticks)}");
            sb.AppendLine($"\tMemory ==========================================");
            sb.AppendLine($"\t\tMemory Initial size:\t\t{profilerResult.InitialSize}");
            sb.AppendLine($"\t\tMemory End size:\t\t{profilerResult.EndSize}");
            sb.AppendLine($"\t\tMemory Increase:\t\t{profilerResult.Increase}");

            if (fullTrace)
            {
                sb.AppendLine($"##### Iterations");
                sb.AppendLine("Timestamp | Duration | Init size | End size");
                sb.AppendLine("--- | --- | ---: | ---:");
                foreach (var iteration in profilerResult.Iterations)
                {
                    sb.AppendLine($"{iteration.TimeStamp} | {iteration.Duration} | {iteration.InitialSize} | {iteration.AfterExecution}");
                }
            }

            var result = sb.ToString();
            
            System.Diagnostics.Trace.WriteLine(result);

            return result;
        }

        /// <summary>
        /// Returns the timespan that the Warmup took
        /// </summary>
        /// <param name="result">The ProfilerResult</param>
        /// <returns>The timespan that the warmup took</returns>
        public static TimeSpan Warmup(this IProfilerResult result)
        {
            if (result.ResultValues.ContainsKey(ResultValueType.Warmup))
            {
                return (TimeSpan)result.ResultValues[ResultValueType.Warmup];
            }

            return new TimeSpan();
        }

        /// <summary>
        /// Returns the timespan that the complete Session took
        /// </summary>
        /// <param name="result">The ProfilerResult</param>
        /// <returns>The timespan that the session took</returns>
        public static TimeSpan Elapsed(this IProfilerResult result)
        {
            if (result.ResultValues.ContainsKey(ResultValueType.Elapsed))
            {
                return (TimeSpan)result.ResultValues[ResultValueType.Elapsed];
            }

            return new TimeSpan();
        }
    }
}
