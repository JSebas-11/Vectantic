namespace Vectantic.Core.Exceptions;

/// <summary>
/// Represents errors that occur during vector, embedding or any math operations.
/// </summary>
/// <remarks>
/// This exception is used for failures related to vector dimensions,
/// invalid numerical constraints, or unsupported mathematical operations.
/// </remarks>
public class VectanticMathException : VectanticException {
    private const string DefaultMessage = """
    An unexpected error occurred during a math operation.
    Verify that dimensions, constraints, values, etc. are valid and compatible.
    """;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticMathException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the math operation failure.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticMathException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}