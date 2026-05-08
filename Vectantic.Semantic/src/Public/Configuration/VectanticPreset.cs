using Vectantic.Core.Configuration;
using Vectantic.Semantic.Builders;
using Vectantic.Semantic.Enums;

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
public sealed class VectanticPreset : VectanticModelInfo {
    
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
        IReadOnlyList<Uri> tokenizerFiles,
        PoolingStrategy pooling,
        TokenizationType tokenization,
        int? maxTokens,
        bool requiresTokenTypeIds) 
        : base(id, modelUrl, checksum, TokenizerFiles2Dict(tokenizerFiles))
    {
        LowerCase = lowercase;
        OutputTensorName = outputTensorName;
        TokenizerFiles = tokenizerFiles;
        Pooling = pooling;
        Tokenization = tokenization;
        RequiresTokenTypeIds = requiresTokenTypeIds;
        MaxTokens = maxTokens;
    }

    private static IReadOnlyDictionary<string, Uri> TokenizerFiles2Dict(IReadOnlyList<Uri> tokFiles)
        => tokFiles.ToDictionary(uri => Path.GetFileName(uri.AbsolutePath), uri => uri);

    #region DEFAULT MODELS

    /// <summary>
    /// Gets the built-in preset configuration for the sentence-transformers/all-MiniLM-L6-v2 model.
    /// </summary>
    /// <remarks>
    /// This preset uses WordPiece tokenization with mean pooling and is optimized
    /// for lightweight semantic embedding workloads.
    /// </remarks>
    public static VectanticPreset MiniLML6V2 { get; } = new PresetBuilder()
        .WithId("all-MiniLM-L6-v2")
        .WithModelUrl("https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/onnx/model.onnx")
        .WithChecksum("6fd5d72fe4589f189f8ebc006442dbb529bb7ce38f8082112682524616046452")
        .ApplyLowerCase(true)
        .WithTokenTypeIds(true)
        .WithOutputTensorName("last_hidden_state")
        .WithTokenizerFiles([
            "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/tokenizer.json",
            "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/tokenizer_config.json",
            "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/special_tokens_map.json",
            "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/vocab.txt"
        ])
        .WithMaxTokens(512)
        .WithPoolingStrategy(PoolingStrategy.Mean)
        .WithTokenizationType(TokenizationType.WordPiece)
        .Build();
    
    /// <summary>
    /// Gets the built-in preset configuration for the BAAI/bge-small-en-v1.5 model.
    /// </summary>
    /// <remarks>
    /// This preset uses WordPiece tokenization with mean pooling and is optimized
    /// for high-quality English semantic retrieval tasks.
    /// </remarks>
    public static VectanticPreset BgeSmallEnV15 { get; } = new PresetBuilder()
        .WithId("bge-small-en-v1.5")
        .WithModelUrl("https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/onnx/model.onnx")
        .WithChecksum("828e1496d7fabb79cfa4dcd84fa38625c0d3d21da474a00f08db0f559940cf35")
        .ApplyLowerCase(true)
        .WithTokenTypeIds(true)
        .WithOutputTensorName("last_hidden_state")
        .WithTokenizerFiles([
            "https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/tokenizer.json",
            "https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/tokenizer_config.json",
            "https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/special_tokens_map.json",
            "https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/vocab.txt"
        ])
        .WithMaxTokens(512)
        .WithPoolingStrategy(PoolingStrategy.Mean)
        .WithTokenizationType(TokenizationType.WordPiece)
        .Build();

    #endregion
}