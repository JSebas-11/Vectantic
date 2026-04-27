namespace Vectantic.Semantic.Entities;

public class Embedding {
    public float[] Vector { get; internal init; } = [];
    public string? Context { get; internal init; }
    public int Dimensions => Vector.Length;

    public Span<float> AsSpan() => Vector;
}