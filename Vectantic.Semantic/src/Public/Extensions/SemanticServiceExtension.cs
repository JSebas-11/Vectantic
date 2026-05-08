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

/// <summary>
/// Provides dependency injection extensions for registering semantic embedding services and model infrastructure.
/// </summary>
/// <remarks>
/// This extension registers all services required for tokenization, pooling,
/// embedding generation, semantic search, and ONNX inference execution.
/// The returned <see cref="VectanticBuilder"/> must be finalized by calling
/// <see cref="VectanticBuilder.EnsureModelAsync(IProgress{float}?, CancellationToken)"/>
/// to download and register the model runtime.
/// </remarks>
public static class SemanticServiceExtension {

    /// <summary>
    /// Registers semantic embedding services, tokenization components, pooling strategies,
    /// and model infrastructure into the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The service collection used to register Vectantic services.
    /// </param>
    /// <param name="coreOpts">
    /// The core runtime configuration used for downloading, caching, and ONNX execution.
    /// </param>
    /// <param name="semanticOpts">
    /// The semantic runtime configuration used for embedding post-processing behavior.
    /// </param>
    /// <param name="preset">
    /// The semantic model preset describing tokenizer resources, pooling strategy,
    /// and ONNX model metadata.
    /// </param>
    /// <returns>
    /// A <see cref="VectanticBuilder"/> used to finalize model initialization and download.
    /// </returns>
    /// <exception cref="VectanticException">
    /// Thrown when GPU acceleration is enabled, as GPU execution providers are not supported in V1.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when <see cref="VectanticOptions.CacheDirectory"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when Vectantic Core has already been registered in the current service collection.
    /// Only a single model registration is supported in V1.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when either the provided tokenization type or the provided pooling strategy is not supported.
    /// </exception>
    /// <remarks>
    /// This method only registers services and configuration metadata.
    /// Model files are not downloaded until <see cref="VectanticBuilder.EnsureModelAsync(System.IProgress{float}?, System.Threading.CancellationToken)"/>
    /// is executed.
    /// </remarks>
    /// <example>
    /// <code>
    /// await services
    ///     .AddVectanticSemantic(
    ///         new VectanticOptions(),
    ///         new SemanticOptions(),
    ///         VectanticPreset.MiniLML6V2)
    ///     .EnsureModelAsync();
    /// </code>
    /// </example>
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