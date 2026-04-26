namespace Vectantic.Core.Models;

public record DownloadResult(
    string ModelPath,
    IReadOnlyDictionary<string, string> AdditionalPaths
);