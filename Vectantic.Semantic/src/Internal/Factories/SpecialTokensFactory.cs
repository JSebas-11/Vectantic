using System.Text.Json;
using Vectantic.Core.Exceptions;
using Vectantic.Semantic.Enums;
using Vectantic.Semantic.Internal.Constants;
using Vectantic.Semantic.Internal.Extensions;
using Vectantic.Semantic.Internal.Models;

namespace Vectantic.Semantic.Internal.Factories;

internal static class SpecialTokensFactory {
    // -------------------- METHS --------------------
    internal static SpecialTokens FromJson(JsonDocument json) {
        var root = json.RootElement;
        
        return new SpecialTokens(
            root.TryGetSpecialToken(SemanticConstants.UnkToken),
            root.TryGetSpecialToken(SemanticConstants.SepToken),
            root.TryGetSpecialToken(SemanticConstants.PadToken),
            root.TryGetSpecialToken(SemanticConstants.MaskToken),
            root.TryGetSpecialToken(SemanticConstants.ClsToken),
            root.TryGetSpecialToken(SemanticConstants.BosToken),
            root.TryGetSpecialToken(SemanticConstants.EosToken)
        );
    }
    internal static SpecialTokens FromTokenizationDefault(TokenizationType tokenization)
        => tokenization switch {
            TokenizationType.WordPiece or TokenizationType.Bert 
                => new SpecialTokens("[UNK]", "[SEP]", "[PAD]", "[MASK]", "[CLS]", null, null),
            TokenizationType.Bpe 
                => new SpecialTokens("<unk>", "</s>", "<pad>", "<mask>", "<s>", "<s>", "</s>"),
            _ => throw new VectanticInvalidConstructionException($"Special tokens missing for model")
        };

    // -------------------- INNER METHS --------------------
    private static string? TryGetSpecialToken(this JsonElement element, string prop) {
        var value = element.GetStringValue(prop);

        if (string.IsNullOrWhiteSpace(value)) {
            var tokenObj = element.GetObject(prop);

            if (tokenObj is null) return null;
            
            return tokenObj?.GetStringValue("content");
        }

        return value;
    }
}