using Microsoft.Extensions.DependencyInjection;
using Vectantic.Core.Builders;
using Vectantic.Core.Configuration;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Internal.Extensions;
using Vectantic.Core.Internal.Factories;
using Vectantic.Semantic.Configuration;
using Vectantic.Semantic.Internal.Download;
using Vectantic.Semantic.Internal.Factories;
using Vectantic.Semantic.Internal.Pooling;
using Vectantic.Semantic.Internal.Services;
using Vectantic.Semantic.Internal.Tokenization;
using Vectantic.Semantic.Services;

namespace Vectantic.Semantic.Extensions;

public static class SemanticServiceExtension {
    public static VectanticBuilder AddVectanticSemantic(
        this IServiceCollection services, 
        VectanticOptions coreOpts, SemanticOptions semanticOpts, 
        VectanticPreset preset) 
    {
        // CORE
        services.AddVectantic(coreOpts);

        // CONFIGURATION
        services.AddSingleton(semanticOpts);

        // DOWNLOAD
        var downloader = new SemanticModelDownloader( FileDownloaderFactory.Create(coreOpts) );

        // TOKENIZATION
        switch (preset.Tokenization) {
            case Enums.TokenizationType.WordPiece:
                services.AddSingleton<ISemanticTokenizer, WordPieceTokenizer>();
                break;
            default:
                throw new VectanticException("Tokenization type provided is not supported.");
        }

        // POOLING
        switch (preset.Pooling) {
            case Enums.PoolingStrategy.Cls:
                services.AddSingleton<IPoolingStrategy, ClsPooling>();
                break;
            case Enums.PoolingStrategy.Mean:
                services.AddSingleton<IPoolingStrategy, MeanPooling>();
                break;
            default:
                throw new VectanticException("Pooling strategy provided is not supported.");
        }
        
        // SERVICES
        services.AddSingleton<IEmbeddingService, EmbeddingService>();
        services.AddSingleton<ISemanticSearchService, SemanticSearchService>();

        return new VectanticBuilder(
            services, downloader, preset, coreOpts,
            result => {
                services.AddSingleton(
                    SemanticModelFactory.Create(result, preset)
                );
            }
        );
    }
}