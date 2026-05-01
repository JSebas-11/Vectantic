using Vectantic.Core.Models;
using Vectantic.Semantic.Enums;

namespace Vectantic.Semantic.Internal.Models;

internal sealed class ResolvedSemanticModel : ResolvedModel {
    public bool LowerCase { get; }
    public string OutputTensorName { get; }
    public string TokenizerPath { get; }
    public PoolingStrategy Pooling { get; }
    public TokenizationType Tokenization { get; }

    internal ResolvedSemanticModel(
        string modelPath,
        int? maxTokens,
        bool lowercase,
        string outputTensorName,
        string tokenizerPath,
        PoolingStrategy pooling,
        TokenizationType tokenization)
        : base(modelPath, maxTokens)
    {
        LowerCase = lowercase;
        OutputTensorName = outputTensorName;
        TokenizerPath = tokenizerPath;
        Pooling = pooling;
        Tokenization = tokenization;
    }
}