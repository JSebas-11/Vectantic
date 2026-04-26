using Vectantic.Core.Configuration;
using Vectantic.Core.Models;

namespace Vectantic.Core.Interfaces;

public interface IModelDownloader {
    Task<DownloadResult> DownloadModelAsync(
        VectanticModelInfo info, 
        VectanticOptions opts, 
        IProgress<float>? progress,
        CancellationToken ct = default
    );
}