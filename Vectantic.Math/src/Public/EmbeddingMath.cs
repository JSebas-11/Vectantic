using System.Numerics.Tensors;
using Vectantic.Math.Internal.Utilities;

namespace Vectantic.Math;

public static class EmbeddingMath {
    // -------------------- METHS --------------------
    public static float Dot(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        VectorGuard.NonEmptyOrException(a);
        VectorGuard.SameLengthOrException(a, b);
        
        return TensorPrimitives.Dot(a, b); 
    }

    public static float Magnitude(ReadOnlySpan<float> v) {
        VectorGuard.NonEmptyOrException(v);
        var squareSum = TensorPrimitives.SumOfSquares(v);
        
        return MathF.Sqrt(squareSum);
    }
    
    public static void Normalize(Span<float> v) {
        var magnitude = Magnitude(v);
        VectorGuard.NonZeroOrException(magnitude, "Magnitude");

        TensorPrimitives.Multiply(v, 1f / magnitude, v);
    }
    
    public static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        VectorGuard.SameLengthOrException(a, b);

        var magA = Magnitude(a);
        var magB = Magnitude(b);
        VectorGuard.NonZeroOrException(magA, "Magnitude A");
        VectorGuard.NonZeroOrException(magB, "Magnitude B");

        return TensorPrimitives.Dot(a, b) /  (magA * magB);
    }
    
    public static IReadOnlyList<(int Index, float Score)> TopK(
        ReadOnlySpan<float> query, IReadOnlyList<float[]> candidates, int k)
    {
        var candidatesCount = candidates.Count;
        VectorGuard.ValidKOrException(k, candidatesCount);
        
        var queryMag = Magnitude(query);
        VectorGuard.NonZeroOrException(queryMag, "Query Magnitude");
        
        var candidatesMag = PrevalidateCandidatesMagnitude(candidates, query.Length);
        
        var pQueue = new PriorityQueue<(int Index, float Score), float>(k);
        for (int i = 0; i < candidatesCount; i++) {
            var score = TensorPrimitives.Dot(query, candidates[i]) / (queryMag * candidatesMag[i]);

            if (pQueue.Count < k) pQueue.Enqueue((i, score), score);
            else if (pQueue.TryPeek(out _, out var lowestScore) && score > lowestScore) {
                pQueue.Dequeue();
                pQueue.Enqueue((i, score), score);
            }
        }

        var kArray = new (int Index, float Score)[k];
        for (int i = pQueue.Count - 1; i >= 0; i--) kArray[i] = pQueue.Dequeue();
        
        return kArray.AsReadOnly();
    }
    
    // -------------------- INNER METHS --------------------
    private static float[] PrevalidateCandidatesMagnitude(
        IReadOnlyList<float[]> candidates, int queryLen) 
    {
        var candidatesCount = candidates.Count;
        var magnitudes = new float[candidatesCount];
        
        for (int i = 0; i < candidatesCount; i++) {
            VectorGuard.SameLengthOrException(candidates[i], queryLen);
            var candidateMag = Magnitude(candidates[i]);
            VectorGuard.NonZeroOrException(candidateMag, "Candidate Magnitude");
            magnitudes[i] = candidateMag;
        }
        
        return magnitudes;
    }
}