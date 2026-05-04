namespace Vectantic.Core.Configuration;

public class VectanticModelInfo {
    public string Id { get; }
    public Uri ModelUrl { get; }
    public string Checksum { get; }
    public IReadOnlyDictionary<string, Uri> ExtraFiles { get; }

    protected VectanticModelInfo(
        string id, Uri modelUrl, string checksum,
        IReadOnlyDictionary<string, Uri>? extraFiles = null) 
    {
        Id = id;
        ModelUrl = modelUrl;
        Checksum = checksum;
        ExtraFiles = extraFiles ?? new Dictionary<string, Uri>();
    }
}