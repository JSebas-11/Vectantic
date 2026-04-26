using Microsoft.Extensions.DependencyInjection;
using Vectantic.Core.Configuration;
using Vectantic.Core.Internal.Download;

namespace Vectantic.Core.Internal.Extensions;

internal static class CoreServiceExtensions {
    internal static IServiceCollection AddVectanticCore(
        this IServiceCollection services, VectanticOptions opts)
    {
        // CONFIGURATION
        services.AddSingleton(opts);

        // DOWNLOAD
        services.AddHttpClient<IFileDownloader, FileDownloader>(client => {
            client.Timeout = opts.DownloadTimeout;

            if (!string.IsNullOrWhiteSpace(opts.AccessToken))
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {opts.AccessToken}");
        });

        return services;
    }
}