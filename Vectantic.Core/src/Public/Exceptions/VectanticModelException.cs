namespace Vectantic.Core.Exceptions;

public class VectanticModelException : VectanticException {
    private const string DefaultMessage = """
    Failed to load or initialize the ONNX model. 
    Verify the model file exists, is a valid ONNX format, and matches the expected inputs.
    """;
    
    public VectanticModelException(string? message = null, Exception? inner = null) 
        : base(message ?? DefaultMessage, inner)
    { }
}