using Vectantic.Core.Configuration;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Interfaces;
using Vectantic.Core.Internal.Download;
using Vectantic.Core.Models;
using Vectantic.Semantic.Internal.Constants;

namespace Vectantic.Semantic.Internal.Download;

internal sealed class SemanticModelDownloader : IModelDownloader {
    // -------------------- INIT --------------------
    private readonly IFileDownloader _fileDownloader;

    public SemanticModelDownloader(IFileDownloader fileDownloader)
        => _fileDownloader = fileDownloader;

    // -------------------- METHS --------------------
    public async Task<DownloadResult> DownloadModelAsync(
        VectanticModelInfo info, VectanticOptions opts, 
        IProgress<float>? progress, CancellationToken ct = default)
    {
        try {
            // PATHS
            var rootDir = Path.Combine(opts.CacheDirectory, info.Id);
            var tokenizerDir = Path.Combine(rootDir, SemanticConstants.TokenizerDirKey);
            var modelPath = Path.Combine(rootDir, Path.GetFileName(info.ModelUrl.AbsolutePath));

            // DOWNLOADS
            var totalDownloads = info.ExtraFiles.Count + 1;
            var downloads = new List<Task>(totalDownloads) {
                DownloadIfNotCached(info.ModelUrl, modelPath, progress, ct)
            };

            foreach (var extraFile in info.ExtraFiles) {
                var filePath = Path.Combine(tokenizerDir, extraFile.Key);
                downloads.Add(DownloadIfNotCached(extraFile.Value, filePath, null, ct));
            }
            await Task.WhenAll(downloads).ConfigureAwait(false);

            // CHECKSUM
            if (!await _fileDownloader.VerifyChecksumAsync(modelPath, info.Checksum).ConfigureAwait(false))
                throw new VectanticModelException("Model was downloaded, however its checksum verification failed.");

            return new DownloadResult(
                modelPath, new Dictionary<string, string> { [SemanticConstants.TokenizerDirKey] = tokenizerDir });
        }
        catch (VectanticModelException) { throw; }
        catch (VectanticDownloadException) { throw; }
        catch (Exception ex) {
            throw new VectanticDownloadException($"Error downloading model ({info.Id}): {ex.Message}", ex);
        }
    }

    // -------------------- INNER METHS --------------------
    private async Task DownloadIfNotCached(Uri src, string path, IProgress<float>? progress, CancellationToken ct) {
        if (_fileDownloader.IsCached(path)) return;

        await _fileDownloader.DownloadFileAsync(src, path, progress, ct).ConfigureAwait(false);
    }
}