namespace Vectantic.Core.Exceptions;

public class VectanticInvalidConstructionException : VectanticException {
    private const string DefaultMessage = """
    Object construction failed due to missing or invalid configuration. 
    Ensure all required fields are set.
    """;

    public VectanticInvalidConstructionException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}