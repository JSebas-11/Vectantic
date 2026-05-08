using Vectantic.Core.Configuration;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Models;

namespace Vectantic.Core.Interfaces;

/// <summary>
/// Provides model and resource download operations for Vectantic runtime initialization.
/// </summary>
/// <remarks>
/// Implementations are responsible for downloading ONNX models and auxiliary resources,
/// validating integrity constraints, and resolving local filesystem paths.
/// This interface serves as the extensibility point for new SDK modules, custom download strategies,
/// storage providers, or caching mechanisms.
/// </remarks>
public interface IModelDownloader {

    /// <summary>
    /// Downloads the configured ONNX model and any additional runtime resources.
    /// </summary>
    /// <param name="info">
    /// The model metadata describing the resources to download.
    /// </param>
    /// <param name="opts">
    /// The runtime configuration controlling download behavior and cache resolution.
    /// </param>
    /// <param name="progress">
    /// An optional progress reporter that receives download progress values between 0 and 1.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to cancel the download operation.
    /// </param>
    /// <returns>
    /// A task containing the resolved local paths of the downloaded resources.
    /// </returns>
    /// <exception cref="VectanticDownloadException">
    /// Thrown when model downloading, resource validation, or integrity verification fails.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled through the provided cancellation token.
    /// </exception>
    /// <remarks>
    /// Implementations should ensure downloaded resources are fully validated
    /// before returning a successful result.
    /// </remarks>
    Task<DownloadResult> DownloadModelAsync(
        VectanticModelInfo info, 
        VectanticOptions opts, 
        IProgress<float>? progress,
        CancellationToken ct = default
    );
}