using Vectantic.Semantic.Entities;

namespace Vectantic.Semantic.Services;

public interface ISemanticSearchService {
    Task<SemanticSearchResults> SearchAsync(string query, IReadOnlyList<string> docs, int topK);
    Task<SemanticSearchResults> SearchAsync(float[] query, IReadOnlyList<float[]> docs, int topK);
}