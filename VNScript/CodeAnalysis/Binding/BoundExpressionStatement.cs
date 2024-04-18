using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class BoundExpressionStatement : BoundStatement
{
    public BoundExpression Expression { get; }

    public BoundExpressionStatement(BoundExpression expression)
    {
        Expression = expression;
    }

    public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;
}