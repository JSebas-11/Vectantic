using Vectantic.Core.Exceptions;
using Vectantic.Semantic.Entities;

namespace Vectantic.Semantic.Services;

/// <summary>
/// Provides semantic similarity search over text documents or embedding vectors.
/// </summary>
/// <remarks>
/// Implementations generate or compare embeddings using cosine similarity
/// and return the highest scoring semantic matches.
/// </remarks>
/// <example>
/// <code>
/// var results = await semanticSearchService.SearchAsync(
///     "machine learning",
///     documents,
///     topK: 5);
/// </code>
/// </example>
public interface ISemanticSearchService {
    
    /// <summary>
    /// Performs semantic similarity search over a collection of text documents.
    /// </summary>
    /// <param name="query">
    /// The query text used for semantic comparison.
    /// </param>
    /// <param name="docs">
    /// The candidate documents to evaluate.
    /// </param>
    /// <param name="topK">
    /// The maximum number of highest-scoring matches to return.
    /// </param>
    /// <returns>
    /// A task containing the semantic search results.
    /// </returns>
    /// <exception cref="VectanticMathException">
    /// Thrown when <paramref name="topK"/> is less than or equal to zero or exceeds the amount of candidates.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when embedding generation or semantic comparison fails.
    /// </exception>
    /// <remarks>
    /// Both the query and candidate documents are embedded before similarity scoring is performed.
    /// </remarks>
    Task<SemanticSearchResults> SearchAsync(string query, IReadOnlyList<string> docs, int topK);

    /// <summary>
    /// Performs semantic similarity search over embedding vectors.
    /// </summary>
    /// <param name="query">
    /// The query embedding vector.
    /// </param>
    /// <param name="docs">
    /// The candidate embedding vectors to evaluate.
    /// </param>
    /// <param name="topK">
    /// The maximum number of highest-scoring matches to return.
    /// </param>
    /// <returns>
    /// A task containing the semantic search results.
    /// </returns>
    /// <exception cref="VectanticMathException">
    /// Thrown when <paramref name="topK"/> is less than or equal to zero or exceeds the amount of candidates.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when embedding generation or semantic comparison fails.
    /// </exception>
    Task<SemanticSearchResults> SearchAsync(float[] query, IReadOnlyList<float[]> docs, int topK);
}