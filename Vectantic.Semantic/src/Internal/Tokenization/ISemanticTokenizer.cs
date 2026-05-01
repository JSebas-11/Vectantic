using Vectantic.Semantic.Internal.Models;

namespace Vectantic.Semantic.Internal.Tokenization;

internal interface ISemanticTokenizer {
    TokenizerOutput Tokenize(string text);
    TokenizerOutput TokenizeBatch(IReadOnlyList<string> texts);
}