namespace Vectantic.Semantic.Entities;

public class SemanticMatch {
    public string Text { get; internal init; } = string.Empty;
    public float Score { get; internal init; }
}