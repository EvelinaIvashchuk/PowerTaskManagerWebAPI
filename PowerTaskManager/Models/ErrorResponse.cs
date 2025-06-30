using System.Text.Json.Serialization;

namespace PowerTaskManager.Models
{
    /// <summary>
    /// Represents a standardized error response for the API
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the detailed error information (only included in development environment)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Details { get; set; }

        /// <summary>
        /// Creates a new instance of ErrorResponse
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="code">The error code</param>
        /// <param name="details">The detailed error information</param>
        public ErrorResponse(string message, string code = null, string details = null)
        {
            Message = message;
            Code = code;
            Details = details;
        }
    }
}