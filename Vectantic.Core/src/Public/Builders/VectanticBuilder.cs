using Microsoft.Extensions.DependencyInjection;
using Vectantic.Core.Configuration;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Interfaces;
using Vectantic.Core.Internal.Onnx;
using Vectantic.Core.Models;

namespace Vectantic.Core.Builders;

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
    public async Task EnsureModelsAsync(IProgress<float>? progress = null, CancellationToken ct = default) {
        // DOWNLOAD
        var downloadResult = await DownloadWithRetries(progress, ct);

        _onComplete(downloadResult);
        _services.AddSingleton<IOnnxSession>(_ => new OnnxSession(downloadResult.ModelPath));
    }

    // -------------------- INNER METHS --------------------
    private async Task<DownloadResult> DownloadWithRetries(IProgress<float>? progress, CancellationToken ct) {
        int attempts = 0;
        while (true) {
            try {
                return await _modelDownloader.DownloadModelAsync(_modelInfo, _opts, progress, ct);
            }
            catch (VectanticDownloadException ex) {
                attempts++;
                if (attempts >= _opts.MaxRetries)
                    throw new VectanticDownloadException($"Model download failed after {attempts} attempts. {ex.Message}", ex);

                await Task.Delay(TimeSpan.FromSeconds(attempts * 2), ct);
            }
        }
    }
}