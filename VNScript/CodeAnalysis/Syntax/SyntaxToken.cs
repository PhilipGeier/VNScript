using VNScript.CodeAnalysis.Text;

namespace VNScript.CodeAnalysis.Syntax;

public sealed class SyntaxToken(SyntaxKind kind, int position, string? text, object? value) : SyntaxNode
{
    public override SyntaxKind Kind { get; } = kind;
    public int Position { get; } = position;
    public string? Text { get; } = text;
    public object? Value { get; } = value;
    public override TextSpan Span => new(Position, Text?.Length ?? 0);
    public bool IsMissing => Text is null;
}