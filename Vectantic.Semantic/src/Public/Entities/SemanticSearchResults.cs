namespace Vectantic.Semantic.Entities;

public class SemanticSearchResults {
    public IReadOnlyList<SemanticMatch> Matches { get; internal init; } = [];
    public int TotalCandidates { get; internal init; }
    public bool IsEmpty => Matches.Count == 0;

    public SemanticMatch? Best => Matches.Count > 0 ? Matches[0] : null;
}