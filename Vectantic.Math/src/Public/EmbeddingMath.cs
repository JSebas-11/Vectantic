using System.Numerics.Tensors;
using Vectantic.Math.Internal.Utilities;

namespace Vectantic.Math;

/// <summary>
/// Provides high-performance vector math operations for semantic embeddings and similarity computation.
/// </summary>
/// <remarks>
/// All operations are allocation-free and optimized using <see cref="TensorPrimitives"/>
/// for SIMD-accelerated execution. This type can be used independently from the rest of the Vectantic ecosystem
/// for generic vector similarity and embedding workloads.
/// </remarks>
/// <example>
/// <code>
/// float similarity = EmbeddingMath.CosineSimilarity(
///     embeddingA,
///     embeddingB);
/// </code>
/// </example>
public static class EmbeddingMath {
    #region METHS

    /// <summary>
    /// Computes the dot product between two vectors.
    /// </summary>
    /// <param name="a">
    /// The first vector.
    /// </param>
    /// <param name="b">
    /// The second vector.
    /// </param>
    /// <returns>
    /// The dot product of the provided vectors.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when either vector is empty or when the vectors do not have the same length.
    /// </exception>
    /// <remarks>
    /// This operation is SIMD-accelerated through <see cref="TensorPrimitives"/>.
    /// </remarks>
    public static float Dot(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        VectorGuard.NonEmptyOrException(a);
        VectorGuard.SameLengthOrException(a, b);
        
        return TensorPrimitives.Dot(a, b); 
    }

    /// <summary>
    /// Computes the Euclidean magnitude of a vector.
    /// </summary>
    /// <param name="v">
    /// The input vector.
    /// </param>
    /// <returns>
    /// The Euclidean magnitude of the vector.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the vector is empty.
    /// </exception>
    /// <remarks>
    /// The magnitude is calculated as the square root of the sum of squared vector components.
    /// </remarks>
    public static float Magnitude(ReadOnlySpan<float> v) {
        VectorGuard.NonEmptyOrException(v);
        var squareSum = TensorPrimitives.SumOfSquares(v);
        
        return MathF.Sqrt(squareSum);
    }
    
    /// <summary>
    /// Normalizes a vector in-place to unit length.
    /// </summary>
    /// <param name="v">
    /// The vector to normalize.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the vector is empty or has zero magnitude.
    /// </exception>
    /// <remarks>
    /// This operation modifies the underlying vector directly without additional allocations.
    /// After normalization, the vector magnitude becomes approximately 1.
    /// </remarks>
    public static void Normalize(Span<float> v) {
        var magnitude = Magnitude(v);
        VectorGuard.NonZeroOrException(magnitude, "Magnitude");

        TensorPrimitives.Multiply(v, 1f / magnitude, v);
    }
    
    /// <summary>
    /// Computes the cosine similarity between two vectors.
    /// </summary>
    /// <param name="a">
    /// The first vector.
    /// </param>
    /// <param name="b">
    /// The second vector.
    /// </param>
    /// <returns>
    /// The cosine similarity score between the vectors.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the vectors have different lengths or when either vector has zero magnitude.
    /// </exception>
    /// <remarks>
    /// Cosine similarity measures the angular similarity between vectors
    /// and produces values typically ranging from -1 to 1.
    /// </remarks>
    public static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        VectorGuard.SameLengthOrException(a, b);

        var magA = Magnitude(a);
        var magB = Magnitude(b);
        VectorGuard.NonZeroOrException(magA, "Magnitude A");
        VectorGuard.NonZeroOrException(magB, "Magnitude B");

        return TensorPrimitives.Dot(a, b) /  (magA * magB);
    }
    
    /// <summary>
    /// Computes the top-k highest cosine similarity matches for a query embedding.
    /// </summary>
    /// <param name="query">
    /// The query embedding vector.
    /// </param>
    /// <param name="candidates">
    /// The candidate embedding vectors to compare against.
    /// </param>
    /// <param name="k">
    /// The number of highest-scoring matches to return.
    /// </param>
    /// <returns>
    /// A collection containing the candidate index and similarity score of the top matches.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="k"/> is less than or equal to zero,
    /// or greater than the number of candidate vectors.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when candidate vectors have incompatible dimensions
    /// or when any vector has zero magnitude.
    /// </exception>
    /// <remarks>
    /// This method prevalidates candidate vector dimensions and magnitudes before similarity scoring.
    /// Internally, a bounded priority queue is used to minimize allocations and reduce sorting overhead.
    /// </remarks>
    /// <example>
    /// <code>
    /// var matches = EmbeddingMath.TopK(
    ///     queryEmbedding,
    ///     candidateEmbeddings,
    ///     k: 5);
    /// </code>
    /// </example>
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
    #endregion
    
    #region INNER METHS
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
    #endregion
}