using Microsoft.ML.Tokenizers;
using Vectantic.Semantic.Exceptions;
using Vectantic.Semantic.Internal.Models;
using Vectantic.Semantic.Internal.Utilities;

namespace Vectantic.Semantic.Internal.Tokenization;

internal sealed class WordPieceTokenizer : ISemanticTokenizer {
    // -------------------- INIT --------------------
    private readonly ResolvedSemanticModel _semanticModel;
    private readonly BertTokenizer _tokenizer;

    public WordPieceTokenizer(ResolvedSemanticModel semanticModel) {
        _semanticModel = semanticModel; 
        _tokenizer = BertTokenizer.Create(
            GetVocabFilePath(_semanticModel.TokenizerPath), 
            new BertOptions {
                LowerCaseBeforeTokenization = _semanticModel.LowerCase,
                ApplyBasicTokenization = true,
                SplitOnSpecialTokens = true,
                ClassificationToken = "[CLS]",
                SeparatorToken = "[SEP]",
                PaddingToken = "[PAD]",
                UnknownToken = "[UNK]"
            }
        );
    }

    // -------------------- METHS --------------------
    public TokenizerOutput Tokenize(string text) {
        TokenizerOutput tokFunc() {
            var ids = GetIds(text);
            
            var padId = (long)_tokenizer.PaddingTokenId;
            var idsCount = ids.Count;
            var maxTokens = _semanticModel.MaxTokens ?? idsCount;
            
            var inputIds = new long[1, maxTokens];
            var attentionMask = new long[1, maxTokens];

            for (int i = 0; i < maxTokens; i++) inputIds[0, i] = padId;

            for (int i = 0; i < idsCount; i++) {
                inputIds[0, i] = (long)ids[i];
                attentionMask[0, i] = 1L;
            }

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

            var padId = (long)_tokenizer.PaddingTokenId;

            var inputIds = new long[txtsCount, MaxLen];
            var attentionMask = new long[txtsCount, MaxLen];

            for (int i = 0; i < txtsCount; i++) {
                var batchIds = Ids[i];
                var batchIdsCount = Ids[i].Count;

                for (int j = 0; j < MaxLen; j++) inputIds[i, j] = padId;

                for (int j = 0; j < batchIdsCount; j++) {
                    inputIds[i, j] = (long)batchIds[j];
                    attentionMask[i, j] = 1L;
                }
            }

            return new TokenizerOutput(inputIds, attentionMask);
        }

        return SemanticExceptionHandler.Handle(
            tokBatchFunc, 
            ex => new VectanticTokenizationException($"Tokenization failed unexpectedly. {ex.Message}", ex)
        );
    }

    // -------------------- INNER METHS --------------------
    private static string GetVocabFilePath(string path) {
        var vocabPath = Path.Combine(path, "vocab.txt");
        
        if (!File.Exists(vocabPath))
            throw new VectanticTokenizationException($"Vocab.txt was not found at {vocabPath}.");

        return vocabPath;
    }
    
    private IReadOnlyList<int> GetIds(string text) {
        if (string.IsNullOrWhiteSpace(text))
            throw new VectanticTokenizationException("Text to tokenize was not provided.");

        IReadOnlyList<int> ids;
        try {
            ids = _tokenizer.EncodeToIds(
                text, 
                maxTokenCount: _semanticModel.MaxTokens ?? int.MaxValue,
                addSpecialTokens: true, 
                normalizedText: out _,
                charsConsumed: out int charsConsumed
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