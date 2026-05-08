namespace Vectantic.Core.Exceptions;

/// <summary>
/// Represents the base exception type for all Vectantic-specific errors.
/// </summary>
/// <remarks>
/// All custom exceptions thrown by Vectantic inherit from this type.
/// It provides a unified abstraction for handling SDK-specific failures
/// across model loading, tokenization, pooling or downloading operations.
/// </remarks>
public class VectanticException : Exception {
    private const string DefaultMessage = "An unexpected error occurred in Vectantic.";

    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the failure.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}