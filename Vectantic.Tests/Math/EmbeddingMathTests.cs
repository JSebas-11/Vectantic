using Vectantic.Math;

namespace Vectantic.Tests.Math;

[Trait("Module", "Math")]
[Trait("Feature", "Embeddings")]
public class EmbeddingMathTests {

    #region DOT
    [Theory]
    [InlineData(new float[] { 1f, 2f, 3f }, new float[] { 4f, 5f, 6f }, 32f)]
    [InlineData(new float[] { 1f, 0f }, new float[] { 0f, 1f }, 0f)]
    [InlineData(new float[] { -1f, -2f }, new float[] { 3f, 4f }, -11f)]
    public void Dot_KnownInputs_ReturnsCorrectResult(float[] a, float[] b, float expected)
        => Assert.Equal(expected, EmbeddingMath.Dot(a, b), 5);

    [Fact]
    public void Dot_LargeVectors_MatchesVectorOps() {
        var rng = new Random(42);
        float[] a = new float[256], b = new float[256];
        for (int i = 0; i < 256; i++) { a[i] = (float)rng.NextDouble(); b[i] = (float)rng.NextDouble(); }
        Assert.Equal(VectorOps.Dot(a, b), EmbeddingMath.Dot(a, b), 4);
    }

    [Fact]
    public void Dot_EmptyVector_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.Dot([], [1f, 2f]));

    [Fact]
    public void Dot_MismatchedLengths_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.Dot([1f, 2f], [1f]));
    #endregion

    #region MAGNITUDE
    [Theory]
    [InlineData(new float[] { 3f, 4f }, 5f)]
    [InlineData(new float[] { 1f, 0f, 0f }, 1f)]
    [InlineData(new float[] { -3f, -4f }, 5f)]
    public void Magnitude_KnownInputs_ReturnsCorrectResult(float[] v, float expected)
        => Assert.Equal(expected, EmbeddingMath.Magnitude(v), 5);

    [Fact]
    public void Magnitude_LargeVector_MatchesVectorOps() {
        var rng = new Random(7);
        float[] v = new float[512];
        for (int i = 0; i < 512; i++) v[i] = (float)(rng.NextDouble() - 0.5) * 10f;
        Assert.Equal(VectorOps.Magnitude(v), EmbeddingMath.Magnitude(v), 5);
    }

    [Fact]
    public void Magnitude_ZeroVector_ReturnsZero()
        => Assert.Equal(0f, EmbeddingMath.Magnitude([0f, 0f, 0f]), 5);

    [Fact]
    public void Magnitude_EmptyVector_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.Magnitude([]));
    #endregion

    #region NORMALIZE
    [Fact]
    public void Normalize_AnyVector_ResultHasMagnitudeOne() {
        float[] v = [3f, 4f];
        EmbeddingMath.Normalize(v);
        Assert.Equal(1f, EmbeddingMath.Magnitude(v), 5);
    }

    [Fact]
    public void Normalize_BasicVector_ComponentsAreCorrect() {
        float[] v = [3f, 4f];
        EmbeddingMath.Normalize(v);
        Assert.Equal(0.6f, v[0], 5);
        Assert.Equal(0.8f, v[1], 5);
    }

    [Fact]
    public void Normalize_MutatesSpanInPlace() {
        float[] v = [3f, 4f];
        EmbeddingMath.Normalize(v);
        Assert.NotEqual(3f, v[0]);
    }

    [Fact]
    public void Normalize_ZeroVector_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.Normalize([0f, 0f]));

    [Fact]
    public void Normalize_EmptyVector_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.Normalize(new float[0].AsSpan()));
    #endregion

    #region COSINESIMILARITY
    [Fact]
    public void CosineSimilarity_IdenticalVectors_ReturnsOne()
        => Assert.Equal(1f, EmbeddingMath.CosineSimilarity([1f, 2f, 3f], [1f, 2f, 3f]), 5);

    [Fact]
    public void CosineSimilarity_OppositeVectors_ReturnsNegativeOne()
        => Assert.Equal(-1f, EmbeddingMath.CosineSimilarity([1f, 0f], [-1f, 0f]), 5);

    [Fact]
    public void CosineSimilarity_OrthogonalVectors_ReturnsZero()
        => Assert.Equal(0f, EmbeddingMath.CosineSimilarity([1f, 0f], [0f, 1f]), 5);

    [Fact]
    public void CosineSimilarity_LargeVectors_MatchesManualCalculation() {
        var rng = new Random(99);
        float[] a = new float[384], b = new float[384];
        for (int i = 0; i < 384; i++) { a[i] = (float)(rng.NextDouble() - 0.5); b[i] = (float)(rng.NextDouble() - 0.5); }

        float expected = VectorOps.Dot(a, b) / (VectorOps.Magnitude(a) * VectorOps.Magnitude(b));
        Assert.Equal(expected, EmbeddingMath.CosineSimilarity(a, b), 5);
    }

    [Fact]
    public void CosineSimilarity_ZeroVectorA_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.CosineSimilarity([0f, 0f], [1f, 2f]));

    [Fact]
    public void CosineSimilarity_ZeroVectorB_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.CosineSimilarity([1f, 2f], [0f, 0f]));

    [Fact]
    public void CosineSimilarity_MismatchedLengths_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.CosineSimilarity([1f, 2f], [1f]));
    #endregion

    #region TOPK
    private static readonly List<float[]> _candidates = [
        [0f, 0f, 1f],
        [1f, 0f, 0f],
        [0f, 1f, 0f],
    ];

    [Fact]
    public void TopK_KEqualsOne_ReturnsBestCandidate() {
        var result = EmbeddingMath.TopK([1f, 0f, 0f], _candidates, k: 1);
        Assert.Single(result);
        Assert.Equal(1, result[0].Index);
        Assert.Equal(1f, result[0].Score, 5);
    }

    [Fact]
    public void TopK_ResultsAreInDescendingOrder() {
        var result = EmbeddingMath.TopK([1f, 0f, 0f], _candidates, k: 3);
        for (int i = 0; i < result.Count - 1; i++)
            Assert.True(result[i].Score >= result[i + 1].Score);
    }

    [Fact]
    public void TopK_AllEqualScores_ReturnsKResults() {
        var equal = new List<float[]> { 
            new float[] {1f, 0f}, new float[] { 1f, 0f }, new float[] { 1f, 0f } 
        };
        var result = EmbeddingMath.TopK([1f, 0f], equal, k: 2);
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(1f, r.Score, 5));
    }

    [Fact]
    public void TopK_LargeDataset_ScoresMatchVectorOps() {
        var rng = new Random(55);
        float[] query = new float[64];
        for (int i = 0; i < 64; i++) query[i] = (float)rng.NextDouble();

        var candidates = new List<float[]>(20);
        for (int j = 0; j < 20; j++)
        {
            float[] c = new float[64];
            for (int i = 0; i < 64; i++) c[i] = (float)rng.NextDouble();
            candidates.Add(c);
        }

        var result = EmbeddingMath.TopK(query, candidates, k: 5);
        float queryMag = VectorOps.Magnitude(query);

        foreach (var (idx, score) in result)
        {
            float expected = VectorOps.Dot(query, candidates[idx])
                / (queryMag * VectorOps.Magnitude(candidates[idx]));
            Assert.Equal(expected, score, 5);
        }
    }

    [Fact]
    public void TopK_KIsZero_ThrowsArgumentOutOfRangeException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => EmbeddingMath.TopK([1f, 0f], _candidates, k: 0));

    [Fact]
    public void TopK_KLargerThanCandidateCount_ThrowsArgumentOutOfRangeException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => EmbeddingMath.TopK([1f, 0f, 0f], _candidates, k: 99));

    [Fact]
    public void TopK_ZeroQueryVector_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => EmbeddingMath.TopK([0f, 0f, 0f], _candidates, k: 1));

    [Fact]
    public void TopK_ZeroCandidateVector_ThrowsArgumentException() {
        var withZero = new List<float[]> { 
            new float[] { 1f, 0f }, new float[] { 0f, 0f } 
        };
        Assert.Throws<ArgumentException>(() => EmbeddingMath.TopK([1f, 0f], withZero, k: 1));
    }

    [Fact]
    public void TopK_CandidateWrongLength_ThrowsArgumentException() {
        var wrongLen = new List<float[]> { 
            new float[] { 1f, 0f, 0f }, new float[] { 1f, 0f } 
        };
        Assert.Throws<ArgumentException>(() => EmbeddingMath.TopK([1f, 0f, 0f], wrongLen, k: 1));
    }
    #endregion

    #region AboveThreshold
    [Fact]
    public void AboveThreshold_AllCandidatesAboveThreshold_ReturnsAll() {
        float[] query = [1f, 0f, 0f];
        var candidates = new List<float[]> {
            new[] { 1f, 0f, 0f }, new[] { 1f, 1f, 0f }
        };

        var result = EmbeddingMath.AboveThreshold(query, candidates, minScore: 0.5f);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void AboveThreshold_NoCandidatesAboveThreshold_ReturnsEmpty() {
        float[] query = [1f, 0f, 0f];
        var candidates = new List<float[]> {
            new[] { 0f, 1f, 0f }, new[] { -1f, 0f, 0f }
        };

        var result = EmbeddingMath.AboveThreshold(query, candidates, minScore: 0.5f);
        Assert.Empty(result);
    }

     [Fact]
    public void AboveThreshold_ScoreExactlyAtThreshold_IsIncluded() {
        float[] query = [1f, 0f];
        var candidates = new List<float[]> { new[] { 1f, 0f } };

        var result = EmbeddingMath.AboveThreshold(query, candidates, minScore: 1.0f);

        Assert.Single(result);
        Assert.Equal(1f, result[0].Score, 5);
    }

    [Fact]
    public void AboveThreshold_ResultsAreInDescendingOrder() {
        float[] query = [1f, 0f, 0f];
        var candidates = new List<float[]> {
            new[] { 0f, 1f, 0f }, new[] { 1f, 1f, 0f }, new[] { 1f, 0f, 0f }
        };

        var result = EmbeddingMath.AboveThreshold(query, candidates, minScore: -1f);

        for (int i = 0; i < result.Count - 1; i++)
            Assert.True(result[i].Score >= result[i + 1].Score, $"Not descending at position {i}");
    }

    [Fact]
    public void AboveThreshold_CorrectIndicesReturned() {
        float[] query = [1f, 0f, 0f];
        var candidates = new List<float[]> {
            new[] { -1f, 0f, 0f }, new[] {  1f, 0f, 0f }, new[] {  0f, 1f, 0f }
        };

        var result = EmbeddingMath.AboveThreshold(query, candidates, minScore: 0.5f);

        Assert.Single(result);
        Assert.Equal(1, result[0].Index);
    }

     [Fact]
    public void AboveThreshold_MinScoreNegativeOne_ReturnsAllCandidates() {
        float[] query = [1f, 0f];
        var candidates = new List<float[]> {
            new[] {  1f, 0f }, new[] {  0f, 1f }, new[] { -1f, 0f }
        };

        var result = EmbeddingMath.AboveThreshold(query, candidates, minScore: -1f);

        Assert.Equal(3, result.Count);
    }

     [Fact]
    public void AboveThreshold_MinScoreBelowNegativeOne_ThrowsArgumentException() {
        float[] query = [1f, 0f];
        var candidates = new List<float[]> { new[] { 1f, 0f } };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EmbeddingMath.AboveThreshold(query, candidates, minScore: -1.1f));
    }

    [Fact]
    public void AboveThreshold_MinScoreAboveOne_ThrowsArgumentException() {
        float[] query = [1f, 0f];
        var candidates = new List<float[]> { new[] { 1f, 0f } };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EmbeddingMath.AboveThreshold(query, candidates, minScore: 1.1f));
    }


    [Fact]
    public void AboveThreshold_ZeroQueryVector_ThrowsArgumentException() {
        var candidates = new List<float[]> { new[] { 1f, 0f } };

        Assert.Throws<ArgumentException>(() =>
            EmbeddingMath.AboveThreshold([0f, 0f], candidates, minScore: 0f));
    }

    [Fact]
    public void AboveThreshold_ZeroCandidateVector_ThrowsArgumentException() {
        float[] query = [1f, 0f];
        var candidates = new List<float[]> { new[] { 1f, 0f }, new[] { 0f, 0f } };

        Assert.Throws<ArgumentException>(() =>
            EmbeddingMath.AboveThreshold(query, candidates, minScore: 0f));
    }

    [Fact]
    public void AboveThreshold_CandidateWrongLength_ThrowsArgumentException() {
        float[] query = [1f, 0f, 0f];
        var candidates = new List<float[]> { new[] { 1f, 0f } };

        Assert.Throws<ArgumentException>(() =>
            EmbeddingMath.AboveThreshold(query, candidates, minScore: 0f));
    }

    [Fact]
    public void AboveThreshold_EmptyQueryVector_ThrowsArgumentException() {
        var candidates = new List<float[]> { new[] { 1f, 0f } };

        Assert.Throws<ArgumentException>(() =>
            EmbeddingMath.AboveThreshold([], candidates, minScore: 0f));
    }
    #endregion
}