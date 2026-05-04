using Microsoft.ML.OnnxRuntime.Tensors;
using Vectantic.Semantic.Exceptions;
using Vectantic.Semantic.Internal.Pooling;
using Vectantic.Semantic.Internal.Utilities;

namespace Vectantic.Semantic.Internal.Tokenization;

internal sealed class MeanPooling : IPoolingStrategy {
    // -------------------- METHS --------------------
    public float[] Pool(DenseTensor<float> tensor, long[,] attentionMask) {
        float[] pool() {
            var buffer = tensor.Buffer.Span;
            var seqLen = tensor.Dimensions[1];
            var hiddenDimLen = tensor.Dimensions[2];

            var (sums, count) = ComputeMaskedSums(buffer, attentionMask, 0, seqLen, hiddenDimLen);

            var result = new float[hiddenDimLen];
            for (int d = 0; d < hiddenDimLen; d++) 
                result[d] = sums[d] / count;

            return result;
        }

        return SemanticExceptionHandler.Handle(
            pool,
            ex => new VectanticPoolingException($"Unexpected error during pooling: {ex.Message}", ex)
        );
    }

    public float[,] PoolBatch(DenseTensor<float> tensor, long[,] attentionMasks) {
        float[,] poolBatch() {
            var buffer = tensor.Buffer.Span;
            var batchSize = tensor.Dimensions[0];
            var seqLen = tensor.Dimensions[1];
            var hiddenDimLen = tensor.Dimensions[2];

            var results = new float[batchSize, hiddenDimLen];
            for (int b = 0; b < batchSize; b++) {
                var (batchSum, count) = ComputeMaskedSums(buffer, attentionMasks, b, seqLen, hiddenDimLen);
                
                for (int d = 0; d < hiddenDimLen; d++)
                    results[b, d] = batchSum[d] / count;
            }

            return results;
        }

        return SemanticExceptionHandler.Handle(
            poolBatch,
            ex => new VectanticPoolingException($"Unexpected error during pooling: {ex.Message}", ex)
        );
    }

    // -------------------- INNER METHS --------------------
    private static (float[] sums, int count) ComputeMaskedSums(
        Span<float> buffer, long[,] attentionMask,
        int batchIndex, int seqLen, int hiddenDimLen) 
    {
        var batchOffset = batchIndex * seqLen * hiddenDimLen;
        
        var sums = new float[hiddenDimLen];
        var count = 0;

        for (int s = 0; s < seqLen; s++) {
            if (attentionMask[batchIndex, s] == 1) {
                count++;

                var offset = batchOffset + s * hiddenDimLen;
                for (int d = 0; d < hiddenDimLen; d++)
                    sums[d] += buffer[offset + d];
            }
        }

        return (sums, count);
    }
}