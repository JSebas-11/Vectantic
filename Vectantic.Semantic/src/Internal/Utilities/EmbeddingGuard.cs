using Vectantic.Core.Exceptions;

namespace Vectantic.Semantic.Internal.Utilities;

internal static class EmbeddingGuard {
    internal static string ValidateAndNormalizeText(string text) {
        var trimmed = text.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new VectanticException("Text to convert into embedding must not be null.");

        return trimmed;
    }
    
    internal static IReadOnlyList<string> ValidateAndNormalizeTexts(IReadOnlyList<string> texts, int count) {
        if (count == 0)
            throw new VectanticException("Text list must not be empty.");

        var result = new List<string>(count);

        foreach (var txt in texts) {
            var newTxt = txt.Trim();
            if (string.IsNullOrWhiteSpace(newTxt))
                throw new VectanticException("Text to convert into embedding must not be null.");
            
            result.Add(newTxt);
        }

        return result.AsReadOnly();
    }
}