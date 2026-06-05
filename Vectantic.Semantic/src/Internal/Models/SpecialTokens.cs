namespace Vectantic.Semantic.Internal.Models;

internal sealed record SpecialTokens (
    string? UnkToken,
    string? SepToken,
    string? PadToken,
    string? MaskToken,
    string? ClsToken,
    string? BosToken,
    string? EosToken
);

internal sealed record SpecialTokensIds (
    int? UnkToken,
    int? SepToken,
    int? PadToken,
    int? MaskToken,
    int? ClsToken,
    int? BosToken,
    int? EosToken
);