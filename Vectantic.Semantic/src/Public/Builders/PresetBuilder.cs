using Vectantic.Core.Exceptions;
using Vectantic.Core.Internal.Utilities;
using Vectantic.Semantic.Configuration;
using Vectantic.Semantic.Enums;

namespace Vectantic.Semantic.Builders;

/// <summary>
/// Provides a fluent API for constructing immutable <see cref="VectanticPreset"/> instances.
/// </summary>
/// <remarks>
/// This builder validates model metadata, tokenizer resources, and runtime configuration
/// before producing a semantic model preset. All generated presets are immutable and can be
/// safely reused across dependency injection registrations.
/// </remarks>
/// <example>
/// <code>
/// var preset = new PresetBuilder()
///     .WithId("custom-model")
///     .WithModelUrl("https://example.com/model.onnx")
///     .WithChecksum("abc123")
///     .WithTokenizerFiles(new[] {
///         "https://example.com/tokenizer.json"
///     })
///     .WithPoolingStrategy(PoolingStrategy.Mean)
///     .WithTokenizationType(TokenizationType.WordPiece)
///     .Build();
/// </code>
/// </example>
public class PresetBuilder {
    #region INIT
    private string? _id;
    private Uri? _modelUri;
    private string? _checksum;
    private bool _lowercase = true;
    private bool _requiresTokenTypeIds = true;
    private string _outputTensorName = "last_hidden_state";
    private List<Uri> _tokenizerFiles = [];
    private PoolingStrategy _pooling = PoolingStrategy.Mean;
    private TokenizationType _tokenization = TokenizationType.WordPiece;
    private int? _maxTokens;
    #endregion
    
    #region BUILD

    /// <summary>
    /// Builds an immutable <see cref="VectanticPreset"/> instance using the configured values.
    /// </summary>
    /// <returns>
    /// A validated <see cref="VectanticPreset"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when required preset configuration values were not provided.
    /// </exception>
    /// <remarks>
    /// A valid preset requires an identifier, model URL, checksum,
    /// and at least one tokenizer resource file.
    /// </remarks>
    public VectanticPreset Build() {
        StringGuard.RequireOrException(_id, "Id", "is required and it was not provided.");
        
        if (_modelUri is null)
            throw new VectanticInvalidConstructionException("ModelUrl is required and it was not provided.");
        
        StringGuard.RequireOrException(_checksum, "Checksum", "is required and it was not provided.");
        
        if (_tokenizerFiles.Count == 0)
            throw new VectanticInvalidConstructionException("At least one TokenizerFile must be provided.");

        return new VectanticPreset(
            _id!, _modelUri, _checksum!, 
            _lowercase,
            _outputTensorName,
            _tokenizerFiles.AsReadOnly(), 
            _pooling, _tokenization,
            _maxTokens,
            _requiresTokenTypeIds
        );
    }
    #endregion

    #region CONFIG
    
    /// <summary>
    /// Sets the unique identifier of the preset.
    /// </summary>
    /// <param name="id">
    /// The unique model identifier.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="id"/> is null, empty, or whitespace.
    /// </exception>
    public PresetBuilder WithId(string id) {
        StringGuard.RequireOrException(id, "Preset's Id");
        
        _id = id;
        return this;
    }
    
    /// <summary>
    /// Sets the ONNX model URL using a string representation.
    /// </summary>
    /// <param name="modelUrl">
    /// The remote URI of the ONNX model file.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="modelUrl"/> is not a valid absolute URI.
    /// </exception>
    public PresetBuilder WithModelUrl(string modelUrl) {
        UriGuard.ValidateOrException(modelUrl, "ModelUrl");
        
        _modelUri = new Uri(modelUrl);
        return this;
    }
    
    /// <summary>
    /// Sets the ONNX model URL.
    /// </summary>
    /// <param name="modelUrl">
    /// The remote URI of the ONNX model file.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="modelUrl"/> is invalid.
    /// </exception>
    public PresetBuilder WithModelUrl(Uri modelUrl) {
        UriGuard.ValidateOrException(modelUrl, "ModelUrl");

        _modelUri = modelUrl;
        return this;
    }
    
    /// <summary>
    /// Sets the SHA-256 checksum used to validate the downloaded model file.
    /// </summary>
    /// <param name="checksum">
    /// The checksum used for integrity verification.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="checksum"/> is null, empty, or whitespace.
    /// </exception>
    public PresetBuilder WithChecksum(string checksum) {
        StringGuard.RequireOrException(checksum, "Preset's Checksum");
        
        _checksum = checksum;
        return this;
    }
    
    /// <summary>
    /// Configures whether input text should be lowercased before tokenization.
    /// </summary>
    /// <param name="lowercase">
    /// <see langword="true"/> to lowercase input text before tokenization; otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    public PresetBuilder ApplyLowerCase(bool lowercase) {
        _lowercase = lowercase;
        return this;
    }
    
    /// <summary>
    /// Configures whether token type IDs are required during model inference.
    /// </summary>
    /// <param name="required">
    /// <see langword="true"/> if token type IDs are required; otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    public PresetBuilder WithTokenTypeIds(bool required) {
        _requiresTokenTypeIds = required;
        return this;
    }

    /// <summary>
    /// Sets the ONNX output tensor name containing token embeddings.
    /// </summary>
    /// <param name="outTensorName">
    /// The output tensor name used during pooling extraction.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="outTensorName"/> is null, empty, or whitespace.
    /// </exception>
    public PresetBuilder WithOutputTensorName(string outTensorName) {
        StringGuard.RequireOrException(outTensorName, "Preset's Output Tensor Name");
        
        _outputTensorName = outTensorName;
        return this;
    }
    
    /// <summary>
    /// Replaces the tokenizer resource collection using string URI values.
    /// </summary>
    /// <param name="tokenizerFiles">
    /// The tokenizer resource URIs required by the preset.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when one or more tokenizer resource URIs are invalid.
    /// </exception>
    public PresetBuilder WithTokenizerFiles(IEnumerable<string> tokenizerFiles) {
        _tokenizerFiles = UriGuard.CreateListOrException(tokenizerFiles, "TokenizerFiles");
        return this;
    }

    /// <summary>
    /// Replaces the tokenizer resource collection.
    /// </summary>
    /// <param name="tokenizerFiles">
    /// The tokenizer resource URIs required by the preset.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when one or more tokenizer resource URIs are invalid.
    /// </exception>
    public PresetBuilder WithTokenizerFiles(IEnumerable<Uri> tokenizerFiles) {
        _tokenizerFiles = UriGuard.CreateListOrException(tokenizerFiles, "TokenizerFiles");
        return this;
    }
    
    /// <summary>
    /// Adds a tokenizer resource file to the preset configuration.
    /// </summary>
    /// <param name="tokenizerFile">
    /// The tokenizer resource URI.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="tokenizerFile"/> is invalid.
    /// </exception>
    /// <remarks>
    /// Duplicate tokenizer resource URIs are ignored.
    /// </remarks>
    public PresetBuilder AddTokenizerFile(Uri tokenizerFile) {
        UriGuard.ValidateOrException(tokenizerFile, "TokenizerFile");

        if (!_tokenizerFiles.Contains(tokenizerFile))
            _tokenizerFiles.Add(tokenizerFile);
        
        return this;
    }
    
    /// <summary>
    /// Adds a tokenizer resource file to the preset configuration using a string URI value.
    /// </summary>
    /// <param name="tokenizerFile">
    /// The tokenizer resource URI.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="tokenizerFile"/> is not a valid absolute URI.
    /// </exception>
    /// <remarks>
    /// Duplicate tokenizer resource URIs are ignored.
    /// </remarks>
    public PresetBuilder AddTokenizerFile(string tokenizerFile) {
        UriGuard.ValidateOrException(tokenizerFile, "TokenizerFile");
        var uri = new Uri(tokenizerFile);

        if (!_tokenizerFiles.Contains(uri))
            _tokenizerFiles.Add(uri);
        
        return this;
    }
    
    /// <summary>
    /// Sets the pooling strategy used to aggregate token embeddings.
    /// </summary>
    /// <param name="pooling">
    /// The pooling strategy used during embedding generation.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="pooling"/> is not a defined enumeration value.
    /// </exception>
    public PresetBuilder WithPoolingStrategy(PoolingStrategy pooling) {
        if (!Enum.IsDefined(pooling))
            throw new VectanticInvalidConstructionException("Invalid PoolingStrategy value.");

        _pooling = pooling;
        return this;
    }
    
    /// <summary>
    /// Sets the tokenization algorithm used by the preset.
    /// </summary>
    /// <param name="tokenization">
    /// The tokenization type used during text preprocessing.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="tokenization"/> is not a defined enumeration value.
    /// </exception>
    public PresetBuilder WithTokenizationType(TokenizationType tokenization) {
        if (!Enum.IsDefined(tokenization))
            throw new VectanticInvalidConstructionException("Invalid TokenizationType value.");

        _tokenization = tokenization;
        return this;
    }
    
    /// <summary>
    /// Sets the maximum supported token count for the model.
    /// </summary>
    /// <param name="maxTokens">
    /// The maximum number of tokens accepted by the model.
    /// </param>
    /// <returns>
    /// The current <see cref="PresetBuilder"/> instance.
    /// </returns>
    /// <exception cref="VectanticInvalidConstructionException">
    /// Thrown when <paramref name="maxTokens"/> is less than or equal to zero.
    /// </exception>
    public PresetBuilder WithMaxTokens(int maxTokens) {
        if (maxTokens <= 0)
            throw new VectanticInvalidConstructionException("Maximum tokens for model must be greater than 0.");

        _maxTokens = maxTokens;
        return this;
    }
    #endregion
}