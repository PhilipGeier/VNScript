﻿namespace VNScript.CodeAnalysis.Syntax;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken LiteralToken { get; }
    public object? Value { get; }
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public LiteralExpressionSyntax(SyntaxToken literalToken)
        :this(literalToken, literalToken.Value)
    {
    }

    public LiteralExpressionSyntax(SyntaxToken literalToken, object? value)
    {
        LiteralToken = literalToken;
        Value = value;
    }
}