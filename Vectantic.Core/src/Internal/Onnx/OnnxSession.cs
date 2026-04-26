using Microsoft.ML.OnnxRuntime;
using Vectantic.Core.Exceptions;

namespace Vectantic.Core.Internal.Onnx;

internal sealed class OnnxSession : IOnnxSession {
    // -------------------- INIT --------------------
    private readonly InferenceSession _session;

    internal OnnxSession(string modelPath) {
        try { _session = new InferenceSession(modelPath); }
        catch (Exception ex) {
            throw new VectanticModelException($"Failed to load ONNX model from {modelPath}. {ex.Message}", ex);
        }
    }

    // -------------------- METHS --------------------
    public Task<IDisposableReadOnlyCollection<DisposableNamedOnnxValue>> RunAsync(
        IReadOnlyCollection<NamedOnnxValue> inputs, CancellationToken ct = default
    ) => Task.Run(() => {
        try { return _session.Run(inputs); }
        catch (Exception ex) {
            throw new VectanticModelException($"ONNX inference failed. {ex.Message}", ex);
        }
    }, ct);

    public void Dispose() => _session.Dispose();
}