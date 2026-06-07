using Vectantic.Semantic.Configuration;

namespace Vectantic.Semantic.Enums;

/// <summary>
/// Defines the tokenization algorithm used to convert text into model input tokens.
/// </summary>
/// <remarks>
/// The tokenization type must match the tokenizer files and ONNX model architecture
/// configured in the associated <see cref="VectanticPreset"/>.
///
/// Using an incompatible tokenization strategy may produce incorrect embeddings
/// or cause inference failures.
/// </remarks>
public enum TokenizationType {

    /// <summary>
    /// Uses the WordPiece tokenization algorithm.
    /// </summary>
    /// <remarks>
    /// This value is maintained for backward compatibility with v1.x and is equivalent
    /// to <see cref="Bert"/>.
    /// </remarks>
    [Obsolete("Use Bert instead. WordPiece will be removed in v2.0.")]
    WordPiece,

    /// <summary>
    /// Uses BERT-style WordPiece tokenization.
    /// </summary>
    /// <remarks>
    /// Intended for BERT-derived models such as MiniLM and BGE.
    /// Tokenizer resources typically include a <c>vocab.txt</c> file.
    /// </remarks>
    Bert,

    /// <summary>
    /// Uses Byte Pair Encoding (BPE) tokenization.
    /// </summary>
    /// <remarks>
    /// Intended for RoBERTa-derived models and other architectures based on BPE.
    /// Tokenizer resources typically include <c>vocab.json</c> and <c>merges.txt</c>.
    /// </remarks>
    Bpe
}