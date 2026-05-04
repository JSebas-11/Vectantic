using Vectantic.Core.Configuration;
using Vectantic.Semantic.Builders;
using Vectantic.Semantic.Enums;

namespace Vectantic.Semantic.Configuration;

public sealed class VectanticPreset : VectanticModelInfo {
    public bool LowerCase { get; }
    public string OutputTensorName { get; }
    public IReadOnlyList<Uri> TokenizerFiles { get; }
    public PoolingStrategy Pooling { get; }
    public TokenizationType Tokenization { get; }
    public bool RequiresTokenTypeIds { get; }
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

    // -------------------- DEFAULT MODELS --------------------
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
}