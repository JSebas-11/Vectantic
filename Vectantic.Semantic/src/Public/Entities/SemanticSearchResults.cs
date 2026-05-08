namespace Vectantic.Semantic.Entities;

/// <summary>
/// Represents the results of a semantic similarity search operation.
/// </summary>
/// <remarks>
/// Search results contain the highest-scoring semantic matches ordered by similarity score.
/// </remarks>
public class SemanticSearchResults {
    
    /// <summary>
    /// Gets the semantic matches returned by the search operation.
    /// </summary>
    /// <remarks>
    /// Matches are ordered from highest to lowest similarity score.
    /// </remarks>
    public IReadOnlyList<SemanticMatch> Matches { get; internal init; } = [];
    
    /// <summary>
    /// Gets the total number of candidate items evaluated during the search operation.
    /// </summary>
    public int TotalCandidates { get; internal init; }
    
    /// <summary>
    /// Gets a value indicating whether the search returned no matches.
    /// </summary>
    public bool IsEmpty => Matches.Count == 0;

    /// <summary>
    /// Gets the highest-scoring semantic match, if available.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="null"/> when the result set is empty.
    /// </remarks>
    public SemanticMatch? Best => Matches.Count > 0 ? Matches[0] : null;
}