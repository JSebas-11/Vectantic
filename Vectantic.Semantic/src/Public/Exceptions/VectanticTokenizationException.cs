using Vectantic.Core.Exceptions;

namespace Vectantic.Semantic.Exceptions;

/// <summary>
/// Represents errors that occur during text tokenization operations.
/// </summary>
/// <remarks>
/// This exception is thrown when input text cannot be tokenized due to
/// invalid tokenizer resources, malformed tokenizer configuration,
/// or incompatible tokenization inputs.
/// </remarks>
public class VectanticTokenizationException : VectanticException {
    private const string DefaultMessage = """
    An error occurred during tokenization. 
    Verify the tokenizer files are present and the input text is valid.
    """;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticTokenizationException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the tokenization failure.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticTokenizationException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}