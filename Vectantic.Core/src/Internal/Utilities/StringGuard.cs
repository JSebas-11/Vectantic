using Vectantic.Core.Exceptions;

namespace Vectantic.Core.Internal.Utilities;

internal static class StringGuard {
    internal static void RequireOrException(
        string? str, string name, string? msg = "must be provided.") 
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new VectanticInvalidConstructionException($"{name} {msg}");
    }
}