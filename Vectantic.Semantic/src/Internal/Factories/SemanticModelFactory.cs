using Vectantic.Core.Exceptions;
using Vectantic.Core.Models;
using Vectantic.Semantic.Configuration;
using Vectantic.Semantic.Internal.Constants;
using Vectantic.Semantic.Internal.Models;

namespace Vectantic.Semantic.Internal.Factories;

internal static class SemanticModelFactory {
    internal static ResolvedSemanticModel Create(
        DownloadResult downloadResult, VectanticPreset preset)
    {
        if (!downloadResult.AdditionalPaths.TryGetValue(SemanticConstants.TokenizerDirKey, out string? tokenizerPath))
            throw new VectanticModelException($"Tokenizer directory path missing for model ({preset.Id})");

        return new(
            downloadResult.ModelPath,
            preset.MaxTokens,
            preset.LowerCase,
            preset.OutputTensorName,
            tokenizerPath,
            preset.Pooling,
            preset.Tokenization,
            preset.RequiresTokenTypeIds
        );
    }
}