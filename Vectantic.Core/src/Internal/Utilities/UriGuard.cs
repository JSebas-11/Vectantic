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
        
        var listCount = urisList.Count;
        if (listCount == 0)
            throw new VectanticInvalidConstructionException($"{name} cannot be empty.");

        var parsedList = new List<Uri>(listCount);
        foreach (var uri in urisList) {
            ValidateOrException(uri, name);
            parsedList.Add(new Uri(uri));
        }

        return parsedList;
    }

    // -------------------- URI-DICT --------------------
    internal static Dictionary<Uri, string> CreateDictOrException(IDictionary<string, string> dict, string name) {
        var urisDict = dict?.ToDictionary()
            ?? throw new VectanticInvalidConstructionException($"{name} cannot be null.");

        var dictCount = urisDict.Count;
        if (dictCount == 0)
            throw new VectanticInvalidConstructionException($"{name} cannot be empty.");

        var parsedDict = new Dictionary<Uri, string>(dictCount);
        foreach (var item in urisDict) {
            ValidateOrException(item.Key, "Dictionary value (Uri)");
            StringGuard.RequireOrException(item.Value, "Dictionary value (string)");

            parsedDict.Add(new Uri(item.Key), item.Value);
        }

        return parsedDict;
    }

    internal static Dictionary<Uri, string> CreateDictOrException(IDictionary<Uri, string> dict, string name) {
        var urisDict = dict?.ToDictionary()
            ?? throw new VectanticInvalidConstructionException($"{name} cannot be null.");

        if (urisDict.Count == 0)
            throw new VectanticInvalidConstructionException($"{name} cannot be empty.");

        foreach (var item in urisDict) {
            ValidateOrException(item.Key, "Dictionary value (Uri)");
            StringGuard.RequireOrException(item.Value, "Dictionary value (string)");
        }

        return urisDict;
    }
}