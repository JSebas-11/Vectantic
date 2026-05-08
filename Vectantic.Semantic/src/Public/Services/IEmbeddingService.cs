using Vectantic.Core.Exceptions;
using Vectantic.Semantic.Entities;
using Vectantic.Semantic.Exceptions;

namespace Vectantic.Semantic.Services;

/// <summary>
/// Provides semantic embedding generation for text inputs.
/// </summary>
/// <remarks>
/// Implementations tokenize input text, execute ONNX inference,
/// apply pooling strategies, and optionally normalize the resulting vectors.
/// Generated embeddings can be used for semantic search, clustering,
/// classification, or similarity comparison workloads.
/// </remarks>
/// <example>
/// <code>
/// var embedding = await embeddingService.EmbedAsync("Vectantic enables local semantic embeddings.");
/// </code>
/// </example>
public interface IEmbeddingService {
    
    /// <summary>
    /// Generates a semantic embedding for a single text input.
    /// </summary>
    /// <param name="text">
    /// The input text to embed.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to cancel the embedding operation.
    /// </param>
    /// <returns>
    /// A task containing the generated semantic embedding.
    /// </returns>
    /// <exception cref="VectanticTokenizationException">
    /// Thrown when tokenization fails due to invalid tokenizer resources or input processing errors.
    /// </exception>
    /// <exception cref="VectanticModelException">
    /// Thrown when ONNX inference execution fails.
    /// </exception>
    /// <exception cref="VectanticPoolingException">
    /// Thrown when the model output tensor cannot be pooled into a final embedding.
    /// </exception>
    /// <exception cref="VectanticMathException">
    /// Thrown when a vector operation was not able to be perfomed on embedding.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled through the provided cancellation token.
    /// </exception>
    Task<Embedding> EmbedAsync(string text, CancellationToken ct = default);
    
    /// <summary>
    /// Generates semantic embeddings for multiple text inputs in a single batch operation.
    /// </summary>
    /// <param name="texts">
    /// The collection of input texts to embed.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to cancel the embedding operation.
    /// </param>
    /// <returns>
    /// A task containing the generated semantic embeddings.
    /// </returns>
    /// <exception cref="VectanticTokenizationException">
    /// Thrown when tokenization fails due to invalid tokenizer resources or input processing errors.
    /// </exception>
    /// <exception cref="VectanticModelException">
    /// Thrown when ONNX inference execution fails.
    /// </exception>
    /// <exception cref="VectanticPoolingException">
    /// Thrown when the model output tensor cannot be pooled into final embeddings.
    /// </exception>
    /// <exception cref="VectanticMathException">
    /// Thrown when a vector operation was not able to be perfomed on embeddings.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled through the provided cancellation token.
    /// </exception>
    /// <remarks>
    /// Batch embedding operations reduce inference overhead by processing multiple
    /// inputs within a single ONNX execution pipeline.
    /// </remarks>
    Task<IReadOnlyList<Embedding>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default);
}