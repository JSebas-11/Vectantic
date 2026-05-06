using System.Security.Cryptography;
using Vectantic.Core.Exceptions;

namespace Vectantic.Core.Internal.Download;

internal sealed class FileDownloader : IFileDownloader {
    // -------------------- INIT --------------------
    private const int BufferSize = 81920;
    private readonly HttpClient _httpClient;

    internal FileDownloader(HttpClient httpClient) => _httpClient = httpClient;
        
    // -------------------- METHS --------------------
    public async Task DownloadFileAsync(
        Uri source, string destinationPath, IProgress<float>? progress, CancellationToken ct = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

        var success = false;
        var tempPath = destinationPath + ".tmp";
        try {
            using var response = await _httpClient.GetAsync(source, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new VectanticDownloadException($"Failed to download {source}. Status: {response.StatusCode}");
            
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;

            using var contentStream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            using var fileStream = new FileStream(
                tempPath, FileMode.Create, FileAccess.Write,
                FileShare.None, BufferSize, FileOptions.Asynchronous
            );

            await FillAndFlushFileAsync(contentStream, fileStream, totalBytes, progress, ct).ConfigureAwait(false);
            success = true;
        }
        catch (VectanticDownloadException) { throw; }
        catch (Exception ex) { 
            throw new VectanticDownloadException($"Unexpected error downloading {source}. {ex.Message}", ex);
        }
        finally { 
            if (!success && File.Exists(tempPath)) File.Delete(tempPath);
        }
        
        File.Move(tempPath, destinationPath, overwrite: true);
    }

    public bool IsCached(string filePath) => File.Exists(filePath);
    public async Task<bool> VerifyChecksumAsync(string filePath, string expectedChecksum) {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream).ConfigureAwait(false);

        return string.Equals(BitConverter.ToString(hash).Replace("-", ""), expectedChecksum, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------- INNER METHS --------------------
    private static async Task FillAndFlushFileAsync(
        Stream stream, FileStream fileStream, long totalBytes, IProgress<float>? progress, CancellationToken ct) 
    {
        var buffer = new byte[BufferSize];
        var bytesDownloaded = 0L;
        int bytesRead;
        
        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(), ct).ConfigureAwait(false)) > 0) {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct)
                .ConfigureAwait(false);

            bytesDownloaded += bytesRead;

            if (totalBytes > 0)
                progress?.Report(bytesDownloaded / (float)totalBytes);
        }

        await fileStream.FlushAsync(ct).ConfigureAwait(false);
    }
}