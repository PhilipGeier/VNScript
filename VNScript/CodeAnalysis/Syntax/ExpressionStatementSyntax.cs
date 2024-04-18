namespace VNScript.CodeAnalysis.Syntax;

public sealed class ExpressionStatementSyntax : StatementSyntax
{
    public ExpressionSyntax Expression { get; }

    // TODO: Add semicolon
    // public SyntaxToken SemiColonToken { get; } 
    public ExpressionStatementSyntax(ExpressionSyntax expression /*, SyntaxToken semiColonToken*/)
    {
        Expression = expression;
        // SemiColonToken = semiColonToken;
    }
    
    public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
}