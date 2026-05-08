namespace Vectantic.Semantic.Enums;

/// <summary>
/// Defines the strategy used to aggregate token-level embeddings into a single semantic vector.
/// </summary>
/// <remarks>
/// Pooling strategies determine how transformer output tensors are reduced into
/// fixed-size embeddings suitable for semantic similarity and retrieval operations.
/// </remarks>
public enum PoolingStrategy {
    
    /// <summary>
    /// Computes the embedding by averaging all token embeddings using the attention mask.
    /// </summary>
    /// <remarks>
    /// Mean pooling generally produces stable semantic representations and is the
    /// recommended strategy for most sentence-transformer models.
    /// </remarks>
    Mean,
    
    /// <summary>
    /// Uses the embedding of the CLS token as the final semantic representation.
    /// </summary>
    /// <remarks>
    /// CLS pooling relies on the model encoding semantic context into the first token position.
    /// This strategy is commonly used by classification-oriented transformer architectures.
    /// </remarks>
    Cls
}