namespace VNScript.CodeAnalysis.Syntax;

public sealed class ElseClauseSyntax : SyntaxNode
{
    public SyntaxToken ElseKeyword { get; }
    public StatementSyntax ElseStatement { get; }

    public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax elseStatement)
    {
        ElseKeyword = elseKeyword;
        ElseStatement = elseStatement;
    }

    public override SyntaxKind Kind => SyntaxKind.ElseClause;
}