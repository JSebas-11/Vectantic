using System.Text.Json;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Models;
using Vectantic.Semantic.Configuration;
using Vectantic.Semantic.Internal.Constants;
using Vectantic.Semantic.Internal.Models;

namespace Vectantic.Semantic.Internal.Factories;

internal static class SemanticModelFactory {
    // -------------------- METHS --------------------
    internal static ResolvedSemanticModel Create(
        DownloadResult downloadResult, VectanticPreset preset)
    {
        if (!downloadResult.AdditionalPaths.TryGetValue(SemanticConstants.TokenizerDirKey, out string? tokenizerPath))
            throw new VectanticModelException($"Tokenizer directory path missing for model ({preset.Id})");

        var tokensMapsPath = Path.Combine(tokenizerPath, "special_tokens_map.json");
        
        var specialTokens = File.Exists(tokensMapsPath)
            ? ParseSpecialTokens(tokensMapsPath)
            : SpecialTokensFactory.FromTokenizationDefault(preset.Tokenization);

        return new(
            downloadResult.ModelPath,
            preset.MaxTokens,
            preset.LowerCase,
            preset.OutputTensorName,
            tokenizerPath,
            preset.Pooling,
            preset.Tokenization,
            specialTokens,
            preset.RequiresTokenTypeIds
        );
    }

    // -------------------- INNER METHS --------------------
    private static SpecialTokens ParseSpecialTokens(string path) {
        using var file = File.OpenRead(path);
        using var json = JsonDocument.Parse(file);
        return SpecialTokensFactory.FromJson(json);
    }
}