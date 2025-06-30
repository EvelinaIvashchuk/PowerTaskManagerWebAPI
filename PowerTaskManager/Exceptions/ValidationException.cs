using System.Collections.Generic;

namespace PowerTaskManager.Exceptions
{
    /// <summary>
    /// Exception thrown when validation fails
    /// </summary>
    public class ValidationException : AppException
    {
        /// <summary>
        /// Gets the validation errors
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        public ValidationException(string message)
            : base(message, "validation_failed")
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class
        /// </summary>
        /// <param name="errors">The validation errors</param>
        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred", "validation_failed")
        {
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="errors">The validation errors</param>
        public ValidationException(string message, IDictionary<string, string[]> errors)
            : base(message, "validation_failed")
        {
            Errors = errors;
        }
    }
}