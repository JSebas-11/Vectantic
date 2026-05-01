using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime.Tensors;
using Vectantic.Semantic.Exceptions;
using Vectantic.Semantic.Internal.Utilities;

namespace Vectantic.Semantic.Internal.Pooling;

internal sealed class ClsPooling : IPoolingStrategy {
    public float[] Pool(DenseTensor<float> tensor, long[,] attentionMask) {
        _ = attentionMask;

        float[] pool() {
            var buffer = tensor.Buffer.Span;
            var hiddenDimLen = tensor.Dimensions[2];

            var result = new float[hiddenDimLen];
            buffer[..hiddenDimLen].CopyTo(result);

            return result;
        }

        return SemanticExceptionHandler.Handle(
            pool, ex => new VectanticPoolingException($"Unexpected error during pooling: {ex.Message}", ex)
        );
    }

    public float[,] PoolBatch(DenseTensor<float> tensor, long[,] attentionMasks) {
        _ = attentionMasks;

        float[,] poolBatch() {
            var buffer = tensor.Buffer.Span;
            var batchLen = tensor.Dimensions[0];
            var seqLen = tensor.Dimensions[1];
            var hiddenDimLen = tensor.Dimensions[2];
            var stride = seqLen * hiddenDimLen;

            var result = new float[batchLen, hiddenDimLen];

            for (int i = 0; i < batchLen; i++) {
                var offset = i * stride;
                var resultSpan = MemoryMarshal.CreateSpan(ref result[i, 0], hiddenDimLen);
                buffer.Slice(offset, hiddenDimLen).CopyTo(resultSpan);
            }

            return result;
        }

        return SemanticExceptionHandler.Handle(
            poolBatch, ex => new VectanticPoolingException($"Unexpected error during pooling: {ex.Message}", ex)
        );
    }
}