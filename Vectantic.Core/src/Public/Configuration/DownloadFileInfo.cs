namespace Vectantic.Core.Configuration;

/// <summary>
/// Represents an additional file that must be downloaded and cached
/// as part of a model configuration.
/// </summary>
/// <remarks>
/// Instances of this type are typically used to describe tokenizer resources,
/// configuration files, vocabulary files, merge rules, or other assets required
/// by a model at runtime.
/// </remarks>
public sealed class DownloadFileInfo {

    /// <summary>
    /// Gets the file name used when storing the downloaded file locally.
    /// </summary>
    /// <remarks>
    /// This value determines the final file name within the model cache directory
    /// and does not need to match the source file name contained in the download URL.
    /// </remarks>
    public string FileName { get; }

    /// <summary>
    /// Gets the remote location from which the file will be downloaded.
    /// </summary>
    /// <remarks>
    /// The referenced resource is downloaded during model initialization if it is
    /// not already available in the local cache.
    /// </remarks>
    public Uri DownloadUrl { get; }

    /// <summary>
    /// Gets the relative directory within the model cache where the file is stored.
    /// </summary>
    /// <value>
    /// Defaults to <c>"/"</c>, indicating the model's root cache directory.
    /// </value>
    /// <remarks>
    /// This value allows related files to be organized into subdirectories while
    /// preserving the structure expected by the model runtime.
    /// </remarks>
    public string InternalDirectory { get; } = "/";

    /// <summary>
    /// Gets the optional SHA-256 checksum used to validate the downloaded file.
    /// </summary>
    /// <value>
    /// <see langword="null"/> by default.
    /// </value>
    /// <remarks>
    /// When specified, the downloaded file is validated before being used.
    /// Providing a checksum is recommended for files that are critical to
    /// model execution or tokenization.
    /// </remarks>
    public string? Checksum { get; } = null;

    internal bool RequireChecksum => !string.IsNullOrWhiteSpace(Checksum);

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadFileInfo"/> class.
    /// </summary>
    /// <param name="fileName">
    /// The file name used when storing the downloaded file locally.
    /// </param>
    /// <param name="downloadUrl">
    /// The remote URI from which the file will be downloaded.
    /// </param>
    /// <param name="internalDirectory">
    /// The relative directory within the model cache where the file will be stored.
    /// Defaults to the model root directory.
    /// </param>
    /// <param name="checksum">
    /// An optional SHA-256 checksum used to validate the downloaded file.
    /// </param>
    public DownloadFileInfo(
        string fileName, Uri downloadUrl,
        string internalDirectory = "/", string? checksum = null) 
    {
        FileName = fileName;
        DownloadUrl = downloadUrl;
        InternalDirectory = internalDirectory;
        Checksum = checksum;
    }
}