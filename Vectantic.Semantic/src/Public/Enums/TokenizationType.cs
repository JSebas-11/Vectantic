using Vectantic.Semantic.Configuration;

namespace Vectantic.Semantic.Enums;

/// <summary>
/// Defines the tokenization algorithm used to convert text into model input tokens.
/// </summary>
/// <remarks>
/// Tokenization types must match the tokenizer resources and ONNX model architecture
/// configured in the associated <see cref="VectanticPreset"/>.
/// </remarks>
public enum TokenizationType { 
    
    /// <summary>
    /// Uses the WordPiece tokenization algorithm for subword segmentation.
    /// </summary>
    /// <remarks>
    /// WordPiece splits text into subword units using a fixed vocabulary and is commonly
    /// used by BERT-based transformer architectures.
    /// </remarks>
    WordPiece 
}