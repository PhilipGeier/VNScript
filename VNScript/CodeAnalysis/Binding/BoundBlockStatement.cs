using System.Collections.Immutable;
using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class BoundBlockStatement : BoundStatement
{
    public ImmutableArray<BoundStatement> Statements { get; }

    public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
    {
        Statements = statements;
    }

    public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
}