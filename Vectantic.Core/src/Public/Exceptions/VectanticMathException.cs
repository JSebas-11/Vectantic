namespace Vectantic.Core.Exceptions;

public class VectanticMathException : VectanticException {
    private const string DefaultMessage = """
    An unexpected error occurred during a math operation.
    Verify that dimensions, constraints, values, etc. are valid and compatible.
    """;

    public VectanticMathException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}