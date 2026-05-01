using Vectantic.Core.Exceptions;
using Vectantic.Core.Internal.Utilities;
using Vectantic.Semantic.Configuration;
using Vectantic.Semantic.Enums;

namespace Vectantic.Semantic.Builders;

public class PresetBuilder {
    // -------------------- INIT --------------------
    private string? _id;
    private Uri? _modelUri;
    private string? _checksum;
    private bool _lowercase = true;
    private string _outputTensorName = "last_hidden_state";
    private List<Uri> _tokenizerFiles = [];
    private PoolingStrategy _pooling = PoolingStrategy.Mean;
    private TokenizationType _tokenization = TokenizationType.WordPiece;
    private int? _maxTokens;
    
    // -------------------- BUILD --------------------
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
            _maxTokens
        );
    }

    // -------------------- CONFIG --------------------
    public PresetBuilder WithId(string id) {
        StringGuard.RequireOrException(id, "Preset's Id");
        
        _id = id;
        return this;
    }
    
    public PresetBuilder WithModelUrl(string modelUrl) {
        UriGuard.ValidateOrException(modelUrl, "ModelUrl");
        
        _modelUri = new Uri(modelUrl);
        return this;
    }
    
    public PresetBuilder WithModelUrl(Uri modelUrl) {
        UriGuard.ValidateOrException(modelUrl, "ModelUrl");

        _modelUri = modelUrl;
        return this;
    }
    
    public PresetBuilder WithChecksum(string checksum) {
        StringGuard.RequireOrException(checksum, "Preset's Checksum");
        
        _checksum = checksum;
        return this;
    }
    
    public PresetBuilder ApplyLowerCase(bool lowercase) {
        _lowercase = lowercase;
        return this;
    }

    public PresetBuilder WithOutputTensorName(string outTensorName) {
        StringGuard.RequireOrException(outTensorName, "Preset's Output Tensor Name");
        
        _outputTensorName = outTensorName;
        return this;
    }
    
    public PresetBuilder WithTokenizerFiles(IEnumerable<string> tokenizerFiles) {
        _tokenizerFiles = UriGuard.CreateListOrException(tokenizerFiles, "TokenizerFiles");
        return this;
    }

    public PresetBuilder WithTokenizerFiles(IEnumerable<Uri> tokenizerFiles) {
        _tokenizerFiles = UriGuard.CreateListOrException(tokenizerFiles, "TokenizerFiles");
        return this;
    }
    
    public PresetBuilder AddTokenizerFile(Uri tokenizerFile) {
        UriGuard.ValidateOrException(tokenizerFile, "TokenizerFile");

        if (!_tokenizerFiles.Contains(tokenizerFile))
            _tokenizerFiles.Add(tokenizerFile);
        
        return this;
    }
    
    public PresetBuilder AddTokenizerFile(string tokenizerFile) {
        UriGuard.ValidateOrException(tokenizerFile, "TokenizerFile");
        var uri = new Uri(tokenizerFile);

        if (!_tokenizerFiles.Contains(uri))
            _tokenizerFiles.Add(uri);
        
        return this;
    }
    
    public PresetBuilder WithPoolingStrategy(PoolingStrategy pooling) {
        if (!Enum.IsDefined(pooling))
            throw new VectanticInvalidConstructionException("Invalid PoolingStrategy value.");

        _pooling = pooling;
        return this;
    }
    
    public PresetBuilder WithTokenizationType(TokenizationType tokenization) {
        if (!Enum.IsDefined(tokenization))
            throw new VectanticInvalidConstructionException("Invalid TokenizationType value.");

        _tokenization = tokenization;
        return this;
    }
    
    public PresetBuilder WithMaxTokens(int maxTokens) {
        if (maxTokens <= 0)
            throw new VectanticInvalidConstructionException("Maximum tokens for model must be greater than 0.");

        _maxTokens = maxTokens;
        return this;
    }
}