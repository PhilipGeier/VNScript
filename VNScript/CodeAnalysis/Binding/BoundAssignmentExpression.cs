using VNScript.CodeAnalysis.Binding.Enums;

namespace VNScript.CodeAnalysis.Binding;

internal sealed class BoundAssignmentExpression : BoundExpression
{
    public VariableSymbol? Variable { get; }
    public BoundExpression Expression { get; }

    public BoundAssignmentExpression(VariableSymbol? variable, BoundExpression expression)
    {
        Variable = variable;
        Expression = expression;
    }

    public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
    public override Type Type => Expression.Type;
}

internal sealed class BoundVariableDeclaration : BoundStatement
{
    public VariableSymbol? Variable { get; }
    public BoundExpression Initializer { get; }

    public BoundVariableDeclaration(VariableSymbol? variable, BoundExpression initializer)
    {
        Variable = variable;
        Initializer = initializer;
    }

    public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;
}