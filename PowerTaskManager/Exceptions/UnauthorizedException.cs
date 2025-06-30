namespace PowerTaskManager.Exceptions
{
    /// <summary>
    /// Exception thrown when a user is not authorized to perform an action
    /// </summary>
    public class UnauthorizedException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
        /// </summary>
        public UnauthorizedException()
            : base("You are not authorized to perform this action", "unauthorized")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        public UnauthorizedException(string message)
            : base(message, "unauthorized")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException, "unauthorized")
        {
        }
    }
}