namespace Vectantic.Core.Configuration;

public class VectanticModelInfo {
    public string Id { get; }
    public Uri ModelUrl { get; }
    public string Checksum { get; }

    protected VectanticModelInfo(string id, Uri modelUrl, string checksum) {
        Id = id;
        ModelUrl = modelUrl;
        Checksum = checksum;
    }
}