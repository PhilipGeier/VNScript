namespace VNScript.CodeAnalysis.Syntax;

public class UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand) : ExpressionSyntax
{
    public SyntaxToken OperatorToken { get; } = operatorToken;
    public ExpressionSyntax Operand { get; } = operand;
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
}