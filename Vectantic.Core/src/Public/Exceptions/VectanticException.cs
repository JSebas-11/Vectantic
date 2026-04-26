namespace Vectantic.Core.Exceptions;

public class VectanticException : Exception {
    private const string DefaultMessage = "An unexpected error occurred in Vectantic.";

    public VectanticException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}