using Vectantic.Core.Exceptions;
using Vectantic.Math;
using Vectantic.Semantic.Entities;
using Vectantic.Semantic.Services;

namespace Vectantic.Semantic.Internal.Services;

internal sealed class SemanticSearchService : ISemanticSearchService {
    // -------------------- INIT --------------------
    private readonly IEmbeddingService _embeddingService;

    public SemanticSearchService(IEmbeddingService embeddingService) {
        _embeddingService = embeddingService;
    }

    // -------------------- METHS --------------------
    public async Task<SemanticSearchResults> SearchAsync(string query, IReadOnlyList<string> docs, int topK) {
        var queryTask = _embeddingService.EmbedAsync(query);
        var docsTask = _embeddingService.EmbedBatchAsync(docs);
        await Task.WhenAll(queryTask, docsTask).ConfigureAwait(false);

        var topKList = TryTopK(queryTask.Result.Vector, [.. docsTask.Result.Select(e => e.Vector)], topK);

        var matches = new List<SemanticMatch>(topK);
        foreach (var (index, score) in topKList)
            matches.Add(new SemanticMatch() 
                { Text = docs[index], OriginalIndex = index, Score = score }
            );

        return new SemanticSearchResults() { Matches = matches, TotalCandidates = docs.Count };
    }

    public Task<SemanticSearchResults> SearchAsync(float[] query, IReadOnlyList<float[]> docs, int topK) {
        var topKList = TryTopK(query, docs, topK);

        var matches = new List<SemanticMatch>(topK);
        foreach (var (index, score) in topKList)
            matches.Add(new SemanticMatch() 
                { OriginalIndex = index, Score = score }
            );

        return Task.FromResult(new SemanticSearchResults() 
            { Matches = matches, TotalCandidates = docs.Count }
        );
    }

    // -------------------- INNER METHS --------------------
    private static IReadOnlyList<(int index, float score)> TryTopK(float[] query, IReadOnlyList<float[]> docs, int topK) {
        try {
            return EmbeddingMath.TopK(query.AsSpan(), docs, topK);
        }
        catch (ArgumentOutOfRangeException ex) {
            throw new VectanticMathException($"A math error occurred during semantic searching: {ex.Message}", ex);
        }
        catch (ArgumentException ex) {
            throw new VectanticMathException($"A math error occurred during semantic searching: {ex.Message}", ex);
        }
    }
}