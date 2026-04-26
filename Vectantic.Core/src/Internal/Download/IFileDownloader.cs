namespace Vectantic.Core.Internal.Download;

internal interface IFileDownloader {
    Task DownloadFileAsync(
        Uri source, string destinationPath, 
        IProgress<float>? progress, CancellationToken ct = default);
    Task<bool> VerifyChecksumAsync(string filePath, string expectedChecksum);
    bool IsCached(string filePath);
}