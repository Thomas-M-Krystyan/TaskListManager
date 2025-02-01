namespace TaskList.Logic.Extensions
{
    /// <summary>
    /// The extension methods for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the most meaningful exception message.
        /// </summary>
        /// <remarks>
        ///   NOTE: Error messages are trimmed to not include the trailing period.
        /// </remarks>
        public static string GetMessage(this Exception exception)
        {
            string exceptionMessage = exception.InnerException?.Message ?? exception.Message;

            // Formatting error messages to trim periods, so all the application messages are following the same standard
            return exceptionMessage.EndsWith('.')
                ? exceptionMessage[..^1]  // This case will happen only in the original .NET system exceptions
                : exceptionMessage;
        }
    }
}
