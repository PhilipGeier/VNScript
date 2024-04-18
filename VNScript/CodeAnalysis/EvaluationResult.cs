using System.Collections.Immutable;

namespace VNScript.CodeAnalysis;

public sealed class EvaluationResult
{
    public object? Value { get; }
    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object? value)
    {
        Value = value;
        Diagnostics = diagnostics;
    }
}