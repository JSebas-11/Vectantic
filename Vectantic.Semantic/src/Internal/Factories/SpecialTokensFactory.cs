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
    internal static SpecialTokensIds IdsFromJson(JsonDocument json, SpecialTokens tokens) {
        var root = json.RootElement;
        
        return new SpecialTokensIds(
            root.GetInt(tokens.UnkToken),
            root.GetInt(tokens.SepToken),
            root.GetInt(tokens.PadToken),
            root.GetInt(tokens.MaskToken),
            root.GetInt(tokens.ClsToken),
            root.GetInt(tokens.BosToken),
            root.GetInt(tokens.EosToken)
        );
    }
    internal static SpecialTokensIds IdsFromJson(JsonDocument json) {
        var root = json.RootElement;
        
        return new SpecialTokensIds(
            root.GetInt(SemanticConstants.UnkTokenId),
            root.GetInt(SemanticConstants.SepTokenId),
            root.GetInt(SemanticConstants.PadTokenId),
            root.GetInt(SemanticConstants.MaskTokenId),
            root.GetInt(SemanticConstants.ClsTokenId),
            root.GetInt(SemanticConstants.BosTokenId),
            root.GetInt(SemanticConstants.EosTokenId)
        );
    }
    internal static Dictionary<string, int> SpecialTokensDict(SpecialTokens? tokens, SpecialTokensIds? ids) {
        if (tokens is null || ids is null)
            throw new VectanticInvalidConstructionException("Both SpecialTokens and SpecialTokensIds must be provided");

        var dict = new Dictionary<string, int> (7);
        
        dict.AddIfNotNull(tokens.ClsToken, ids.ClsToken);
        dict.AddIfNotNull(tokens.MaskToken, ids.MaskToken);
        dict.AddIfNotNull(tokens.PadToken, ids.PadToken);
        dict.AddIfNotNull(tokens.SepToken, ids.SepToken);
        dict.AddIfNotNull(tokens.UnkToken, ids.UnkToken);
        dict.AddIfNotNull(tokens.BosToken, ids.BosToken);
        dict.AddIfNotNull(tokens.EosToken, ids.EosToken);
        
        return dict;
    }
    internal static SpecialTokens FromTokenizationDefault(TokenizationType tokenization)
        => tokenization switch {
            TokenizationType.WordPiece or TokenizationType.Bert 
                => new SpecialTokens("[UNK]", "[SEP]", "[PAD]", "[MASK]", "[CLS]", null, null),
            TokenizationType.Bpe 
                => new SpecialTokens("<unk>", "</s>", "<pad>", "<mask>", "<s>", "<s>", "</s>"),
            _ => throw new VectanticInvalidConstructionException("Special tokens missing (TokenizationType is not compatible)")
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

    private static void AddIfNotNull(this Dictionary<string, int> dict, string? key, int? value) {
        if (!string.IsNullOrWhiteSpace(key) && value is not null)
            dict.Add(key, (int)value);
    }
}