namespace Vectantic.Core.Exceptions;

public class VectanticDownloadException : VectanticException {
    private const string DefaultMessage = """
    Failed to download the model or additional files.
    Check your internet connection and verify the preset URLs.
    """;
    
    public VectanticDownloadException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}