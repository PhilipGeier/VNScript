namespace VNScript.CodeAnalysis.Syntax;

public abstract class StatementSyntax : SyntaxNode
{
    
}

public sealed class VariableDeclarationSyntax : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken EqualsToken { get; }
    public ExpressionSyntax Initializer { get; }

    public VariableDeclarationSyntax(SyntaxToken keyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax initializer)
    {
        Keyword = keyword;
        Identifier = identifier;
        EqualsToken = equalsToken;
        Initializer = initializer;
    }
    
    public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
}