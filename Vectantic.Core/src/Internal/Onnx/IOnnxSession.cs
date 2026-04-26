using Microsoft.ML.OnnxRuntime;

namespace Vectantic.Core.Internal.Onnx;

internal interface IOnnxSession : IDisposable {
    Task<IDisposableReadOnlyCollection<DisposableNamedOnnxValue>> RunAsync(
        IReadOnlyCollection<NamedOnnxValue> inputs, 
        CancellationToken ct = default
    );
}