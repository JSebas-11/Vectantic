using Microsoft.Extensions.DependencyInjection;
using Vectantic.Core.Configuration;
using Vectantic.Core.Exceptions;

namespace Vectantic.Core.Internal.Extensions;

internal static class CoreServiceExtensions {
    internal static IServiceCollection AddVectantic(
        this IServiceCollection services, VectanticOptions opts)
    {
        // CONFIGURATION / VALIDATION
        if (opts.UseGpu) 
            throw new VectanticException("GPU acceleration is not supported in V1. Set UseGpu = false.");
        if (string.IsNullOrWhiteSpace(opts.CacheDirectory)) 
            throw new VectanticException("Cache directory must be provided.");
        if (services.Any(s => s.ServiceType == typeof(VectanticCoreMarker)))
            throw new VectanticException (
                "Vectantic Core is already registered. Only one model is allowed in V1.");

        // INJECTION
        services.AddSingleton(opts);
        services.AddSingleton<VectanticCoreMarker>();

        return services;
    }
}