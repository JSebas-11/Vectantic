using Microsoft.ML.Tokenizers;
using Vectantic.Semantic.Exceptions;
using Vectantic.Semantic.Internal.Factories;
using Vectantic.Semantic.Internal.Models;
using Vectantic.Semantic.Internal.Utilities;

namespace Vectantic.Semantic.Internal.Tokenization;

internal sealed class BpeSemanticTokenizer : ISemanticTokenizer {
    // -------------------- INIT --------------------
    private readonly ResolvedSemanticModel _semanticModel;
    private readonly BpeTokenizer _tokenizer;
    private readonly Dictionary<string, int> _specialTokens;

    public BpeSemanticTokenizer(ResolvedSemanticModel semanticModel) {
        _semanticModel = semanticModel;
        _specialTokens = SpecialTokensFactory.SpecialTokensDict(_semanticModel.SpecialTokens, _semanticModel.SpecialTokensIds);
        var (vocab, merges) = GetVocabAndMergesPath(_semanticModel.TokenizerPath);
        var opts = new BpeOptions(vocab, merges) {
            ByteLevel = true,
            FuseUnknownTokens = false,
            // SpecialTokensFactory & SemanticModelFactory makes sure at least default values are provided
            UnknownToken = _semanticModel.SpecialTokens.UnkToken,
            BeginningOfSentenceToken = _semanticModel.SpecialTokens.BosToken,
            EndOfSentenceToken = _semanticModel.SpecialTokens.EosToken,
            SpecialTokens = _specialTokens
        };
        _tokenizer = BpeTokenizer.Create(opts);
    }

    // -------------------- METHS --------------------
    public TokenizerOutput Tokenize(string text) {
        TokenizerOutput tokFunc() {
            var ids = GetIds(text);
            
            // SemanticModelFactory makes sure PAD, BOF, EOF tokens are present, otherwise it will throw an exception
            var padId = (long)_specialTokens[_semanticModel.SpecialTokens.PadToken!];
            var bosId = (long)_specialTokens[_semanticModel.SpecialTokens.BosToken!];
            var eofId = (long)_specialTokens[_semanticModel.SpecialTokens.EosToken!];
            var idsCount = ids.Count;
            var maxTokens = _semanticModel.MaxTokens ?? (idsCount+2);
            
            var inputIds = new long[1, maxTokens];
            var attentionMask = new long[1, maxTokens];

            for (int i = 0; i < maxTokens; i++) inputIds[0, i] = padId;

            inputIds[0, 0] = bosId;
            attentionMask[0, 0] = 1L;

            for (int i = 0; i < idsCount; i++) {
                inputIds[0, i+1] = (long)ids[i];
                attentionMask[0, i+1] = 1L;
            }

            inputIds[0, idsCount+1] = eofId;
            attentionMask[0, idsCount+1] = 1L;

            return new TokenizerOutput(inputIds, attentionMask);
        }

        return SemanticExceptionHandler.Handle(
            tokFunc, 
            ex => new VectanticTokenizationException($"Tokenization failed unexpectedly. {ex.Message}", ex)
        );
    }

    public TokenizerOutput TokenizeBatch(IReadOnlyList<string> texts) {
        TokenizerOutput tokBatchFunc() {
            var txtsCount = texts.Count;

            var (MaxLen, Ids) = GetIdsAndMaxLen(texts, txtsCount);
            MaxLen += 2;

            // SemanticModelFactory makes sure PAD, BOF, EOF tokens are present, otherwise it will throw an exception
            var padId = (long)_specialTokens[_semanticModel.SpecialTokens.PadToken!];
            var bosId = (long)_specialTokens[_semanticModel.SpecialTokens.BosToken!];
            var eofId = (long)_specialTokens[_semanticModel.SpecialTokens.EosToken!];

            var inputIds = new long[txtsCount, MaxLen];
            var attentionMask = new long[txtsCount, MaxLen];

            for (int i = 0; i < txtsCount; i++) {
                var batchIds = Ids[i];
                var batchIdsCount = Ids[i].Count;

                for (int j = 0; j < MaxLen; j++) inputIds[i, j] = padId;

                inputIds[i, 0] = bosId;
                attentionMask[i, 0] = 1L;

                for (int j = 0; j < batchIdsCount; j++) {
                    inputIds[i, j+1] = (long)batchIds[j];
                    attentionMask[i, j+1] = 1L;
                }

                inputIds[i, batchIdsCount+1] = eofId;
                attentionMask[i, batchIdsCount+1] = 1L;
            }

            return new TokenizerOutput(inputIds, attentionMask);
        }

        return SemanticExceptionHandler.Handle(
            tokBatchFunc, 
            ex => new VectanticTokenizationException($"Tokenization failed unexpectedly. {ex.Message}", ex)
        );
    }

    // -------------------- INNER METHS --------------------
    private static (string vocab, string merges) GetVocabAndMergesPath(string path) {
        var vocabPath = Path.Combine(path, "vocab.json");
        var mergesPath = Path.Combine(path, "merges.txt");
        
        if (!File.Exists(vocabPath)) 
            throw new VectanticTokenizationException($"Vocab file was not found at {vocabPath}.");

        if (!File.Exists(mergesPath)) 
            throw new VectanticTokenizationException($"Merges file was not found at {mergesPath}.");
            
        return (vocabPath, mergesPath);
    }

    private IReadOnlyList<int> GetIds(string text) {
        if (string.IsNullOrWhiteSpace(text))
            throw new VectanticTokenizationException("Text to tokenize was not provided.");

        IReadOnlyList<int> ids;
        try {
            ids = _tokenizer.EncodeToIds(
                text,
                maxTokenCount: (_semanticModel.MaxTokens ?? int.MaxValue) - 2,
                normalizedText: out _,
                charsConsumed: out int charsConsumed,
                considerPreTokenization: true,
                considerNormalization: true
            );
            
            if (charsConsumed < text.Length)
                throw new VectanticTokenizationException(
                    $"Input text exceeds maximum token limit of {_semanticModel.MaxTokens}. " +
                    "Consider increasing MaxTokens or shortening your input.");

        }
        catch (VectanticTokenizationException) { throw; }
        catch (Exception ex) {
            throw new VectanticTokenizationException($"Tokenization failed unexpectedly. {ex.Message}", ex);
        }

        return ids;
    }

    private (int MaxLen, IReadOnlyList<int>[] Ids) GetIdsAndMaxLen(IReadOnlyList<string> texts, int count) {
        if (count == 0)
            throw new VectanticTokenizationException("At least one text must be provided for batch tokenization.");

        var ids = new IReadOnlyList<int>[count];
        var maxLen = 0;
        for (int i = 0; i < count; i++) {
            ids[i] = GetIds(texts[i]);
            var idLen = ids[i].Count;
            if (idLen > maxLen)
                maxLen = idLen;
        }

        return (maxLen, ids);
    }
}