using Vectantic.Semantic.Builders;
using Vectantic.Semantic.Enums;

namespace Vectantic.Semantic.Configuration;

public sealed partial class VectanticPreset {

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
        .WithTokenizationType(TokenizationType.Bert)
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
        .WithTokenizationType(TokenizationType.Bert)
        .Build();

    /// <summary>
    /// Gets the built-in preset configuration for the sentence-transformers/all-roberta-large-v1 model.
    /// </summary>
    /// <remarks>
    /// This preset uses BPE tokenization with mean pooling and produces 1024-dimensional
    /// embeddings optimized for semantic similarity and retrieval tasks.
    /// 
    /// Token type IDs are disabled because RoBERTa models do not use segment embeddings.
    /// </remarks>
    public static VectanticPreset AllRobertaLargeV1 { get; } = new PresetBuilder()
        .WithId("all-roberta-large-v1")
        .WithModelUrl("https://huggingface.co/sentence-transformers/all-roberta-large-v1/resolve/main/onnx/model.onnx")
        .WithChecksum("6f2d93448ed45b05ed5af01d70a39c478b7bbbfd85da8c29e9f5257aad8fce81")
        .WithTokenTypeIds(false)
        .WithOutputTensorName("last_hidden_state")
        .WithTokenizerFiles([
            "https://huggingface.co/sentence-transformers/all-roberta-large-v1/resolve/main/config.json",
            "https://huggingface.co/sentence-transformers/all-roberta-large-v1/resolve/main/special_tokens_map.json",
            "https://huggingface.co/sentence-transformers/all-roberta-large-v1/resolve/main/tokenizer_config.json",
            "https://huggingface.co/sentence-transformers/all-roberta-large-v1/resolve/main/merges.txt",
            "https://huggingface.co/sentence-transformers/all-roberta-large-v1/resolve/main/vocab.json"
        ])
        .WithMaxTokens(512)
        .WithPoolingStrategy(PoolingStrategy.Mean)
        .WithTokenizationType(TokenizationType.Bpe)
        .Build();
}