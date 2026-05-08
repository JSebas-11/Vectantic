namespace Vectantic.Core.Exceptions;

/// <summary>
/// Represents errors that occur during model or resource download operations.
/// </summary>
/// <remarks>
/// This exception is thrown when Vectantic fails to download ONNX models,
/// tokenizer resources, or auxiliary files required during initialization.
/// Common causes include network failures, invalid resource URLs,
/// authentication issues, or checksum validation failures.
/// </remarks>
public class VectanticDownloadException : VectanticException {
    private const string DefaultMessage = """
    Failed to download the model or additional files.
    Check your internet connection and verify the preset URLs.
    """;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticDownloadException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the download failure.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticDownloadException(
        string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}