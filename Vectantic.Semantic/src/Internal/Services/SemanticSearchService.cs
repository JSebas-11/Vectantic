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
        var (queryEmbed, docsEmbed) = await GetEmbeddingsAsync(query, docs);
        var topKList = TryMath(
            () => EmbeddingMath.TopK(queryEmbed, docsEmbed, topK)
        );

        return SemanticResultDoc(topKList, docs);
    }

    public async Task<SemanticSearchResults> SearchAsync(string query, IReadOnlyList<string> docs, float minScore) {
        var (queryEmbed, docsEmbed) = await GetEmbeddingsAsync(query, docs);
        var minScoreList = TryMath(
            () => EmbeddingMath.AboveThreshold(queryEmbed, docsEmbed, minScore)
        );

        return SemanticResultDoc(minScoreList, docs);
    }

    public Task<SemanticSearchResults> SearchAsync(float[] query, IReadOnlyList<float[]> docs, int topK)
        => Task.FromResult(
                SemanticResultVector(
                    TryMath(() => EmbeddingMath.TopK(query.AsSpan(), docs, topK)),
                    docs.Count
                )
            );

    public Task<SemanticSearchResults> SearchAsync(float[] query, IReadOnlyList<float[]> docs, float minScore)
        => Task.FromResult(
                SemanticResultVector(
                    TryMath(() => EmbeddingMath.AboveThreshold(query.AsSpan(), docs, minScore)),
                    docs.Count
                )
            );

    // -------------------- INNER METHS --------------------
    private static IReadOnlyList<(int index, float score)> TryMath(
        Func<IReadOnlyList<(int index, float score)>> func) 
    {
        try {
            return func();
        }
        catch (ArgumentOutOfRangeException ex) {
            throw new VectanticMathException($"A math error occurred during semantic searching: {ex.Message}", ex);
        }
        catch (ArgumentException ex) {
            throw new VectanticMathException($"A math error occurred during semantic searching: {ex.Message}", ex);
        }
    }

    private async Task<(float[] queryEmbed, IReadOnlyList<float[]> docsEmbed)> GetEmbeddingsAsync(
        string query, IReadOnlyList<string> docs) 
    {
        var queryTask = _embeddingService.EmbedAsync(query);
        var docsTask = _embeddingService.EmbedBatchAsync(docs);
        await Task.WhenAll(queryTask, docsTask).ConfigureAwait(false);

        var docsVectors = docsTask.Result.Select(e => e.Vector).ToArray();

        return (queryTask.Result.Vector, docsVectors.AsReadOnly());
    }

    private static SemanticSearchResults SemanticResultVector(
        IReadOnlyList<(int index, float score)> matchesList, int totalDocs) 
    {
        var matches = new List<SemanticMatch>(matchesList.Count);
        foreach (var (index, score) in matchesList)
            matches.Add(new SemanticMatch() 
                { OriginalIndex = index, Score = score }
            );

        return new SemanticSearchResults() { Matches = matches, TotalCandidates = totalDocs } ;
    }
    private static SemanticSearchResults SemanticResultDoc(
        IReadOnlyList<(int index, float score)> matchesList, IReadOnlyList<string> docs) 
    {
        var matches = new List<SemanticMatch>(matchesList.Count);
        foreach (var (index, score) in matchesList)
            matches.Add(new SemanticMatch() 
                { Text = docs[index], OriginalIndex = index, Score = score }
            );

        return new SemanticSearchResults() { Matches = matches, TotalCandidates = docs.Count };
    }
}