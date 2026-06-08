using Vectantic.Core.Configuration;
using Vectantic.Semantic.Builders;
using Vectantic.Semantic.Enums;
using Vectantic.Semantic.Internal.Constants;

namespace Vectantic.Semantic.Configuration;

/// <summary>
/// Represents an immutable semantic embedding model preset with tokenization and pooling configuration.
/// </summary>
/// <remarks>
/// Presets define all resources and runtime metadata required to execute semantic embedding generation.
/// Instances are immutable and can only be constructed through <see cref="PresetBuilder"/>.
/// Built-in presets provide validated configurations for commonly used embedding models.
/// </remarks>
/// <example>
/// <code>
/// await services
///     .AddVectanticSemantic(
///         vecOpts => {},
///         semOpts => {},
///         VectanticPreset.MiniLML6V2)
///     .EnsureModelAsync();
/// </code>
/// </example>
public sealed partial class VectanticPreset : VectanticModelInfo {
    
    /// <summary>
    /// Gets a value indicating whether input text should be converted to lowercase before tokenization.
    /// </summary>
    /// <remarks>
    /// This value should match the preprocessing requirements of the underlying tokenizer and model.
    /// </remarks>
    public bool LowerCase { get; }

    /// <summary>
    /// Gets the name of the ONNX output tensor containing token embeddings.
    /// </summary>
    /// <remarks>
    /// This tensor is consumed by the pooling strategy to generate the final embedding vector.
    /// </remarks>
    public string OutputTensorName { get; }

    /// <summary>
    /// Gets the additional model files required for inference.
    /// </summary>
    /// <remarks>
    /// Some ONNX models depend on supplementary files in addition to the primary
    /// model file referenced by <see cref="VectanticModelInfo.ModelUrl"/>.
    ///
    /// Common examples include external ONNX data files referenced by the model,
    /// such as <c>model.onnx_data</c>. These files are downloaded and cached
    /// automatically during model initialization.
    ///
    /// The dictionary key represents the file download URI, while the value
    /// contains the expected SHA-256 checksum used for integrity verification.
    /// </remarks>
    public IReadOnlyDictionary<Uri, string> ModelFiles { get; }

    /// <summary>
    /// Gets the tokenizer resource files required for text tokenization.
    /// </summary>
    /// <remarks>
    /// Typical files include tokenizer definitions, vocabulary files,
    /// and special token mappings.
    /// </remarks>
    public IReadOnlyList<Uri> TokenizerFiles { get; }

    /// <summary>
    /// Gets the pooling strategy used to aggregate token embeddings.
    /// </summary>
    /// <remarks>
    /// Pooling determines how token-level embeddings are transformed into
    /// a single fixed-size semantic vector.
    /// </remarks>
    public PoolingStrategy Pooling { get; }

    /// <summary>
    /// Gets the tokenization algorithm used by the model.
    /// </summary>
    /// <remarks>
    /// The tokenization type must be compatible with the tokenizer resources
    /// and ONNX model architecture.
    /// </remarks>
    public TokenizationType Tokenization { get; }

    /// <summary>
    /// Gets a value indicating whether token type IDs are required during inference.
    /// </summary>
    /// <remarks>
    /// Some transformer architectures require token type IDs as an additional model input tensor.
    /// </remarks>
    public bool RequiresTokenTypeIds { get; }

    /// <summary>
    /// Gets the maximum number of tokens supported by the model.
    /// </summary>
    /// <remarks>
    /// Input sequences exceeding this limit should be truncated before inference.
    /// </remarks>
    public int? MaxTokens { get; }

    internal VectanticPreset(
        string id,
        Uri modelUrl,
        string checksum,
        bool lowercase,
        string outputTensorName,
        IReadOnlyDictionary<Uri, string> modelFiles,
        IReadOnlyList<Uri> tokenizerFiles,
        PoolingStrategy pooling,
        TokenizationType tokenization,
        int? maxTokens,
        bool requiresTokenTypeIds) 
        : base(id, modelUrl, checksum, DefineExtraFiles(modelFiles, tokenizerFiles))
    {
        LowerCase = lowercase;
        OutputTensorName = outputTensorName;
        ModelFiles = modelFiles;
        TokenizerFiles = tokenizerFiles;
        Pooling = pooling;
        Tokenization = tokenization;
        RequiresTokenTypeIds = requiresTokenTypeIds;
        MaxTokens = maxTokens;
    }

    private static IReadOnlyList<DownloadFileInfo> DefineExtraFiles(
        IReadOnlyDictionary<Uri, string> modelFiles, IReadOnlyList<Uri> tokFiles
    ) {
        var modelCount = modelFiles.Count;
        var tokCount = tokFiles.Count;
        var maxFiles = Greater(modelCount, tokCount);

        var extraFiles = new List<DownloadFileInfo>(modelCount + tokCount);

        for (int i = 0; i < maxFiles; i++) {
            if (i < modelCount) {
                var modelFile = modelFiles.ElementAt(i);
                extraFiles.Add(new DownloadFileInfo(Path.GetFileName(modelFile.Key.AbsolutePath), modelFile.Key, "/", modelFile.Value));
            }
            if (i < tokCount) {
                var url = tokFiles[i];
                extraFiles.Add(new DownloadFileInfo(Path.GetFileName(url.AbsolutePath), url, SemanticConstants.TokenizerDirKey));
            }
        }

        return extraFiles;
    }

    private static int Greater(params int[] numbers) => numbers.Max();
}