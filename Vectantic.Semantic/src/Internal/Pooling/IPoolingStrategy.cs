using Microsoft.ML.OnnxRuntime.Tensors;

namespace Vectantic.Semantic.Internal.Pooling;

internal interface IPoolingStrategy {
    float[] Pool(DenseTensor<float> tensor, long[,] attentionMask);
    float[,] PoolBatch(DenseTensor<float> tensor, long[,] attentionMasks);
}