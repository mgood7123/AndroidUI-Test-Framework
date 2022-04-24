
namespace MeasureMap
{
    /// <summary>
    /// Enumeration for ResultValues that are stored in <see cref="ProfilerResult"/>
    /// </summary>
    public class ResultValueType : Enumeration
    {
        /// <summary>
        /// Warmup
        /// </summary>
        public static readonly ResultValueType Warmup = new ResultValueType(1, "Warmup");

        /// <summary>
        /// Elapsed
        /// </summary>
        public static readonly ResultValueType Elapsed = new ResultValueType(2, "Elapsed");

        private ResultValueType(int id, string name) : base(id, name)
        {
        }
    }
}
