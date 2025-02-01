namespace TaskList.Logic.Responses
{
    /// <summary>
    /// The standardized, general-purpose application response to business operations.
    /// </summary>
    public readonly struct CommandResponse
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation was unsuccessful.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The overrideContent of the response.
        /// 
        /// <para>
        ///   In case of failure, it contains the error description.
        /// </para>
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponse"/> struct.
        /// </summary>
        private CommandResponse(bool isSuccess, string content)
        {
            IsSuccess = isSuccess;
            Content = content;
        }

        /// <summary>
        /// The successful result of the operation.
        /// </summary>
        /// <param name="overrideContent">The overrideContent replacing the default content.</param>
        /// <param name="content">The text enriching the default content.</param>
        public static CommandResponse Success(string content, bool overrideContent = false)
            => new(true, overrideContent
                ? content
                : $"Operation succeeded{(string.IsNullOrWhiteSpace(content) ? string.Empty : $": {content}")}.");

        /// <summary>
        /// The unsuccessful result of the operation.
        /// </summary>
        public static CommandResponse Failure(string failureReason)
            => new(false, string.Format("Operation failed: {0}.", failureReason));

        /// <summary>
        /// The unsuccessful result of the operation.
        /// </summary>
        public static CommandResponse Failure(Exception exception)
            => new(false, string.Format("Operation failed: {0}.", exception.Message));
    }
}
