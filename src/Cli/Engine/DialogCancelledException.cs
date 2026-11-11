namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Signals that the user aborted an interactive dialog with "cancel".
    /// The REPL loop catches it and prints a short note.
    /// </summary>
    public sealed class DialogCancelledException : Exception
    {
        /// <summary>
        /// Initializes a new dialog cancellation signal.
        /// </summary>
        public DialogCancelledException()
            : base("Cancelled.")
        {
        }
    }
}
