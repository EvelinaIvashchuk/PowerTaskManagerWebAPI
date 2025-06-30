namespace PowerTaskManager.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// </summary>
    public class ResourceNotFoundException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class
        /// </summary>
        /// <param name="resourceType">The type of resource that was not found</param>
        /// <param name="resourceId">The ID of the resource that was not found</param>
        public ResourceNotFoundException(string resourceType, object resourceId)
            : base($"{resourceType} with ID {resourceId} was not found", "resource_not_found")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        public ResourceNotFoundException(string message)
            : base(message, "resource_not_found")
        {
        }
    }
}