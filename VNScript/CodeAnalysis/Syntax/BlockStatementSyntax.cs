using System.Collections.Immutable;

namespace VNScript.CodeAnalysis.Syntax;

public sealed class BlockStatementSyntax : StatementSyntax
{
    public SyntaxToken OpenBraceToken { get; }
    public ImmutableArray<StatementSyntax> Statements { get; }
    public SyntaxToken CloseBraceToken { get; }

    public BlockStatementSyntax(SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        Statements = statements;
        CloseBraceToken = closeBraceToken;
    }

    public override SyntaxKind Kind => SyntaxKind.BlockStatement;
}