namespace VNScript.CodeAnalysis.Syntax;

public sealed class BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) : ExpressionSyntax
{
    public ExpressionSyntax Left { get; } = left;
    public SyntaxToken OperatorToken { get; } = operatorToken;
    public ExpressionSyntax Right { get; } = right;

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
}