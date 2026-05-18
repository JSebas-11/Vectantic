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
    /// Performs semantic similarity search over a collection of text documents, returning all matches above a minimum score threshold.
    /// </summary>
    /// <param name="query">The query text to compare semantically against the candidate documents.</param>
    /// <param name="docs">The candidate documents to evaluate.</param>
    /// <param name="minScore">
    /// The minimum cosine similarity score a candidate must meet to be included in the results.
    /// For normalized vectors this ranges from 0 to 1. For unnormalized vectors from -1 to 1.
    /// </param>
    /// <returns>
    /// A task containing <see cref="SemanticSearchResults"/> with all matches at or above <paramref name="minScore"/>, in descending score order.
    /// Returns an empty result set if no candidates meet the threshold — this is not an error.
    /// </returns>
    /// <exception cref="VectanticMathException">
    /// Thrown when <paramref name="minScore"/> is outside the valid range of -1 to 1.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when embedding generation or semantic comparison fails unexpectedly.
    /// </exception>
    /// <remarks>
    /// Unlike the topK overload, the number of results is variable and depends on how many candidates meet the threshold.
    /// </remarks>
    Task<SemanticSearchResults> SearchAsync(string query, IReadOnlyList<string> docs, float minScore);

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

     /// <summary>
    /// Performs semantic similarity search over pre-computed embedding vectors, returning all matches above a minimum score threshold.
    /// </summary>
    /// <param name="query">The query embedding vector to compare against candidates.</param>
    /// <param name="docs">The candidate embedding vectors to evaluate.</param>
    /// <param name="minScore">
    /// The minimum cosine similarity score a candidate must meet to be included in the results.
    /// For normalized vectors this ranges from 0 to 1. For unnormalized vectors from -1 to 1.
    /// </param>
    /// <returns>
    /// A task containing <see cref="SemanticSearchResults"/> with all matches at or above <paramref name="minScore"/>, in descending score order.
    /// <see cref="SemanticMatch.Text"/> will be null as no source text is available.
    /// Returns an empty result set if no candidates meet the threshold — this is not an error.
    /// </returns>
    /// <exception cref="VectanticMathException">
    /// Thrown when <paramref name="minScore"/> is outside the valid range of -1 to 1.
    /// </exception>
    /// <exception cref="VectanticException">
    /// Thrown when semantic comparison fails unexpectedly.
    /// </exception>
    /// <remarks>
    /// Use this overload when embeddings are pre-computed and cached, avoiding redundant embedding generation.
    /// </remarks>
    Task<SemanticSearchResults> SearchAsync(float[] query, IReadOnlyList<float[]> docs, float minScore);
}