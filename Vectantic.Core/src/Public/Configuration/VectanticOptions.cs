namespace Vectantic.Core.Configuration;

/// <summary>
/// Provides runtime configuration options for model downloading, caching, and ONNX execution.
/// </summary>
/// <remarks>
/// This type configures the infrastructure behavior of Vectantic.Core, including
/// authentication, cache management, download policies, and hardware acceleration.
/// These settings are typically registered once during application startup.
/// </remarks>
/// <example>
/// <code>
/// var options = new VectanticOptions {
///     AccessToken = "hf_xxxxx",
///     CacheDirectory = "./models",
///     DownloadTimeout = TimeSpan.FromMinutes(10),
///     MaxRetries = 3,
///     UseGpu = false
/// };
/// </code>
/// </example>
public sealed class VectanticOptions {
    private static readonly string DefaultCacheDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Vectantic", "models"
    );

    /// <summary>
    /// Gets or sets the HuggingFace access token used for authenticated model downloads.
    /// </summary>
    /// <remarks>
    /// This value is required when downloading gated or private models from HuggingFace.
    /// Public models can typically be downloaded without authentication.
    /// </remarks>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the local directory used to cache downloaded model files.
    /// </summary>
    /// <remarks>
    /// Downloaded ONNX models and auxiliary assets are persisted in this directory
    /// to avoid repeated network downloads across application runs.
    /// </remarks>
    public string CacheDirectory { get; set; } = DefaultCacheDirectory;
    
    /// <summary>
    /// Gets or sets the maximum duration allowed for model download operations.
    /// </summary>
    /// <remarks>
    /// Applies to individual HTTP download requests performed by the model downloader.
    /// </remarks>
    public TimeSpan DownloadTimeout { get; set; } = TimeSpan.FromMinutes(5);
    
    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed download operations.
    /// </summary>
    /// <remarks>
    /// Retries are performed for transient network failures and unsuccessful HTTP responses.
    /// </remarks>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Gets or sets a value indicating whether GPU acceleration should be enabled for ONNX inference.
    /// </summary>
    /// <remarks>
    /// WARNING: GPU acceleration is not available in v1.
    /// </remarks>
    /// <remarks>
    /// When enabled, the runtime attempts to initialize a GPU execution provider if supported
    /// by the current environment and ONNX Runtime installation.
    /// </remarks>
    public bool UseGpu { get; set; } = false;
}