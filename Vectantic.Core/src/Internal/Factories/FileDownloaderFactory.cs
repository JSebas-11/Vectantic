using Vectantic.Core.Configuration;
using Vectantic.Core.Internal.Download;

namespace Vectantic.Core.Internal.Factories;

internal static class FileDownloaderFactory {
    internal static IFileDownloader Create(VectanticOptions opts) {
        var httpClient = new HttpClient() { Timeout = opts.DownloadTimeout };

        if (!string.IsNullOrWhiteSpace(opts.AccessToken))
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "Authorization", $"Bearer {opts.AccessToken}"
            );

        return new FileDownloader(httpClient);
    }
}