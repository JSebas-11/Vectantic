using Vectantic.Semantic.Entities;

namespace Vectantic.Semantic.Services;

public interface IEmbeddingService {
    Task<Embedding> EmbedAsync(string text);
    Task<IReadOnlyList<Embedding>> EmbedBatchAsync(IReadOnlyList<string> texts);
}