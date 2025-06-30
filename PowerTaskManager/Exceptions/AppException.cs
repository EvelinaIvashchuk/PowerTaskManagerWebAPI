namespace PowerTaskManager.Exceptions
{
    /// <summary>
    /// Base exception class for application-specific exceptions
    /// </summary>
    public class AppException : Exception
    {
        /// <summary>
        /// Gets the error code associated with this exception
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="errorCode">The error code</param>
        public AppException(string message, string errorCode = "application_error") 
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        /// <param name="errorCode">The error code</param>
        public AppException(string message, Exception innerException, string errorCode = "application_error") 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}