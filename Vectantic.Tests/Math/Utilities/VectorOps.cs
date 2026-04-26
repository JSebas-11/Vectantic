namespace Vectantic.Tests.Math;

internal static class VectorOps {
    internal static float Dot(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        float sum = 0f;
        for (int i = 0; i < a.Length; i++) sum += a[i] * b[i];
        return sum;
    }

    internal static float Magnitude(ReadOnlySpan<float> v) {
        float sum = 0f;
        for (int i = 0; i < v.Length; i++) sum += v[i] * v[i];
        return MathF.Sqrt(sum);
    }
}