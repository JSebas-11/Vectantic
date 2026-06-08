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
            var modelPath = Path.Combine(rootDir, Path.GetFileName(info.ModelUrl.AbsolutePath));

            // DOWNLOADS & CHECKSUMS
            var totalDownloads = info.ExtraFiles.Count + 1;
            var downloads = new List<Task>(totalDownloads) {
                DownloadIfNotCached(info.ModelUrl, modelPath, progress, ct)
            };
            var checksums = new List<Task>(totalDownloads) {
                VerifyChecksum(modelPath, info.Checksum)
            };

            foreach (var extraFile in info.ExtraFiles) {
                var filePath = Path.Combine(rootDir, extraFile.InternalDirectory, extraFile.FileName);
                downloads.Add(DownloadIfNotCached(extraFile.DownloadUrl, filePath, null, ct));

                if (extraFile.RequireChecksum)
                    checksums.Add(VerifyChecksum(filePath, extraFile.Checksum!));
            }
            await Task.WhenAll(downloads).ConfigureAwait(false); 
            await Task.WhenAll(checksums).ConfigureAwait(false);

            // TokenizerPath hardcoded as there is compatibility for tokenizer and model files for now
            return new DownloadResult(
                modelPath, new Dictionary<string, string> 
                { [SemanticConstants.TokenizerDirKey] = Path.Combine(rootDir, SemanticConstants.TokenizerDirKey) }
            );
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
    private async Task VerifyChecksum(string path, string checksum) {
        if (!await _fileDownloader.VerifyChecksumAsync(path, checksum).ConfigureAwait(false))
            throw new VectanticModelException($"File ({path}) was downloaded, however its checksum verification failed.");
    }
}