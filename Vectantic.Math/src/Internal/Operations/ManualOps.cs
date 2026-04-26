using Vectantic.Math.Internal.Utilities;

namespace Vectantic.Math.Internal.Operations;

internal static class ManualOps {
    internal static float Dot(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        VectorGuard.NonEmptyOrException(a);
        VectorGuard.SameLengthOrException(a, b);

        float dot = 0;
        for (int i = 0; i < a.Length; i++) dot += a[i] * b[i];

        return dot;
    }

    internal static float Magnitude(ReadOnlySpan<float> v) {
        VectorGuard.NonEmptyOrException(v);
        float squareSum = 0;
        for (int i = 0; i < v.Length; i++) squareSum += v[i] * v[i];

        return MathF.Sqrt(squareSum);
    }
    
    internal static void Normalize(Span<float> v) {
        var magnitude = Magnitude(v);
        VectorGuard.NonZeroOrException(magnitude, "Magnitude");

        for (int i = 0; i < v.Length; i++) v[i] /= magnitude;
    }
    
    internal static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        var magA = Magnitude(a);
        var magB = Magnitude(b);

        VectorGuard.NonZeroOrException(magA, "Magnitude");
        VectorGuard.NonZeroOrException(magB, "Magnitude");

        return Dot(a, b) / (magA * magB);
    }
    
    internal static IReadOnlyList<(int Index, float Score)> TopK(
        ReadOnlySpan<float> query, IReadOnlyList<float[]> candidates, int k) 
    {
        VectorGuard.ValidKOrException(k, candidates.Count);

        var pQueue = new PriorityQueue<(int Index, float Score), float>(k);

        for (int i = 0; i < candidates.Count; i++) {
            var score = CosineSimilarity(query, candidates[i]);
            
            if (pQueue.Count < k) {
                pQueue.Enqueue(new (i, score), score);
                continue;
            }

            pQueue.TryPeek(out _, out float lowestScore);
            if (score > lowestScore) {
                pQueue.Dequeue();
                pQueue.Enqueue(new (i, score), score);
            }
        }

        var kArray = new (int Index, float Score)[k];

        var index = pQueue.Count - 1;
        while (pQueue.Count > 0) {
            kArray[index] = pQueue.Dequeue();
            index--;
        }

        return kArray.AsReadOnly();
    }
}