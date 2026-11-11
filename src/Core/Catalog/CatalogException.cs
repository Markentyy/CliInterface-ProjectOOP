namespace CliInterface.Core.Catalog
{
    /// <summary>
    /// Error raised by catalog operations: duplicate ids, ambiguous or
    /// missing names. Presentation layers map it to user facing messages.
    /// </summary>
    public sealed class CatalogException : Exception
    {
        /// <summary>
        /// Initializes a new catalog error with the given message.
        /// </summary>
        /// <param name="message">The user facing error text.</param>
        public CatalogException(string message)
            : base(message)
        {
        }
    }
}
