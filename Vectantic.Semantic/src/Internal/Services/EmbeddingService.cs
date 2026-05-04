using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Vectantic.Core.Exceptions;
using Vectantic.Core.Internal.Onnx;
using Vectantic.Math;
using Vectantic.Semantic.Configuration;
using Vectantic.Semantic.Entities;
using Vectantic.Semantic.Internal.Constants;
using Vectantic.Semantic.Internal.Models;
using Vectantic.Semantic.Internal.Pooling;
using Vectantic.Semantic.Internal.Tokenization;
using Vectantic.Semantic.Internal.Utilities;
using Vectantic.Semantic.Services;

namespace Vectantic.Semantic.Internal.Services;

internal sealed class EmbeddingService : IEmbeddingService {
    // -------------------- INIT --------------------
    private readonly SemanticOptions _opts;
    private readonly ResolvedSemanticModel _resolvedModel;
    private readonly IOnnxSession _onnxSession;
    private readonly ISemanticTokenizer _tokenizer;
    private readonly IPoolingStrategy _pooling;

    public EmbeddingService(
        SemanticOptions opts, ResolvedSemanticModel resolvedModel,
        IOnnxSession onnxSession,
        ISemanticTokenizer tokenizer, IPoolingStrategy pooling) 
    {
        _opts = opts;
        _resolvedModel = resolvedModel;
        _onnxSession = onnxSession;
        _tokenizer = tokenizer;
        _pooling = pooling;
    }

    // -------------------- METHS --------------------
    public async Task<Embedding> EmbedAsync(string text, CancellationToken ct = default) {
        text = EmbeddingGuard.ValidateAndNormalizeText(text);

        var tokResult = _tokenizer.Tokenize(text);
        var inputs = PackTokensToOnnxInputs(tokResult, _resolvedModel.RequiresTokenTypeIds);
        using var onnxResult = await _onnxSession.RunAsync(inputs, ct);
        var outputs = OnnxOutputsToDenseTensor(onnxResult, _resolvedModel.OutputTensorName);
        var result = _pooling.Pool(outputs, tokResult.AttentionMask);
        
        if (_opts.Normalize) {
            try { EmbeddingMath.Normalize(result); }
            catch (ArgumentException ex) {
                throw new VectanticMathException($"There has been an error during vector normalization: {ex.Message}", ex);
            }
        }

        return new Embedding() {Context = text, Vector = result};
    }

    public async Task<IReadOnlyList<Embedding>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct = default) {
        var txtsCount = texts.Count;
        var txts = EmbeddingGuard.ValidateAndNormalizeTexts(texts, txtsCount);

        var tokOutput = _tokenizer.TokenizeBatch(txts);
        var inputs = PackTokensToOnnxInputs(tokOutput, _resolvedModel.RequiresTokenTypeIds);
        using var onnxResult = await _onnxSession.RunAsync(inputs, ct);
        var outputs = OnnxOutputsToDenseTensor(onnxResult, _resolvedModel.OutputTensorName);
        var poolResult = _pooling.PoolBatch(outputs, tokOutput.AttentionMask);

        var hiddenDimLen = poolResult.GetLength(1);
        var embeddingResult = new List<Embedding>(txtsCount);
        
        for (int i = 0; i < txtsCount; i++) {
            var vector = ExtractRow(poolResult, i, hiddenDimLen);

            if (_opts.Normalize) {
                try { EmbeddingMath.Normalize(vector); }
                catch (ArgumentException ex) {
                    throw new VectanticMathException($"There has been an error during vector normalization: {ex.Message}", ex);
                }
            }

            embeddingResult.Add(new Embedding() {Context = txts[i], Vector = vector} );
        }
        
        return embeddingResult.AsReadOnly();
    }

    // -------------------- INNER METHS --------------------
    private static IReadOnlyCollection<NamedOnnxValue> PackTokensToOnnxInputs(
        TokenizerOutput tokOutput, bool requiresTokenTypeIds) 
    {
        var batchLen = tokOutput.InputIds.GetLength(0); 
        var seqLen = tokOutput.InputIds.GetLength(1);
        var shape = new int[2] { batchLen, seqLen };
        var size = batchLen * seqLen;

        var flatIds = new long[size];
        var flatMask = new long[size];
        
        Buffer.BlockCopy(tokOutput.InputIds, 0, flatIds, 0, size * sizeof(long));
        Buffer.BlockCopy(tokOutput.AttentionMask, 0, flatMask, 0, size * sizeof(long));

        var idsTensor = new DenseTensor<long>(flatIds, shape);
        var attMaskTensor = new DenseTensor<long>(flatMask, shape);

        var inputs = new List<NamedOnnxValue>(requiresTokenTypeIds ? 3 : 2) {
            NamedOnnxValue.CreateFromTensor(SemanticConstants.InputsId, idsTensor),
            NamedOnnxValue.CreateFromTensor(SemanticConstants.AttentionMask, attMaskTensor)
        };

        if (requiresTokenTypeIds) {
            var flatTypeIds = new long[size];
            var tokenIdsTensor = new DenseTensor<long>(flatTypeIds, shape);
            inputs.Add(
                NamedOnnxValue.CreateFromTensor(SemanticConstants.TokenTypeIds, tokenIdsTensor)
            );
        }
        
        return inputs.AsReadOnly();
    }

    private static DenseTensor<float> OnnxOutputsToDenseTensor(
        IDisposableReadOnlyCollection<DisposableNamedOnnxValue> onnxValues, string outTensorName) 
    {
        var hiddenState = onnxValues.FirstOrDefault(o => o.Name == outTensorName);
        return hiddenState is null 
            ? throw new VectanticModelException("Model could not produce output tensor")
            : hiddenState.AsTensor<float>() as DenseTensor<float> 
                ?? throw new VectanticModelException($"Output Tensor ({outTensorName}) could not be cast to DenseTensor.");
    }
    
    private static float[] ExtractRow(float[,] matrix, int row, int hiddenDim) {
        var vector = new float[hiddenDim];
        Buffer.BlockCopy(matrix, row * hiddenDim * sizeof(float), vector, 0, hiddenDim * sizeof(float));

        return vector;
    }    
}