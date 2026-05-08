namespace Vectantic.Core.Models;

/// <summary>
/// Represents the resolved local file paths produced after a successful model download operation.
/// </summary>
/// <remarks>
/// This type contains the downloaded ONNX model path and any additional resource files
/// required by the runtime, such as tokenizer assets, onnx session and configuration files.
/// </remarks>
public record DownloadResult(
    string ModelPath,
    IReadOnlyDictionary<string, string> AdditionalPaths
);