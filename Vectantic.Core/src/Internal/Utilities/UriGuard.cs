using Vectantic.Core.Exceptions;

namespace Vectantic.Core.Internal.Utilities;

internal static class UriGuard {
    // -------------------- URI --------------------
    internal static void ValidateOrException(Uri uri, string name) {
        if (uri is null)
            throw new VectanticInvalidConstructionException($"{name} cannot be null.");

        if (!uri.IsAbsoluteUri)
            throw new VectanticInvalidConstructionException($"{name} must be an absolute URI.");
    }
    internal static void ValidateOrException(string uri, string name) {
        if (string.IsNullOrWhiteSpace(uri))
            throw new VectanticInvalidConstructionException($"{name} must be provided.");

        if (!Uri.TryCreate(uri, UriKind.Absolute, out _))
            throw new VectanticInvalidConstructionException($"{name} must be valid absolute URL.");
    }
    
    // -------------------- URI-COLL --------------------
    internal static List<Uri> CreateListOrException(IEnumerable<Uri> uris, string name) {
        var urisList = uris?.ToList()
            ?? throw new VectanticInvalidConstructionException($"{name} cannot be null.");
        
        if (urisList.Count == 0)
            throw new VectanticInvalidConstructionException($"{name} cannot be empty.");

        foreach (var uri in urisList) ValidateOrException(uri, name);

        return urisList;
    }
    internal static List<Uri> CreateListOrException(IEnumerable<string> uris, string name) {
        var urisList = uris?.ToList()
            ?? throw new VectanticInvalidConstructionException($"{name} cannot be null.");
        
        if (urisList.Count == 0)
            throw new VectanticInvalidConstructionException($"{name} cannot be empty.");

        var parsedList = new List<Uri>(urisList.Count);
        foreach (var uri in urisList) {
            ValidateOrException(uri, name);
            parsedList.Add(new Uri(uri));
        }

        return parsedList;
    }
}