using Vectantic.Core.Exceptions;

namespace Vectantic.Semantic.Exceptions;

/// <summary>
/// Represents errors that occur during embedding pooling operations.
/// </summary>
/// <remarks>
/// This exception is thrown when token embedding tensors cannot be aggregated
/// into a final semantic vector due to invalid shapes, dimensions,
/// or pooling pipeline failures.
/// </remarks>
public class VectanticPoolingException : VectanticException {
    private const string DefaultMessage = """
    An error occurred during the pooling operation.
    Verify the model output tensor has the expected shape and dimensions.
    """;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticPoolingException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the pooling failure.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticPoolingException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}