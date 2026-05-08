using Microsoft.Extensions.DependencyInjection;
using Vectantic.Core.Configuration;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Interfaces;
using Vectantic.Core.Internal.Onnx;
using Vectantic.Core.Models;

namespace Vectantic.Core.Builders;

/// <summary>
/// Coordinates model downloading, validation, and runtime registration for Vectantic services.
/// </summary>
/// <remarks>
/// This builder finalizes the initialization pipeline after dependency injection registration.
/// It downloads the configured model resources, validates model integrity,
/// and registers the ONNX inference session into the service container.
/// </remarks>
/// <example>
/// <code>
/// await services
///     .AddVectanticSemantic(
///         vecOpts => {},
///         semOpts => {},
///         VectanticPreset.MiniLML6V2)
///     .EnsureModelAsync();
/// </code>
/// </example>
public sealed class VectanticBuilder {
    // -------------------- INIT --------------------
    private readonly IServiceCollection _services;
    private readonly IModelDownloader _modelDownloader;
    private readonly VectanticModelInfo _modelInfo;
    private readonly VectanticOptions _opts;
    private readonly Action<DownloadResult> _onComplete;

    internal VectanticBuilder(
        IServiceCollection services, IModelDownloader modelDownloader,
        VectanticModelInfo modelInfo, VectanticOptions opts,
        Action<DownloadResult> onComplete) 
    {
        _services = services;    
        _modelDownloader = modelDownloader;    
        _modelInfo = modelInfo;    
        _opts = opts;    
        _onComplete = onComplete;
    }

    // -------------------- METHS --------------------

    /// <summary>
    /// Downloads the configured model resources and registers the ONNX inference session.
    /// </summary>
    /// <param name="progress">
    /// An optional progress reporter that receives download progress values between 0 and 1.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to cancel the download operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous initialization operation.
    /// </returns>
    /// <exception cref="VectanticDownloadException">
    /// Thrown when the model download fails after all configured retry attempts are exhausted.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled through the provided cancellation token.
    /// </exception>
    /// <remarks>
    /// This method performs model downloading with retry semantics using the configured
    /// <see cref="VectanticOptions.MaxRetries"/> value. After a successful download,
    /// the ONNX runtime session is registered as a singleton service.
    /// </remarks>
    public async Task EnsureModelAsync(IProgress<float>? progress = null, CancellationToken ct = default) {
        // DOWNLOAD
        var downloadResult = await DownloadWithRetries(progress, ct).ConfigureAwait(false);

        _onComplete(downloadResult);
        _services.AddSingleton<IOnnxSession>(_ => new OnnxSession(downloadResult.ModelPath));
    }

    // -------------------- INNER METHS --------------------
    private async Task<DownloadResult> DownloadWithRetries(IProgress<float>? progress, CancellationToken ct) {
        int attempts = 0;
        while (true) {
            try {
                return await _modelDownloader.DownloadModelAsync(_modelInfo, _opts, progress, ct)
                    .ConfigureAwait(false);
            }
            catch (VectanticDownloadException ex) {
                attempts++;
                if (attempts >= _opts.MaxRetries)
                    throw new VectanticDownloadException($"Model download failed after {attempts} attempts. {ex.Message}", ex);

                await Task.Delay(TimeSpan.FromSeconds(attempts * 2), ct).ConfigureAwait(false);
            }
        }
    }
}