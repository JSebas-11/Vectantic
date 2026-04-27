using Vectantic.Core.Exceptions;

namespace Vectantic.Semantic.Exceptions;

public class VectanticPoolingException : VectanticException {
    private const string DefaultMessage = """
    An error occurred during the pooling operation.
    Verify the model output tensor has the expected shape and dimensions.
    """;

    public VectanticPoolingException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}