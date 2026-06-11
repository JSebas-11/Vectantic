namespace Vectantic.Math.Internal.Utilities;

internal static class VectorGuard {
    internal static void NonEmptyOrException(ReadOnlySpan<float> v) {
        if (v.IsEmpty)
            throw new ArgumentException("Vector cannot be empty.");
    }

    internal static void SameLengthOrException(ReadOnlySpan<float> a, ReadOnlySpan<float> b) {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must have the same length.", nameof(b));
    }
    internal static void SameLengthOrException(ReadOnlySpan<float> a, int length) {
        if (a.Length != length)
            throw new ArgumentException($"Vector does not have the same length ({length}).", nameof(a));
    }
    
    internal static void NonZeroOrException(float value, string name) {
        if (value == 0f)
            throw new ArgumentException($"Vector's {name} must not be 0.");
    }
    
    internal static void ValidKOrException(int k, int candidatesCount) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(k, candidatesCount);
    }
    
    internal static void ValidMinScoreOrException(float minScore) {
        ArgumentOutOfRangeException.ThrowIfLessThan(minScore, -1f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minScore, 1f);
    }
}