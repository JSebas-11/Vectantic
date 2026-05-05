namespace Vectantic.Core.Configuration;

public sealed class VectanticOptions {
    private static readonly string DefaultCacheDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Vectantic", "models"
    );

    public string? AccessToken { get; set; }
    public string CacheDirectory { get; set; } = DefaultCacheDirectory;
    public TimeSpan DownloadTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxRetries { get; set; } = 3;
    public bool UseGpu { get; set; } = false;
}