namespace VNScript.CodeAnalysis.Syntax;

public sealed class ExpressionStatementSyntax : StatementSyntax
{
    public ExpressionSyntax Expression { get; }

    // TODO: Add semicolon
    public ExpressionStatementSyntax(ExpressionSyntax expression)
    {
        Expression = expression;
    }


    public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
}