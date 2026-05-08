namespace Vectantic.Core.Exceptions;

/// <summary>
/// Represents errors caused by invalid or incomplete object construction configuration.
/// </summary>
/// <remarks>
/// This exception is typically thrown by builders and configuration components
/// when required values are missing, invalid, or incompatible.
/// </remarks>
public class VectanticInvalidConstructionException : VectanticException {
    private const string DefaultMessage = """
    Object construction failed due to missing or invalid configuration. 
    Ensure all required fields are set.
    """;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticInvalidConstructionException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the invalid construction state.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticInvalidConstructionException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}