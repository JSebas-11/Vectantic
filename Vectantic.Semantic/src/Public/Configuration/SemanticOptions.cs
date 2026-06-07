namespace Vectantic.Semantic.Configuration;

/// <summary>
/// Provides runtime configuration options for semantic embedding generation and search operations.
/// </summary>
/// <remarks>
/// These options control post-processing behavior applied to generated embeddings
/// before similarity computations are performed.
///
/// Default values:
/// - Normalize = true
/// </remarks>
public sealed class SemanticOptions {

    /// <summary>
    /// Gets or sets a value indicating whether generated embeddings should be normalized to unit length.
    /// </summary>
    /// <value>
    /// <see langword="true"/> by default.
    /// </value>
    /// <remarks>
    /// Normalization is recommended when using cosine similarity, as it ensures
    /// consistent vector magnitudes across embedding comparisons and enables
    /// efficient dot-product similarity calculations.
    /// </remarks>
    public bool Normalize { get; set; } = true;
}