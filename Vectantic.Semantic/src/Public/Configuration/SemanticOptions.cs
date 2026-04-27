using Vectantic.Core.Configuration;

namespace Vectantic.Semantic.Configuration;

public sealed class SemanticOptions : VectanticOptions {
    public bool Normalize { get; set; } = true;
}