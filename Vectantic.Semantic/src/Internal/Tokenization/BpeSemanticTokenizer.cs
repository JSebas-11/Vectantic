using Microsoft.ML.Tokenizers;
using Vectantic.Semantic.Internal.Models;

namespace Vectantic.Semantic.Internal.Tokenization;

internal sealed class BpeSemanticTokenizer : ISemanticTokenizer {
    // -------------------- INIT --------------------
    private readonly ResolvedSemanticModel _semanticModel;
    private readonly BpeTokenizer _tokenizer;

    public BpeSemanticTokenizer(ResolvedSemanticModel semanticModel) {
        _semanticModel = semanticModel;
        var opts = new BpeOptions("", "");
        _tokenizer = BpeTokenizer.Create(opts);
    }

    // -------------------- METHS --------------------
    public TokenizerOutput Tokenize(string text)
    {
        throw new NotImplementedException();
    }

    public TokenizerOutput TokenizeBatch(IReadOnlyList<string> texts)
    {
        throw new NotImplementedException();
    }
}