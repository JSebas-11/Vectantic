namespace Vectantic.Core.Exceptions;

/// <summary>
/// Represents errors that occur during ONNX model loading or inference initialization.
/// </summary>
/// <remarks>
/// This exception is thrown when the ONNX runtime cannot load,
/// validate, or execute the configured model.
/// Common causes include invalid model files, incompatible tensor shapes,
/// missing resources, or unsupported runtime configurations.
/// </remarks>
public class VectanticModelException : VectanticException {
    private const string DefaultMessage = """
    Failed to load or initialize the ONNX model. 
    Verify the model file exists, is a valid ONNX format, and matches the expected inputs.
    """;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticModelException"/> class.
    /// </summary>
    /// <param name="message">
    /// The exception message describing the model initialization failure.
    /// </param>
    /// <param name="inner">
    /// The exception that caused the current exception.
    /// </param>
    public VectanticModelException(string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}