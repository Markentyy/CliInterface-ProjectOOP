namespace CliInterface.Data
{
    /// <summary>
    /// Error raised by persistence operations: unreadable or malformed
    /// files, missing directories. Presentation layers show the message.
    /// </summary>
    public sealed class DataException : Exception
    {
        /// <summary>
        /// Initializes a new persistence error with the given message.
        /// </summary>
        /// <param name="message">The user facing error text.</param>
        public DataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new persistence error wrapping the system cause.
        /// </summary>
        /// <param name="message">The user facing error text.</param>
        /// <param name="inner">The original system exception.</param>
        public DataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
