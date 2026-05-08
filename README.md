# 📦 Vectantic

A modern, high-performance .NET SDK for local text embeddings and semantic search — powered by ONNX, no external APIs required.

---

## 🚀 Why Vectantic?

Working with embeddings locally in .NET is complex — managing ONNX models, tokenization, pooling, and vector math is a lot of boilerplate.

Vectantic provides:

- **Local inference** — runs entirely on your machine, no API keys, no data leaving your environment
- **Clean abstractions** — simple IEmbeddingService and ISemanticSearchService interfaces
- **Built-in models** — MiniLM-L6-v2 and BGE-Small-EN-v1.5 ready to use out of the box
- **Automatic model download** — models fetched and cached on first use
- **Batching support** — embed multiple texts in a single inference pass
- **Vector math utilities** — dot product, cosine similarity, TopK search via Vectantic.Math
- **Modular architecture** — Core, Semantic, Math as separate NuGet packages

---

## ⚡ Quick Start

### Installation

```bash 
dotnet add package Vectantic.Core
dotnet add package Vectantic.Semantic
dotnet add package Vectantic.Math
```

### Registration

```csharp
var coreOptions = new VectanticOptions {
    AccessToken = "hf_..."  // optional, recommended for better download reliability and necessary for private models
};

var semanticOptions = new SemanticOptions {
    Normalize = true
};

await services
    .AddVectanticSemantic(coreOptions, semanticOptions, VectanticPreset.MiniLML6V2)
    .EnsureModelAsync();
```

> Note: `EnsureModelAsync()` downloads and caches the model on first run. Subsequent runs load from cache instantly.

### Embedding Text

```csharp
var embedder = provider.GetRequiredService<IEmbeddingService>();

// Single embedding
var embedding = await embedder.EmbedAsync("The quick brown fox");
Console.WriteLine($"Dimensions: {embedding.Dimensions}");
// → Dimensions: 384

// Batch embedding
var embeddings = await embedder.EmbedBatchAsync([
    "The quick brown fox",
    "A lazy dog sat",
    "Neural networks are fascinating"
]);
Console.WriteLine($"Batch count: {embeddings.Count}");
// → Batch count: 3
```

### Semantic Search

```csharp
var search = provider.GetRequiredService<ISemanticSearchService>();

var documents = new List<string> {
    "The Eiffel Tower is located in Paris",
    "Machine learning powers modern AI",
    "The Amazon rainforest covers Brazil",
    "Deep learning uses neural networks",
    "The Louvre museum is in France"
};

var results = await search.SearchAsync(
    query: "artificial intelligence and neural networks",
    docs: documents,
    topK: 2);

foreach (var match in results.Matches)
    Console.WriteLine($"[{match.Score:F4}] {match.Text}");

// → [0.6654] Deep learning uses neural networks
// → [0.5518] Machine learning powers modern AI
```

### Vector Math (Vectantic.Math)

```csharp
var a = new float[] { 1f, 2f, 3f };
var b = new float[] { 4f, 5f, 6f };

var similarity = EmbeddingMath.CosineSimilarity(a, b);
var dot = EmbeddingMath.Dot(a, b);

EmbeddingMath.Normalize(a.AsSpan());
```

---

## 📦 Packages

| Package            | Description                                                     |
|:-------------------|:----------------------------------------------------------------|
| Vectantic.Core     | Download infrastructure, Configuration, ONNX session, DI wiring |
| Vectantic.Semantic | Embedding service, semantic search, tokenization, pooling, etc  |
| Vectantic.Math     | Vector math utilities — dot product, cosine similarity, TopK    |

---

## 🤖 Built-in Models

| Preset                        | Model             | Dimensions | Pooling |
|:------------------------------|:------------------|:----------:|:-------:|
| VectanticPreset.MiniLML6V2    | all-MiniLM-L6-v2  | 384        | Mean    |
| VectanticPreset.BgeSmallEnV15 | bge-small-en-v1.5 | 384        | Mean    |

---

## 🔧 Custom Presets

```csharp
var customPreset = new PresetBuilder()
    .WithId("my-model")
    .WithModelUrl("https://huggingface.co/.../model.onnx")
    .WithChecksum("sha256_here")
    .ApplyLowerCase(true)
    .WithTokenTypeIds(true)
    .WithOutputTensorName("last_hidden_state")
    .WithTokenizerFiles([
        "https://huggingface.co/.../tokenizer.json",
        "https://huggingface.co/.../vocab.txt"
    ])
    .WithMaxTokens(512)
    .WithPoolingStrategy(PoolingStrategy.Mean)
    .WithTokenizationType(TokenizationType.WordPiece)
    .Build();

await services
    .AddVectanticSemantic(coreOptions, semanticOptions, customPreset)
    .EnsureModelAsync();
```

---

## 🗺️ Next Steps

- Additional built-in models
- Multi-model support
- GPU acceleration support
- `Vectantic.Api` — REST API wrapper for non-.NET consumers (n8n, Python, etc.)
- `Vectantic.Generative` — local LLM inference

---

## 🤝 Contributing

Contributions, issues and feature requests are welcome.

---

## 📄 License

This project is licensed under the MIT License. See the LICENSE file for details.

---

## 🙌 Credits

Created and maintained by JSebas-11 (Sebastian Delgado)
Built for developers who want powerful local AI without cloud dependencies.