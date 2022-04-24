using System;

namespace MeasureMap
{
    /// <summary>
    /// Exception that is thrown when a condition is not met
    /// </summary>
    public class AssertionException : Exception
    {
        /// <summary>
        /// Exception that is thrown when a condition is not met
        /// </summary>
        /// <param name="message">The message of the exception</param>
        public AssertionException(string message)
            : base(message)
        {
        }
    }
}
