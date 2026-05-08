namespace Vectantic.Core.Configuration;

/// <summary>
/// Defines the metadata and downloadable resources required to resolve and load an ONNX model.
/// </summary>
/// <remarks>
/// Serves as the base configuration type for model definitions used by the Vectantic pipeline.
/// This type encapsulates the ONNX model source, integrity checksum, and any additional files
/// required to initialize the model runtime, such as tokenizer assets.
/// </remarks>
public class VectanticModelInfo {

    /// <summary>
    /// Gets the unique identifier of the model configuration.
    /// </summary>
    /// <remarks>
    /// This value is typically aligned with the upstream HuggingFace model identifier
    /// and is used for cache resolution and model organization.
    /// </remarks>
    public string Id { get; }

    /// <summary>
    /// Gets the remote location of the ONNX model file.
    /// </summary>
    /// <remarks>
    /// The referenced file is downloaded and cached locally during model initialization.
    /// </remarks>
    public Uri ModelUrl { get; }

    /// <summary>
    /// Gets the SHA-256 checksum used to validate the downloaded model file.
    /// </summary>
    /// <remarks>
    /// The checksum is verified after download to ensure model integrity and prevent
    /// loading corrupted or tampered files.
    /// </remarks>
    public string Checksum { get; }

    /// <summary>
    /// Gets the additional files required by the model runtime.
    /// </summary>
    /// <remarks>
    /// Common examples include tokenizer configuration files, vocabularies,
    /// and special token mappings required during tokenization.
    /// </remarks>
    public IReadOnlyDictionary<string, Uri> ExtraFiles { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VectanticModelInfo"/> class.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the model configuration.
    /// </param>
    /// <param name="modelUrl">
    /// The remote URI of the ONNX model file.
    /// </param>
    /// <param name="checksum">
    /// The SHA-256 checksum used to validate the downloaded model file.
    /// </param>
    /// <param name="extraFiles">
    /// An optional collection of additional files required by the model runtime.
    /// </param>
    protected VectanticModelInfo(
        string id, Uri modelUrl, string checksum,
        IReadOnlyDictionary<string, Uri>? extraFiles = null) 
    {
        Id = id;
        ModelUrl = modelUrl;
        Checksum = checksum;
        ExtraFiles = extraFiles ?? new Dictionary<string, Uri>();
    }
}