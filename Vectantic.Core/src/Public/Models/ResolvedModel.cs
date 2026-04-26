namespace Vectantic.Core.Models;

public class ResolvedModel {
    public string ModelPath { get; }
    public int? MaxTokens { get; }

    protected ResolvedModel(string modelPath, int? maxTokens) {
        ModelPath = modelPath;
        MaxTokens = maxTokens;
    }
}