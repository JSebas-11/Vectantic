namespace Vectantic.Semantic.Entities;

/// <summary>
/// Represents a semantic embedding vector generated from text input.
/// </summary>
/// <remarks>
/// Embeddings are fixed-size numerical vector representations produced by transformer models.
/// They can be used for similarity search, clustering, ranking, and semantic comparison operations.
/// </remarks>
/// <example>
/// <code>
/// var embedding = await embeddingService.EmbedAsync("hello world");
///
/// float similarity = EmbeddingMath.CosineSimilarity(
///     embedding.Vector,
///     otherEmbedding.Vector);
/// </code>
/// </example>
public class Embedding {
    
    /// <summary>
    /// Gets the embedding vector values.
    /// </summary>
    /// <remarks>
    /// The vector length depends on the dimensions produced by the configured embedding model.
    /// </remarks>
    public float[] Vector { get; internal init; } = [];
    
    /// <summary>
    /// Gets the original text context associated with the embedding.
    /// </summary>
    /// <remarks>
    /// This value may be null when embeddings are generated from raw vectors
    /// or reconstructed from external sources.
    /// </remarks>
    public string? Context { get; internal init; }

    /// <summary>
    /// Gets the number of dimensions contained in the embedding vector.
    /// </summary>
    public int Dimensions => Vector.Length;

    /// <summary>
    /// Returns the embedding vector as a mutable span.
    /// </summary>
    /// <returns>
    /// A span over the underlying embedding vector data.
    /// </returns>
    /// <remarks>
    /// This method exposes the underlying vector memory directly without additional allocations.
    /// </remarks>
    public Span<float> AsSpan() => Vector;
}