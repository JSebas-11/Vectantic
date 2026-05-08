namespace Vectantic.Semantic.Entities;

/// <summary>
/// Represents a semantic similarity match produced by a search operation.
/// </summary>
/// <remarks>
/// A semantic match contains the matched text and its associated similarity score.
/// </remarks>
public class SemanticMatch {
    
    /// <summary>
    /// Gets the matched text associated with the semantic result.
    /// </summary>
    public string? Text { get; internal init; }
    
    /// <summary>
    /// Gets the semantic similarity score of the match.
    /// </summary>
    /// <remarks>
    /// Higher scores indicate stronger semantic similarity relative to the query input.
    /// </remarks>
    public float Score { get; internal init; }
}