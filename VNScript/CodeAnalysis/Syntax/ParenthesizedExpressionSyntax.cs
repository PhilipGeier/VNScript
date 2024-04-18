namespace VNScript.CodeAnalysis.Syntax;

public class ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken) : ExpressionSyntax
{
    public SyntaxToken OpenParenthesisToken { get; } = openParenthesisToken;
    public ExpressionSyntax Expression { get; } = expression;
    public SyntaxToken CloseParenthesisToken { get; } = closeParenthesisToken;
    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
}