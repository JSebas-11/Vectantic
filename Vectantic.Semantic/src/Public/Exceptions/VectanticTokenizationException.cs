using Vectantic.Core.Exceptions;

namespace Vectantic.Semantic.Exceptions;

public class VectanticTokenizationException : VectanticException {
    private const string DefaultMessage = """
    An error occurred during tokenization. 
    Verify the tokenizer files are present and the input text is valid.
    """;

    public VectanticTokenizationException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}